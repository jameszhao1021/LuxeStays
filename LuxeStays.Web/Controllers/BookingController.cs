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
using System.Security.Claims;
using System.Linq;

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
        public IActionResult Index()
        {
            var bookings = _unitOfWork.Booking.GetAll(includeProperties: "User,villa");
            return View(bookings);
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
                    _unitOfWork.Booking.UpdateStatus(booking.Id, SD.StatusApproved,0);
                    _unitOfWork.Booking.UpdateStripePaymentId(booking.Id, session.Id, session.PaymentIntentId);
                    _unitOfWork.Save();
                }
            }
            return View(bookingId);
        }

        [Authorize]
        public IActionResult BookingDetails(int bookingId) {
            var booking = _unitOfWork.Booking.Get(booking => booking.Id == bookingId, includeProperties: "User,villa");
            if (booking.VillaNumber == 0 && booking.Status == SD.StatusApproved) {
                var availableVillaNumbers = AssignAvailableVillaNumberByVilla(booking.VillaId);
                booking.VillaNumbers = _unitOfWork.VillaNumber.GetAll(villaNumber => villaNumber.VillaId == booking.VillaId &&
                availableVillaNumbers.Contains(villaNumber.Villa_Number)).ToList();
            }
            return View(booking);
        }

        private  List<int>  AssignAvailableVillaNumberByVilla(int villaId) {
            List<int> availableVillaNumbers = new();
            var villaNumbers = _unitOfWork.VillaNumber.GetAll(villaNumber=>villaNumber.VillaId == villaId);
            var checkedInVilla = _unitOfWork.Booking.GetAll(booking => booking.VillaId == villaId && booking.Status == SD.StatusCheckedIn).Select(booking=>booking.VillaNumber);
            foreach (var villaNumber in villaNumbers)
            {
                if (!checkedInVilla.Contains(villaNumber.Villa_Number)){
                    availableVillaNumbers.Add(villaNumber.Villa_Number);
                }
            }
            return availableVillaNumbers;
        }

        [HttpPost]
        [Authorize(Roles = SD.Role_Admin)]
        public IActionResult CheckIn(Booking booking)
        {

        }

        #region API Calls
        [HttpGet]
        [Authorize]
        public IActionResult GetAll(string status)
        {
            IEnumerable<Booking> objBookings;
            if (User.IsInRole(SD.Role_Admin))
            {
                objBookings = _unitOfWork.Booking.GetAll(includeProperties:"User,villa");
            }
            else
            {
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
                objBookings = _unitOfWork.Booking.GetAll(booking => booking.UserId == userId, includeProperties: "User,villa");
            }
            if (!string.IsNullOrEmpty(status)) {
                objBookings = objBookings.Where(obj => obj.Status.ToLower() == status.ToLower());
            }
            return Json(new {data= objBookings});   
        }

        #endregion
    }
}
