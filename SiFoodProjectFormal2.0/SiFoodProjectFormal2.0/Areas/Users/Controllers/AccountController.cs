using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using SiFoodProjectFormal2._0.Models;
using System.Text;
using SiFoodProjectFormal2._0.ViewModels.Users;
using Microsoft.Win32;

using NuGet.Packaging;
using System.Net.Mail;
using System.Net;


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
            var account = _context.Users.Where(x => x.UserEmail == model.Account).FirstOrDefault();
            var RealPassword = Encoding.ASCII.GetBytes($"{model?.Password}");
            var passwordHash = _context.Users.Where(x => x.UserPasswordHash == RealPassword).FirstOrDefault();

            if (account != null && passwordHash != null)
            {
                var claims = new List<Claim>()
                {
                new Claim(ClaimTypes.Name, $"{model?.Account}"),
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
            var AllAccount = _context.Users.Select(x => x.UserEmail);

            if (AllAccount.Contains(model.EmailAccount))
            {
                return "此帳號已被註冊";
            }
            else
            {
                User user = new User
                {
                    UserEmail = model?.EmailAccount,
                    UserPasswordHash = Encoding.ASCII.GetBytes($"{model?.Password}")
                };

                _context.Users.Add(user);
                _context.SaveChanges();

                var client = new SmtpClient();
                client.Host = "smtp.gmail.com";
                client.Port = 587; 
                client.EnableSsl = true;
                client.Credentials = new NetworkCredential("brad881112@gmail.com", "cttl rkeu vveh ojtv");

                var mail = new MailMessage();

                mail.Subject = "Sifood會員驗證信";
                mail.From = new MailAddress("brad881112@gmail.com", "Sifood官方帳號");
                mail.To.Add($"{model?.EmailAccount}");
                mail.Body = "<h1>請點選以下連結進行驗證</h1>";
                mail.IsBodyHtml = true;
                mail.BodyEncoding = Encoding.UTF8;

                client.Send(mail);

                return "帳號註冊成功, 即將進入驗證階段";
            }
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
