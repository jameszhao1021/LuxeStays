using LuxeStays.Domain.Entities;
using LuxeStays.Infrastructure.Data;
using LuxeStays.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Collections;
using System.Runtime.InteropServices;

namespace LuxeStays.Web.Controllers
{
    public class VillaNumberController : Controller
    {
        private readonly ApplicationDbContext _db;

        public VillaNumberController(ApplicationDbContext db)
        {
            _db = db;
        }


        public IActionResult Index()
        {
            var villaNumbers = _db.VillaNumbers.Include(villaNumber=>villaNumber.Villa).ToList();
            return View(villaNumbers);
        }

        public IActionResult Create()
        {

            VillaNumberVM VillaNumberVM = new VillaNumberVM()
            {
                VillaList = _db.Villas.ToList().Select(item => new SelectListItem
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
           
            bool roomNumberExist = _db.VillaNumbers.Any(u => u.Villa_Number == villaNumberVM.VillaNumber.Villa_Number);

            if (ModelState.IsValid && !roomNumberExist)
            {
                _db.VillaNumbers.Add(villaNumberVM.VillaNumber);
                _db.SaveChanges();
                TempData["success"] = "The villa number has been created successfully.";
                return RedirectToAction("Index");
            }

            if (roomNumberExist)
            {
                TempData["error"] = "The number has already existed.";
            }

            villaNumberVM.VillaList = _db.Villas.ToList().Select(item => new SelectListItem
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
                VillaList = _db.Villas.ToList().Select(item => new SelectListItem
                {
                    Text = item.Name,
                    Value = item.Id.ToString()
                }),
                VillaNumber = _db.VillaNumbers.FirstOrDefault(villaNumber => villaNumber.Villa_Number == villaNumberId)

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
                _db.VillaNumbers.Update(villaNumberVM.VillaNumber);
                _db.SaveChanges();
                TempData["success"] = "The villa number has been updated successfully.";
                return RedirectToAction("Index");
            }
            villaNumberVM.VillaList = _db.Villas.ToList().Select(item => new SelectListItem
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
                VillaList = _db.Villas.ToList().Select(item => new SelectListItem
                {
                    Text = item.Name,
                    Value = item.Id.ToString()
                }),
                VillaNumber = _db.VillaNumbers.FirstOrDefault(villaNumber => villaNumber.Villa_Number == villaNumberId)
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
           

            VillaNumber? deleteVillaNumber = _db.VillaNumbers.FirstOrDefault(villaNumber => villaNumber.Villa_Number == villaNumberVM.VillaNumber.Villa_Number);

            if (deleteVillaNumber != null)
            {
                _db.VillaNumbers.Remove(deleteVillaNumber);
                _db.SaveChanges();
                TempData["success"] = "The villa Number has been deleted successfully.";
                return RedirectToAction("Index");
            }
            TempData["error"] = "The villa number could not be deleted.";

           

            return View();
        }
    }
}
