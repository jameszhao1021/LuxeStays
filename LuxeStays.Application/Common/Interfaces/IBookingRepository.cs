using LuxeStays.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuxeStays.Application.Common.Interfaces
{
    public interface IBookingRepository : IRepository<Booking>
    {
        void Update(Booking booking);
        void UpdateStatus(int bookingId, string bookingStatus);
        void UpdateStripePaymentId(int bookingId, string sessionId, string paymentIntentId);
    }
}
