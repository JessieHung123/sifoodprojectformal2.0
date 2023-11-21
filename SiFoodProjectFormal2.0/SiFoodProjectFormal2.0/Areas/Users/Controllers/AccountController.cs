using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using SiFoodProjectFormal2._0.ViewModels.Users;
using System.Security.Claims;
using SiFoodProjectFormal2._0.Models;

namespace sifoodprojectformal2._0.Areas.Users.Controllers
{
    [Area("Users")]
    public class AccountController : Controller
    {
        private readonly Sifood3Context _context;

        public AccountController(Sifood3Context context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult LoginRegister()
        {
            return View();
        }

        [HttpPost]
        
        public async Task<IActionResult> LoginRegister(LoginVM model)
        {
            string? account = _context.Users.Select(x => x.UserEmail).FirstOrDefault();
            byte[]? password = _context.Users.Select(y => y.UserPasswordHash).FirstOrDefault();


            if (model.Account == account && model.Password == password)
            {
                var claims = new List<Claim>()
                {
                new Claim(ClaimTypes.Name, $"{model.Account}"),
                new Claim(ClaimTypes.Role, "User"),
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

        [HttpPost]
        [Route("/Account/PostAccount")]
        public string PostAccount([FromForm] RegisterVM model)
        {
            User user = new User
            {
                UserId = model?.UserId,
                UserEmail = model?.EmailAccount,
                UserPasswordHash = model.Password
            };

            _context.Users.Add(user);
            _context.SaveChanges();

            return "帳號註冊成功";

        }


        public IActionResult ResetPassword()
        {
            return View();
        }
        public IActionResult RegisterConfirmation()
        {
            return View();

        }
        public IActionResult FinishConfirm()
        {
            return View();
        }
        public IActionResult ForgotPassword()
        {
            return View();
        }
        public IActionResult ForgotPasswordConfirmation()
        {
            return View();
        }
        public IActionResult ResetPasswordConfirmation()
        {
            return View();
        }
    }
}
