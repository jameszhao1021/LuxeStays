using LuxeStays.Infrastructure.Repository;
using Microsoft.AspNetCore.Mvc;
using LuxeStays.Domain.Entities;
using LuxeStays.Application.Common.Interfaces;
using Microsoft.AspNetCore.Identity;
using LuxeStays.Application.Common.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Stripe.Checkout;
using Stripe;
namespace LuxeStays.Web.Controllers
{
    
    public class BookingController : Controller
    {     
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;
        public BookingController (IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }
        [Authorize]
        public async Task<IActionResult> FinaliseBooking(int villaId, DateOnly checkInDate, int nights) {
            var user = await _userManager.GetUserAsync(User);
            Booking booking = new Booking()
            {
                VillaId = villaId,
                villa = _unitOfWork.Villa.Get(u => u.Id == villaId, includeProperties: "VillaAmenity"),
                CheckInDate = checkInDate,
                Nights = nights,
                CheckOutDate = checkInDate.AddDays(nights),
                UserId = user.Id,
                Name = user.Name,
                Phone = user.PhoneNumber,
                Email = user.Email
            };
                booking.TotalCost = booking.Nights * booking.villa.Price;        
                return View(booking);
        }

        [Authorize]
        [HttpPost]
        public IActionResult FinaliseBooking(Booking booking)
        {
            var villa =  _unitOfWork.Villa.Get(villa => villa.Id == booking.VillaId);
            booking.TotalCost = booking.Nights * villa.Price;
            booking.Status = SD.StatusPending;
            booking.BookingDate = DateTime.Now;
            _unitOfWork.Booking.Add(booking);
            _unitOfWork.Save();

            var domain = Request.Scheme + "://" + Request.Host.Value + "/";
            var options = new SessionCreateOptions
            {
                LineItems = new List<SessionLineItemOptions>(),
                Mode = "payment",
                SuccessUrl = domain + $"booking/BookingConfirmation?bookingId={booking.Id}",
                CancelUrl = domain + $"booking/FinaliseBooking?villaId={booking.VillaId}&checkIndate={booking.CheckInDate}&nights={booking.Nights}",
            };
            options.LineItems.Add(new SessionLineItemOptions
            {
                PriceData = new SessionLineItemPriceDataOptions
                {
                    UnitAmount = (long)(booking.TotalCost * 100),
                    Currency = "aud",
                    ProductData = new SessionLineItemPriceDataProductDataOptions
                    {
                        Name = villa.Name,
                        //Images = new List<string> { domain + villa.ImageUrl }
                    },
                },
                Quantity  = 1,
            });
                                       
            var service = new SessionService();
            Session session = service.Create(options);
            
            _unitOfWork.Booking.UpdateStripePaymentId(booking.Id,session.Id, session.PaymentIntentId);
            _unitOfWork.Save();

            Response.Headers.Add("Location", session.Url);
            return new StatusCodeResult(303);
            //return RedirectToAction(nameof(BookingConfirmation), new {bookingId = booking.Id});
        }

        public IActionResult BookingConfirmation(int bookingId)
        {
            Booking booking = _unitOfWork.Booking.Get(booking=>booking.Id == bookingId, includeProperties:"User,villa");
            if(booking.Status == SD.StatusPending)
            {
                var service = new SessionService();
                Session session = service.Get(booking.StripeSessionId);
                if(session.PaymentStatus == "paid")
                {
                    _unitOfWork.Booking.UpdateStatus(booking.Id, SD.StatusApproved);
                    _unitOfWork.Booking.UpdateStripePaymentId(booking.Id, session.Id, session.PaymentIntentId);
                    _unitOfWork.Save();
                }
            }
            return View(bookingId);
        }
    }
}
