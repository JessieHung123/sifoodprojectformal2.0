using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SiFoodProjectFormal2._0.ViewModels.Stores;
using System.Security.Claims;
using SiFoodProjectFormal2._0.Models;
using System.Text;
using System.Security.Cryptography;

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

        public IActionResult SetAccount()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginVM model)
        {
            Store? account = _context.Stores.FirstOrDefault(x => x.Email == model.Account);

            if (account != null)
            {
                string passwordWithSalt = $"{model.Password}{account.PasswordSalt}";
                Byte[] RealPasswordBytes = Encoding.ASCII.GetBytes(passwordWithSalt);

                using (SHA256 sha256 = SHA256.Create())
                {
                    Byte[] RealPasswordHash = sha256.ComputeHash(RealPasswordBytes);

                    if (Enumerable.SequenceEqual(RealPasswordHash, account.PasswordHash))
                    {
                        List<Claim> claims = new List<Claim>()
                        {
                        new Claim(ClaimTypes.Name, $"{model.Account}"),
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

    //    [HttpPost]
    //    [Route("Account/SetPassword")]
    //    public string SetPassword()
    //    {
    //        byte[] saltBytes = new byte[8];
    //        using (RandomNumberGenerator ran = RandomNumberGenerator.Create())
    //        {
    //            ran.GetBytes(saltBytes);
    //        }

    //        SHA256 sha256 = SHA256.Create();
    //        byte[] passwordBytes = Encoding.ASCII.GetBytes($"{model?.Password}{saltBytes}");
    //        byte[] hashBytes = sha256.ComputeHash(passwordBytes);

    //        Random UserVerification = new Random();

    //        User user = new User
    //        {
    //            UserPasswordSalt = saltBytes,
    //            UserPasswordHash = hashBytes,
    //            UserVerificationCode = UserVerification.Next(100000, 999999).ToString(),
    //        };
    //        _context.Users.Add(user);
    //        _context.SaveChanges();

    //        return "密碼設定成功";
    //    }
    //}
}
