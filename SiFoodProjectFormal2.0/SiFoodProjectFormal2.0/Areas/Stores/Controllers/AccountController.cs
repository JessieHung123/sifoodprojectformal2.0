﻿using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using SiFoodProjectFormal2._0.Models;
using System.Text;
using System.Security.Cryptography;
using SiFoodProjectFormal2._0.Areas.Stores.ViewModels;

namespace sifoodprojectformal2._0.Areas.Stores.Controllers
{
    [Area("Stores")]
    public class AccountController : Controller
    {
        private readonly Sifood3Context _context;

        public AccountController(Sifood3Context context)
        {
            _context = context;
        }
        [Route("Account/Login")]
        public IActionResult Login()
        {
            return View();
        }
        [Route("Account/SetAccount")]
        public IActionResult SetAccount()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(StoreLoginVM model)
        {
            Store? account = _context.Stores.FirstOrDefault(x => x.Email == model.StoreAccount);
            if (account != null)
            {
                string passwordWithSalt = $"{model.SetPassword}{account.PasswordSalt}";
                Byte[] RealPasswordBytes = Encoding.ASCII.GetBytes(passwordWithSalt);
                using (SHA256 sha256 = SHA256.Create())
                {
                    Byte[] RealPasswordHash = sha256.ComputeHash(RealPasswordBytes);
                    if (Enumerable.SequenceEqual(RealPasswordHash, account.PasswordHash))
                    {
                        List<Claim> claims = new List<Claim>()
                        {
                        new Claim(ClaimTypes.Name, $"{account.StoreId}"),
                        new Claim(ClaimTypes.Role, "Store"),
                        };
                        ClaimsIdentity identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                        ClaimsPrincipal principal = new ClaimsPrincipal(identity);
                        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                            principal, new AuthenticationProperties
                            {
                                ExpiresUtc = DateTime.UtcNow.AddDays(1)
                            });
                        return RedirectToAction("Main", "Home");
                    }
                }
            }
            return View();
        }

        [HttpPost]
        [Route("Account/SetPassword")]
        public string SetPassword([FromForm] StoreLoginVM model)
        {
            Store? account = _context.Stores.FirstOrDefault(x => x.Email == model.StoreAccount);
            if (account == null)
            {
                return "找不到此商家";
            }
            else
            {
                byte[] saltBytes = new byte[8];
                using (RandomNumberGenerator ran = RandomNumberGenerator.Create())
                {
                    ran.GetBytes(saltBytes);
                }
                SHA256 sha256 = SHA256.Create();
                byte[] passwordBytes = Encoding.ASCII.GetBytes($"{model?.SetPassword}{saltBytes}");
                byte[] hashBytes = sha256.ComputeHash(passwordBytes);
                account.PasswordSalt = saltBytes;
                account.PasswordHash = hashBytes;
                _context.SaveChanges();
                return "密碼設定成功，請重新登入";
            }
        }

        [HttpGet]
        [Route("/Account/StroeLogout")]
        public async Task<IActionResult> StoreLogout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Account", "StoreLogout");
        }
    }
}
