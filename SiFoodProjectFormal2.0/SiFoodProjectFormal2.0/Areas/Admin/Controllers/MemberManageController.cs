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
    public class MemberManageController : Controller
    {
        private readonly Sifood3Context _context;

        public MemberManageController(Sifood3Context context)
        {
            _context = context;
        }

        // GET: Admin/MemberManage
        public async Task<IActionResult> Index()
        {
            //return View(await _context.Users.ToListAsync());

            return _context.Users != null ? 
            View(await _context.Users.ToListAsync()) :
            Problem("Entity set 'Sifood3Context.Users'  is null.");
        }

        // GET: Admin/MemberManage/Details/5
        public IActionResult Details1(string UserId)
        {
            User user = _context.Users.Where(x => x.UserId == UserId).FirstOrDefault();
            return PartialView("_Details", user);
        }
        public async Task<IActionResult> Details(string UserId)
        {
            if (UserId == null || _context.Users == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(m => m.UserId == UserId);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // GET: Admin/MemberManage/Create

        //public IActionResult Create()
        //{
        //    User user = new User();
        //    return PartialView("Create", user);
        //}
        //public IActionResult Create()
        //{
        //    return View();
        //}

        // POST: Admin/MemberManage/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create([Bind("UserId,UserName,UserEmail,UserPasswordHash,UserPhone,UserBirthDate,TotalOrderAmount,UserEnrollDate,UserLatestLogInDate,UserPasswordSalt")] User user)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _context.Add(user);
        //        await _context.SaveChangesAsync();
        //        return RedirectToAction(nameof(Index));
        //    }
        //    return View(user);
        //}

        // GET: Admin/MemberManage/Edit/5


        public IActionResult Edit1(string UserId)
        {
            User user = _context.Users.Where(x => x.UserId == UserId).FirstOrDefault();
            return PartialView("_Edit", user);
        }
        
        public async Task<IActionResult> Edit(string UserId)
        {
            if (UserId == null || _context.Users == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FindAsync(UserId);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        // POST: Admin/MemberManage/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.

        [HttpPost]
        public IActionResult Edit1(User user)
        {
            _context.Users.Update(user);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string UserId, [Bind("UserId,UserName,UserEmail,UserPasswordHash,UserPhone,UserBirthDate,TotalOrderAmount,UserEnrollDate,UserLatestLogInDate,UserPasswordSalt")] User user)
        {
            if (UserId != user.UserId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(user);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.UserId))
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
            return View(user);
        }

        // GET: Admin/MemberManage/Delete/5

        public IActionResult Delete1(string UserId)
        {
            User user = _context.Users.Where(x => x.UserId == UserId).FirstOrDefault();
            return PartialView("_Delete", user);
        }

        [HttpPost]
        public IActionResult Delete1(User user)
        {
            _context.Users.Remove(user);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Delete(string UserId)
        {
            if (UserId == null || _context.Users == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(m => m.UserId == UserId);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: Admin/MemberManage/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string UserId)
        {
            if (_context.Users == null)
            {
                return Problem("Entity set 'Sifood3Context.Users'  is null.");
            }
            var user = await _context.Users.FindAsync(UserId);
            if (user != null)
            {
                _context.Users.Remove(user);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserExists(string id)
        {
          return (_context.Users?.Any(e => e.UserId == id)).GetValueOrDefault();
        }
    }
}
