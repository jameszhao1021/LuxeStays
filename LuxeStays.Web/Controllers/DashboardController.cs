using LuxeStays.Application.Common.Interfaces;
using LuxeStays.Application.Common.Utility;
using LuxeStays.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace LuxeStays.Web.Controllers
{
    public class DashboardController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        static int previousMonth = DateTime.UtcNow.Month == 1 ? 12 : DateTime.UtcNow.Month - 1;
        readonly DateTime previousMonthStartDate = new (DateTime.UtcNow.Year, previousMonth, 1);
        readonly DateTime currentMonthStartDate = new(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);
        private static RadialBarChartVM GetRadialChartDataModel(int totalCount, double currentMonthCount, double prevMonthCount)
        {
            RadialBarChartVM radialBarChartVM = new();

            int increaseDecreaseRatio = 100;
            if (prevMonthCount != 0)
            {
                increaseDecreaseRatio = Convert.ToInt32((currentMonthCount - prevMonthCount) / prevMonthCount * 100);
            }

            radialBarChartVM.TotalCount = totalCount;
            radialBarChartVM.CountInCurrentMonth = Convert.ToInt32(currentMonthCount);
            radialBarChartVM.HasRatioIncrease = currentMonthCount > prevMonthCount;
            radialBarChartVM.Series = new int[] { increaseDecreaseRatio };
            return radialBarChartVM;
        }

        public DashboardController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
         
            return View();
        }

        public async Task<IActionResult> GetTotalBookingRadialChartData()
        {
            var totalBookings = _unitOfWork.Booking.GetAll(u => u.Status != SD.StatusPending && u.Status != SD.StatusCanceled);

            var countByCurrentMonth = totalBookings.Count(u => u.BookingDate >= currentMonthStartDate && u.BookingDate <= DateTime.UtcNow);

            var countByPreviousMonth = totalBookings.Count(u => u.BookingDate >= previousMonthStartDate && u.BookingDate <= currentMonthStartDate);

            RadialBarChartVM radialBarChartVM = new();

            return Json(GetRadialChartDataModel(totalBookings.Count(), countByCurrentMonth, countByPreviousMonth));

        }

        public async Task<IActionResult> GetRegisteredUserRadialChartData()
        {
            var totalUsers = _unitOfWork.User.GetAll();

            var countByCurrentMonth = totalUsers.Count(u => u.CreatedAt >= currentMonthStartDate && u.CreatedAt <= DateTime.UtcNow);

            var countByPreviousMonth = totalUsers.Count(u => u.CreatedAt >= previousMonthStartDate && u.CreatedAt <= currentMonthStartDate);

            return Json(GetRadialChartDataModel(totalUsers.Count(), countByCurrentMonth, countByPreviousMonth));

        }

        public async Task<IActionResult> GetRevenueRadialChartData()
        {
            var totalBookings = _unitOfWork.Booking.GetAll(u => u.Status != SD.StatusPending && u.Status != SD.StatusCanceled);

            var totalRevenue = Convert.ToInt32(totalBookings.Sum(u => u.TotalCost));



            var countByCurrentMonth = totalBookings.Where(u => u.BookingDate >= currentMonthStartDate && u.BookingDate <= DateTime.UtcNow).Sum(u => u.TotalCost);

            var countByPreviousMonth = totalBookings.Where(u => u.BookingDate >= previousMonthStartDate && u.BookingDate <= currentMonthStartDate).Sum(u => u.TotalCost);

            return Json(GetRadialChartDataModel(totalRevenue, countByCurrentMonth, countByPreviousMonth));

        }

        public async Task<IActionResult> GetBookingPieChartData()
        {
            var totalBookings = _unitOfWork.Booking.GetAll(u => u.BookingDate >= DateTime.UtcNow.AddDays(-30) && (u.Status != SD.StatusPending && u.Status != SD.StatusCanceled) );

            var customerWithOneBooking = totalBookings.GroupBy(u => u.UserId).Where(i => i.Count() == 1).Select(x=>x.Key).ToList();

            int bookingsByNewCustomer = customerWithOneBooking.Count();

            var bookingsByReturnCustomer = totalBookings.Count() - bookingsByNewCustomer;

           
            PieChartVM pieChartVM = new();

            pieChartVM.Series = new decimal[] { bookingsByNewCustomer, bookingsByReturnCustomer };

            pieChartVM.Labels = new string[] { "New Customer Bookings", "Returning Customer Bookings" };



            return Json(pieChartVM);

        }

        public async Task<IActionResult> GetMemberAndBookingLineChartData()
        {
            var bookingdata = _unitOfWork.Booking.GetAll(u => u.BookingDate >= DateTime.UtcNow.AddDays(-30) && u.BookingDate.Date <= DateTime.UtcNow)
                .GroupBy(b => b.BookingDate)
                .Select(u => new
                {
                    DateTime = u.Key,
                    NewBookingCount = u.Count()
                });

            var customerdata = _unitOfWork.User.GetAll(u => u.CreatedAt >= DateTime.UtcNow.AddDays(-30) && u.CreatedAt.Date <= DateTime.UtcNow)
                .GroupBy(b => b.CreatedAt)
                .Select(u => new
                {
                    DateTime = u.Key,
                    NewCustomerCount = u.Count()
                });

            var leftJoin = bookingdata.GroupJoin(customerdata, booking => booking.DateTime, customer => customer.DateTime,
                (booking, customer) => new
                {
                    booking.DateTime,
                    booking.NewBookingCount,
                    NewCustomerCount = customer.Select(x => x.NewCustomerCount).FirstOrDefault()
                });

            var rightJoin = customerdata.GroupJoin(bookingdata, customer => customer.DateTime, booking => booking.DateTime,
                (customer,booking ) => new
                {
                    customer.DateTime,
                    NewBookingCount = booking.Select(x => x.NewBookingCount).FirstOrDefault(),
                    customer.NewCustomerCount
                });
            var mergedData = leftJoin.Union(rightJoin).OrderBy(x=>x.DateTime).ToList();
            var newBookingData = mergedData.Select(x=>x.NewBookingCount).ToArray();
            var newCustomerData = mergedData.Select(x => x.NewCustomerCount).ToArray();
            var categories = mergedData.Select(x => x.DateTime.ToString("MM/dd/yyyy")).ToArray();

            List<ChartData> chartDataList = new()
            {
                new ChartData
                {
                    Name = "New Bookings",
                    Data = newBookingData
                },
                new ChartData
                {
                    Name = "New Members",
                    Data = newCustomerData
                }
            };
            LineChartVM lineChartVM = new()
            {
                Categories = categories,
                Series = chartDataList
            };


            return Json(lineChartVM);
        }


        }
    
}
