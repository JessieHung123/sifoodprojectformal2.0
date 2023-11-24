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

        // GET: Admin/StoreManage
        public async Task<IActionResult> Index()
        {
              return _context.Stores != null ? 
                          View(await _context.Stores.ToListAsync()) :
                          Problem("Entity set 'Sifood3Context.Stores'  is null.");
        }

        // GET: Admin/StoreManage/Details/5
        public async Task<IActionResult> Details(string StoreId)
        {
            if (StoreId == null || _context.Stores == null)
            {
                return NotFound();
            }

            var store = await _context.Stores
                .FirstOrDefaultAsync(m => m.StoreId == StoreId);
            if (store == null)
            {
                return NotFound();
            }

            return View(store);
        }

        // GET: Admin/StoreManage/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/StoreManage/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("StoreId,StoreName,ContactName,TaxId,Email,Phone,City,Region,Address,Description,PasswordHash,PasswordSalt,EnrollDate,Latitude,Longitude,OpeningTime,OpeningDay,PhotosPath,LogoPath")] Store store)
        {
            if (ModelState.IsValid)
            {
                _context.Add(store);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(store);
        }

        // GET: Admin/StoreManage/Edit/5
        public async Task<IActionResult> Edit(string StoreId)
        {
            if (StoreId == null || _context.Stores == null)
            {
                return NotFound();
            }

            var store = await _context.Stores.FindAsync(StoreId);
            if (store == null)
            {
                return NotFound();
            }
            return View(store);
        }

        // POST: Admin/StoreManage/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string StoreId, [Bind("StoreId,StoreName,ContactName,TaxId,Email,Phone,City,Region,Address,Description,PasswordHash,PasswordSalt,EnrollDate,Latitude,Longitude,OpeningTime,OpeningDay,PhotosPath,LogoPath")] Store store)
        {
            if (StoreId != store.StoreId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(store);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StoreExists(store.StoreId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(store);
        }

        // GET: Admin/StoreManage/Delete/5
        public async Task<IActionResult> Delete(string StoreId)
        {
            if (StoreId == null || _context.Stores == null)
            {
                return NotFound();
            }

            var store = await _context.Stores
                .FirstOrDefaultAsync(m => m.StoreId == StoreId);
            if (store == null)
            {
                return NotFound();
            }

            return View(store);
        }

        // POST: Admin/StoreManage/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string StoreId)
        {
            if (_context.Stores == null)
            {
                return Problem("Entity set 'Sifood3Context.Stores'  is null.");
            }
            var store = await _context.Stores.FindAsync(StoreId);
            if (store != null)
            {
                _context.Stores.Remove(store);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool StoreExists(string StoreId)
        {
          return (_context.Stores?.Any(e => e.StoreId == StoreId)).GetValueOrDefault();
        }
    }
}
