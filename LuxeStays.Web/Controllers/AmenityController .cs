using LuxeStays.Application.Common.Interfaces;
using LuxeStays.Application.Common.Utility;
using LuxeStays.Domain.Entities;
using LuxeStays.Infrastructure.Data;
using LuxeStays.Infrastructure.Repository;
using LuxeStays.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Collections;
using System.Runtime.InteropServices;

namespace LuxeStays.Web.Controllers
{
    [Authorize(Roles =SD.Role_Admin)]
    public class AmenityController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public AmenityController(IUnitOfWork UnitOfWork)
        {
            _unitOfWork = UnitOfWork;
        }
        public IActionResult Index()
        {
            var amenities = _unitOfWork.Amenity.GetAll(includeProperties:"Villa");
            return View(amenities);
        }

        public IActionResult Create()
        {
            AmenityVM AmenityVM = new AmenityVM()
            {
                VillaList = _unitOfWork.Villa.GetAll().Select(item => new SelectListItem
                {
                    Text = item.Name,
                    Value = item.Id.ToString()
                })
            };
            return View(AmenityVM);
        }

        [HttpPost]
        public IActionResult Create(AmenityVM amenityVM)
        {

            if (ModelState.IsValid)
            {
                _unitOfWork.Amenity.Add(amenityVM.Amenity);
                _unitOfWork.Save();
                TempData["success"] = "The amenity has been created successfully.";
                return RedirectToAction("Index");
            }

            amenityVM.VillaList = _unitOfWork.Villa.GetAll().Select(item => new SelectListItem
            {
                Text = item.Name,
                Value = item.Id.ToString()
            });
            
            return View(amenityVM);
        }

        public IActionResult Update(int amenityId)
        {

            AmenityVM amenityVM = new AmenityVM()
            {
                VillaList = _unitOfWork.Villa.GetAll().Select(item => new SelectListItem
                {
                    Text = item.Name,
                    Value = item.Id.ToString()
                }),
                Amenity = _unitOfWork.Amenity.Get(amenity => amenity.Id == amenityId)

            };
            if (amenityVM.Amenity == null)
            {
                return RedirectToAction("Error", "Home");
            }

            return View(amenityVM);
        }


        [HttpPost]

        public IActionResult Update(AmenityVM amenityVM)
        {

            if (ModelState.IsValid)
            {
                _unitOfWork.Amenity.Update(amenityVM.Amenity);
                _unitOfWork.Save();
                TempData["success"] = "The amenity has been updated successfully.";
                return RedirectToAction("Index");
            }
            amenityVM.VillaList = _unitOfWork.Villa.GetAll().Select(item => new SelectListItem
            {
                Text = item.Name,
                Value = item.Id.ToString()
            });

            return View(amenityVM);
        }

        public IActionResult Delete(int amenityId)
        {
            AmenityVM amenityVM = new AmenityVM()
            {
                VillaList = _unitOfWork.Villa.GetAll().Select(item => new SelectListItem
                {
                    Text = item.Name,
                    Value = item.Id.ToString()
                }),
                Amenity = _unitOfWork.Amenity.Get(amenity => amenity.Id == amenityId)
            };
            if (amenityVM.Amenity == null)
            {
                return RedirectToAction("Error", "Home");
            }

            return View(amenityVM);
        }

        [HttpPost]
       
        public IActionResult Delete(AmenityVM amenityVM)
        {
           
            Amenity? deleteAmenity = _unitOfWork.Amenity.Get(amenity => amenity.Id == amenityVM.Amenity.Id);

            if (deleteAmenity != null)
            {
                _unitOfWork.Amenity.Remove(deleteAmenity);
                _unitOfWork.Save();
                TempData["success"] = "The amenity has been deleted successfully.";
                return RedirectToAction("Index");
            }
            TempData["error"] = "The amenity could not be deleted.";

            return View();
        }
    }
}
