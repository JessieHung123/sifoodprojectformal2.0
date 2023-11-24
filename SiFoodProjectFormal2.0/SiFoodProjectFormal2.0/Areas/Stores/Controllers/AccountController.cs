using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SiFoodProjectFormal2._0.ViewModels.Stores;
using System.Security.Claims;
using SiFoodProjectFormal2._0.Models;

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


        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginVM model)
        {
            string[]? account = _context.Stores.Select(x => x.Email).ToArray();
            byte[]?[] password = _context.Stores.Select(y => y.PasswordHash).ToArray();


            if (account.Contains(model.Account) && password.Contains(model.Password))
            {
                var claims = new List<Claim>()
                {
                new Claim(ClaimTypes.Name, $"{model.Account}"),
                new Claim(ClaimTypes.Role, "Stores"),
                };

                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                var principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                    principal, new AuthenticationProperties
                    {
                        ExpiresUtc = DateTime.UtcNow.AddDays(1)
                    });
                return RedirectToAction("Main", "Home");
            }
            else
            {
                return View();
            }
        }
    }
}
