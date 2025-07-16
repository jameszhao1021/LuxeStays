using LuxeStays.Application.Common.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LuxeStays.Web.Controllers
{
    public class DashboardController : Controller
    {

        private readonly IUnitOfWork _unitOfWork;

        public DashboardController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
         
            return View();
        }
    }
}
