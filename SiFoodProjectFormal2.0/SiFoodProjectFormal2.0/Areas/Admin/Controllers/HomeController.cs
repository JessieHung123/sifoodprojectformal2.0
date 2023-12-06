﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SiFoodProjectFormal2._0.Areas.Users.Models.ViewModels;
using SiFoodProjectFormal2._0.Models;

namespace sifoodprojectformal2._0.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class HomeController : Controller
    {
        private readonly Sifood3Context _context;
        public HomeController(Sifood3Context context) 
        {
            _context = context;
        }
        public IActionResult OrderManage()
        {
            return View();
        }
        public IActionResult MemberManage()
        {
            return View();
        }
        public IActionResult DriverManage()
        {
            return View();
        }
        public IActionResult StoreManage()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(LoginVM model)
        {
            var admin = _context.Admins.Where(x=>x.Account == model.Account).FirstOrDefault();
            if (admin != null && model.Password == admin.Password)
            {
                return RedirectToAction("Index", "OrderManagae");
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }
    }
}
