using LuxeStays.Infrastructure.Repository;
using Microsoft.AspNetCore.Mvc;
using LuxeStays.Domain.Entities;
using LuxeStays.Application.Common.Interfaces;
using Microsoft.AspNetCore.Identity;
using LuxeStays.Application.Common.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
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
            return RedirectToAction(nameof(BookingConfirmation), new {bookingId = booking.Id});
        }

        public IActionResult BookingConfirmation(int bookingId)
        {
            return View(bookingId);
        }
    }
}
