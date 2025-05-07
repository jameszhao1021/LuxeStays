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
    [Authorize(Roles = SD.Role_Admin)]
    public class VillaNumberController : Controller
    {
        //private readonly ApplicationDbContext _db;

        //public VillaNumberController(ApplicationDbContext db)
        //{
        //    _db = db;
        //}

        private readonly IUnitOfWork _unitOfWork;

        public VillaNumberController(IUnitOfWork UnitOfWork)
        {
            _unitOfWork = UnitOfWork;
        }

        public IActionResult Index()
        {
            var villaNumbers = _unitOfWork.VillaNumber.GetAll(includeProperties:"Villa");
            return View(villaNumbers);
        }

        public IActionResult Create()
        {

            VillaNumberVM VillaNumberVM = new VillaNumberVM()
            {
                VillaList = _unitOfWork.Villa.GetAll().Select(item => new SelectListItem
                {
                    Text = item.Name,
                    Value = item.Id.ToString()
                })
            };
            //IEnumerable<SelectListItem> list = _db.Villas.ToList().Select(item => new SelectListItem
            //{
            //    Text = item.Name,
            //    Value = item.Id.ToString()
            //});
            //ViewBag.VillaList = list;

            return View(VillaNumberVM);
        }

        [HttpPost]
        public IActionResult Create(VillaNumberVM villaNumberVM)
        {

            //bool roomNumberExist = _db.VillaNumbers.Any(u => u.Villa_Number == villaNumberVM.VillaNumber.Villa_Number);
            bool roomNumberExist = _unitOfWork.VillaNumber.Any(u => u.Villa_Number == villaNumberVM.VillaNumber.Villa_Number);


            if (ModelState.IsValid && !roomNumberExist)
            {
                _unitOfWork.VillaNumber.Add(villaNumberVM.VillaNumber);
                _unitOfWork.Save();
                TempData["success"] = "The villa number has been created successfully.";
                return RedirectToAction("Index");
            }

            if (roomNumberExist)
            {
                TempData["error"] = "The number has already existed.";
            }

            villaNumberVM.VillaList = _unitOfWork.Villa.GetAll().Select(item => new SelectListItem
            {
                Text = item.Name,
                Value = item.Id.ToString()
            });
            
            return View(villaNumberVM);
        }

        public IActionResult Update(int villaNumberId)
        {

            VillaNumberVM villaNumberVM = new VillaNumberVM()
            {
                VillaList = _unitOfWork.Villa.GetAll().Select(item => new SelectListItem
                {
                    Text = item.Name,
                    Value = item.Id.ToString()
                }),
                VillaNumber = _unitOfWork.VillaNumber.Get(villaNumber => villaNumber.Villa_Number == villaNumberId)

            };
            if (villaNumberVM.VillaNumber == null)
            {
                return RedirectToAction("Error", "Home");
            }

            return View(villaNumberVM);
        }


        [HttpPost]
        //public IActionResult Update(VillaNumber villaNumber)
        //{

        //    if (ModelState.IsValid && villaNumber.Villa_Number > 0)
        //    {

        //        _db.VillaNumbers.Update(villaNumber);
        //        _db.SaveChanges();
        //        TempData["success"] = "The villa number has been updated successfully.";
        //        return RedirectToAction("Index");
        //    }
        //    return View();
        //}

        public IActionResult Update(VillaNumberVM villaNumberVM)
        {

            if (ModelState.IsValid)
            {
                _unitOfWork.VillaNumber.Update(villaNumberVM.VillaNumber);
                _unitOfWork.Save();
                TempData["success"] = "The villa number has been updated successfully.";
                return RedirectToAction("Index");
            }
            villaNumberVM.VillaList = _unitOfWork.Villa.GetAll().Select(item => new SelectListItem
            {
                Text = item.Name,
                Value = item.Id.ToString()
            });

            return View(villaNumberVM);
        }

        public IActionResult Delete(int villaNumberId)
        {
            VillaNumberVM villaNumberVM = new VillaNumberVM()
            {
                VillaList = _unitOfWork.Villa.GetAll().Select(item => new SelectListItem
                {
                    Text = item.Name,
                    Value = item.Id.ToString()
                }),
                VillaNumber = _unitOfWork.VillaNumber.Get(villaNumber => villaNumber.Villa_Number == villaNumberId)
            };
            //VillaNumber? deleteVillaNumber = _db.VillaNumbers.FirstOrDefault(villaNumber => villaNumber.Villa_Number == villaNumberId);
            if (villaNumberVM.VillaNumber == null)
            {
                return RedirectToAction("Error", "Home");
            }

            return View(villaNumberVM);
        }

        [HttpPost]
       
        public IActionResult Delete(VillaNumberVM villaNumberVM)
        {
           

            VillaNumber? deleteVillaNumber = _unitOfWork.VillaNumber.Get(villaNumber => villaNumber.Villa_Number == villaNumberVM.VillaNumber.Villa_Number);

            if (deleteVillaNumber != null)
            {
                _unitOfWork.VillaNumber.Remove(deleteVillaNumber);
                _unitOfWork.Save();
                TempData["success"] = "The villa Number has been deleted successfully.";
                return RedirectToAction("Index");
            }
            TempData["error"] = "The villa number could not be deleted.";

           

            return View();
        }
    }
}
