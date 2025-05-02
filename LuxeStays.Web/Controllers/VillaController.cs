using LuxeStays.Application.Common.Interfaces;
using LuxeStays.Domain.Entities;
using LuxeStays.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Runtime.InteropServices;

namespace LuxeStays.Web.Controllers
{
    public class VillaController : Controller
    {
        //private readonly ApplicationDbContext _db;

        //private readonly IVillaRepository _villaRepo;
        private readonly IUnitOfWork _unitOfWork;

        //public VillaController(ApplicationDbContext db)
        //{
        //    _db = db;
        //}

        //public VillaController(IVillaRepository villaRepo)
        //{
        //    _villaRepo = villaRepo;
        //}

        public VillaController(IUnitOfWork UnitOfWork)
        {
            _unitOfWork = UnitOfWork;
        }

        //public IActionResult Index()
        //{
        //    var villas = _db.Villas.ToList();
        //    return View(villas);
        //}

        public IActionResult Index()
        {
            var villas = _unitOfWork.Villa.GetAll();
            return View(villas);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Villa villa)
        {
            if (villa.Name == villa.Description)
            {
                ModelState.AddModelError("Description", "The description connot exactly match the Name.");
            }
            if (ModelState.IsValid)
            {
                //_db.Villas.Add(villa);
                _unitOfWork.Villa.Add(villa);
                //_db.SaveChanges();
                _unitOfWork.Save();
                TempData["success"] = "The villa has been created successfully.";
                return RedirectToAction("Index");
            }
            return View();
        }

        public IActionResult Update(int villaId)
        {
            //Villa? updateVilla = _db.Villas.FirstOrDefault(villa=>villa.Id == villaId);
            Villa? updateVilla = _unitOfWork.Villa.Get(villa => villa.Id == villaId);

            if (updateVilla == null)
            {
                return RedirectToAction("Error", "Home");
            }
            
            return View(updateVilla);
        }

        [HttpPost]
        public IActionResult Update(Villa villa)
        {

            if (ModelState.IsValid && villa.Id > 0)
            {
                //_db.Villas.Update(villa);
                _unitOfWork.Villa.Update(villa);
                //_db.SaveChanges();
                _unitOfWork.Save();
                TempData["success"] = "The villa has been updated successfully.";
                return RedirectToAction("Index");
            }
            return View();
        }

        public IActionResult Delete(int villaId) {
            //Villa? deleteVilla = _db.Villas.FirstOrDefault(villa => villa.Id == villaId);
            Villa? deleteVilla = _unitOfWork.Villa.Get(villa => villa.Id == villaId);

            if (deleteVilla == null)
            {
                return RedirectToAction("Error", "Home");
            }

            return View(deleteVilla);
        }

        [HttpPost] 
     
        public IActionResult Delete(Villa villa)
        {
            //Villa? deleteVilla = _db.Villas.FirstOrDefault(item => item.Id == villa.Id);
            Villa? deleteVilla = _unitOfWork.Villa.Get(item => item.Id == villa.Id);

            if (deleteVilla !=null)
            {
                //_db.Villas.Remove(deleteVilla);
                _unitOfWork.Villa.Remove(deleteVilla);

                //_db.SaveChanges();
                _unitOfWork.Save();
                TempData["success"] = "The villa has been deleted successfully.";
                return RedirectToAction("Index");
            }
            TempData["error"] = "The villa could not be deleted.";

            return View();

        }
    }
}
