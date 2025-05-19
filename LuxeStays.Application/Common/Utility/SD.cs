using LuxeStays.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuxeStays.Application.Common.Utility
{
    public static class SD
    {
        public const string Role_Customer = "Customer";
        public const string Role_Admin = "Admin";
        public const string StatusPending = "Pending";
        public const string StatusApproved = "Approved";

        public const string StatusCheckedIn = "CheckedIn";

        public const string StatusCompleted = "Completed";

        public const string StatusCanceled = "Canceled";

        public const string StatusRefunded = "Refunded";

        public static int VillaRomsAvailable_Count(int villaId, List<VillaNumber> villaNumberList, DateOnly checkInDate, int nights, List<Booking> bookings)
        {
            List<int> bookingInDate = new();

            var roomsInVilla = villaNumberList.Where(villaNumber => villaNumber.VillaId == villaId).Count();
            var finalAvailableRooms = int.MaxValue;
            for (int i = 0; i < nights; i++)
            {
                var villasBooked = bookings.Where(booking => booking.CheckInDate <= checkInDate.AddDays(i)
                && booking.CheckOutDate > checkInDate.AddDays(i) && booking.VillaId == villaId);
                foreach (var booking in villasBooked)
                {
                    if (!bookingInDate.Contains(booking.Id))
                    {
                        bookingInDate.Add(booking.Id);
                    }
                }
                var totalAvailableRooms = roomsInVilla - bookingInDate.Count();
                if (totalAvailableRooms == 0)
                {
                    return 0;
                }
                else
                {
                    if (totalAvailableRooms < finalAvailableRooms)
                    {
                        finalAvailableRooms = totalAvailableRooms;
                    }
                }
                
            }
            return finalAvailableRooms;
        }

    }
}

