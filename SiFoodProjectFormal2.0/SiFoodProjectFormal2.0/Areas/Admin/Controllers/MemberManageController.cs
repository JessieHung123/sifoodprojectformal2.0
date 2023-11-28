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

        public async Task<IActionResult> Index(string searchString)
        {
            IQueryable<User> users = _context.Users;
            if (!string.IsNullOrEmpty(searchString))
            {
                users = users.Where(u => u.UserName.Contains(searchString) || u.UserEmail.Contains(searchString) || u.UserPhone.Contains(searchString));
            }
            return View(await users.ToListAsync());
        }

        public IActionResult Details1(string UserId)
        {
            User user = _context.Users.Where(x => x.UserId == UserId).FirstOrDefault();
            return PartialView("_Details", user);
        }
        
        public IActionResult Edit1(string UserId)
        {
            User user = _context.Users.Where(x => x.UserId == UserId).FirstOrDefault();
            return PartialView("_Edit", user);
        }
        
        [HttpPost]       
        public IActionResult Edit1(User user)
        {
            _context.Users.Update(user);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

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
    }
}
