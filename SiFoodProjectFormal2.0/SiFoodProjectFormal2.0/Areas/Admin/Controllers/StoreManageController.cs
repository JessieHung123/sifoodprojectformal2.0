using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SiFoodProjectFormal2._0.Models;

namespace SiFoodProjectFormal2._0.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class StoreManageController : Controller
    {
        private readonly Sifood3Context _context;

        public StoreManageController(Sifood3Context context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index(string searchString)
        {
            IQueryable<Store> stores = _context.Stores;
            if (!string.IsNullOrEmpty(searchString))
            {
                stores = stores.Where(u => u.StoreName.Contains(searchString));
            }
            return View(await stores.ToListAsync());
        }

        public IActionResult Details(string storeId)
        {
            Store store = _context.Stores.Where(x => x.StoreId == storeId).FirstOrDefault();
            return PartialView("_DetailsPartialView", store);
        }

        public IActionResult Edit(string storeId)
        {
            Store store = _context.Stores.Where(x => x.StoreId == storeId).FirstOrDefault();
            return PartialView("_EditPartialView", store);
        }

        [HttpPost]
        public IActionResult Edit(Store store)
        {
            _context.Stores.Update(store);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        public IActionResult Delete(string storeId)
        {
            Store store = _context.Stores.Where(x => x.StoreId == storeId).FirstOrDefault();
            return PartialView("_DeletePartialView", store);
        }

        [HttpPost]
        public IActionResult Delete(Store store)
        {
            _context.Stores.Remove(store);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

    }
}
