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
        public IActionResult Index(int page = 1, int pageSize = 5, string searchStores = null)
        {
            // 從 TempData 中讀取搜索條件值
            if (!string.IsNullOrEmpty(searchStores))
            {
                TempData["SearchStores"] = searchStores;
            }
            else
            {
                searchStores = TempData["SearchStores"] as string ?? "";
            }

            var query = _context.Stores.AsQueryable();

            if (!string.IsNullOrEmpty(searchStores))
            {
                query = query.Where(u => u.StoreName.Contains(searchStores) || u.ContactName.Contains(searchStores));
            }

            var totalEntries = query.Count();
            var stores = query.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            var entriesStart = (page - 1) * pageSize + 1;
            var entriesEnd = entriesStart + stores.Count - 1;

            ViewBag.Page = page;
            ViewBag.TotalPages = (int)Math.Ceiling(totalEntries / (double)pageSize);
            ViewBag.EntriesStart = entriesStart;
            ViewBag.EntriesEnd = entriesEnd;
            ViewBag.TotalEntries = totalEntries;

            return View(stores);
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
