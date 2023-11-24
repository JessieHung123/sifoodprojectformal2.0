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
using System.Security.Cryptography;
using System.Data.SqlTypes;
using System.Collections.Generic;


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
            User? account = _context.Users.FirstOrDefault(x => x.UserEmail == model.Account);

            if (account != null && account.UserAuthenticated == 1)
            {
                string passwordWithSalt = $"{model.Password}{account.UserPasswordSalt}";
                Byte[] RealPasswordBytes = Encoding.ASCII.GetBytes(passwordWithSalt);

                using (SHA256 sha256 = SHA256.Create())
                {
                    Byte[] RealPasswordHash = sha256.ComputeHash(RealPasswordBytes);

                    if (Enumerable.SequenceEqual(RealPasswordHash, account.UserPasswordHash))
                    {
                        List<Claim> claims = new List<Claim>()
                        {
                        new Claim(ClaimTypes.Name, $"{model.Account}"),
                        new Claim(ClaimTypes.Role, "User"),
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
        [Route("/Account/PostAccount")]
        public string PostAccount([FromForm] RegisterVM model)
        {
            IQueryable<string> AllAccount = _context.Users.Select(x => x.UserEmail);

            if (AllAccount.Contains(model.EmailAccount))
            {
                return "此帳號已被註冊";
            }
            else
            {
                byte[] saltBytes = new byte[8];
                using (RandomNumberGenerator ran = RandomNumberGenerator.Create())
                {
                    ran.GetBytes(saltBytes);
                }
                model.PasswordSalt = saltBytes;

                SHA256 sha256 = SHA256.Create();
                byte[] passwordBytes = Encoding.ASCII.GetBytes($"{model?.Password}{saltBytes}");
                byte[] hashBytes = sha256.ComputeHash(passwordBytes);

                Random UserVerification = new Random();

                User user = new User
                {
                    UserEmail = $"{model?.EmailAccount}",
                    UserPasswordSalt = model?.PasswordSalt,
                    UserPasswordHash = hashBytes,
                    UserVerificationCode = UserVerification.Next(100000, 999999).ToString(),
                };
                _context.Users.Add(user);
                _context.SaveChanges();

                SmtpClient client = new SmtpClient();
                client.Host = "smtp.gmail.com";
                client.Port = 587;
                client.EnableSsl = true;
                client.Credentials = new NetworkCredential("brad881112@gmail.com", "cttl rkeu vveh ojtv");

                MailMessage mail = new MailMessage();

                mail.Subject = "Sifood會員驗證信";
                mail.From = new MailAddress("brad881112@gmail.com", "Sifood官方帳號");
                mail.To.Add($"{model?.EmailAccount}");
                string MailHtmlBody = $"<div id=\"body\" class=\"sc-jlZhew fuwAaz body\" data-name=\"body\" data-draggable=\"false\" data-empty=\"false\" style=\"background-color: rgb(237, 237, 237);\">\r\n    <div id=\"nsDEeKWBv5AT25uLHham4W\" class=\"sc-jlZhew fuwAaz vertical-frame\" data-name=\"vertical-frame\" data-draggable=\"true\" data-empty=\"false\" style=\"align-self: center; padding: 60px 30px 100px; border-radius: 0px; background-color: rgb(26, 33, 32); margin-bottom: unset; --width: 680px;\">\r\n        <div id=\"nC73z7rDaZtb1DnPoZmesl\" class=\"sc-jlZhew fuwAaz vertical-frame\" data-name=\"vertical-frame\" data-draggable=\"true\" data-empty=\"false\" style=\"align-self: center; padding: 0px; border-radius: 0px; background-color: unset; --width: 475px;\">\r\n            <div id=\"nWyg-VBoKGfIpL9Xj0qwoq\" src=\"https://uploads.tabular.email/u/88f987f4-4b2f-49a3-bfd1-56a8c4319a80/51fea2b2-7c41-41de-86af-dd2b48532c62.png\" alt=\"\" class=\"sc-jlZhew fuwAaz image\" data-name=\"image\" data-draggable=\"true\" data-empty=\"false\" style=\"align-self: flex-start; text-align: left; margin-top: 0px; margin-bottom: 90px; padding: 0px; --width: 40px;\">\r\n                     </div>\r\n            <div id=\"nzk_5G7mxamMm4foEnLJHm\" class=\"sc-jlZhew fuwAaz heading-1\" data-name=\"heading-1\" data-draggable=\"true\" data-empty=\"false\" contenteditable=\"false\" spellcheck=\"false\" autocomplete=\"off\" autocorrect=\"off\" aria-multiline=\"true\" role=\"textbox\" style=\"align-self: center; margin-top: 0px; margin-bottom: 16px; font-family: &quot;Albert Sans&quot;; font-weight: 700; font-style: normal; color: rgb(255, 255, 255); font-size: 35px; letter-spacing: 0px; word-spacing: 0px; line-height: 39px; text-align: left; text-transform: none; text-decoration: none; direction: ltr; --width: 475px; padding-bottom: 0px;\">\r\n                <span id=\"nCayFSnnkGGRlSTMgTwG7k\" data-type=\"TEXT_NODE\" data-name=\"#text\" style=\"\">帳號驗證</span>\r\n            </div>\r\n            <div id=\"n8eO6sS8lPQIrh69cr9M49\" class=\"sc-jlZhew fuwAaz paragraph\" data-name=\"paragraph\" data-draggable=\"true\" data-empty=\"false\" contenteditable=\"false\" spellcheck=\"false\" autocomplete=\"off\" autocorrect=\"off\" aria-multiline=\"true\" role=\"textbox\" style=\"align-self: flex-start; margin-top: 0px; margin-bottom: 56px; font-family: &quot;Albert Sans&quot;; font-weight: 400; font-style: normal; color: rgb(255, 255, 255); font-size: 21px; letter-spacing: 0px; word-spacing: 0px; line-height: 32px; text-align: left; text-transform: none; text-decoration: none; direction: ltr; --width: 430px;\">\r\n                <span id=\"nkqntWTTVhUjDKRCzOLEKf\" data-type=\"TEXT_NODE\" data-name=\"#text\" style=\"\">為了驗證您的帳號, 請幫我在驗證畫面輸入以下驗證碼</span>\r\n            </div>\r\n            <div id=\"nCyOjg661zdZ7IUK7vV47T\" href=\"https://tabular.email\" class=\"sc-jlZhew fuwAaz button\" data-name=\"button\" data-draggable=\"true\" data-empty=\"false\" contenteditable=\"false\" spellcheck=\"false\" autocomplete=\"off\" autocorrect=\"off\" aria-multiline=\"true\" role=\"textbox\" style=\"align-self: flex-start; font-family: &quot;Inter Tight&quot;; font-weight: 800; font-style: normal; text-align: center; text-decoration: none; font-size: 14px; line-height: 48px; color: rgb(255, 255, 255); background-color: rgb(17, 152, 114); margin-top: 0px; margin-bottom: 0px; padding: 0px; letter-spacing: 0.65px; word-spacing: 0px; direction: ltr; border-radius: 4px; text-transform: uppercase; --width: 246px;\">\r\n                <span id=\"nONbX9vLZTyWRQAlCXgYjO\" data-type=\"TEXT_NODE\" data-name=\"#text\" style=\"\">您的驗證碼為 : {user.UserVerificationCode}</span>\r\n            </div>\r\n        </div>\r\n    </div>\r\n</div>";
                mail.Body = MailHtmlBody;
                mail.IsBodyHtml = true;
                mail.BodyEncoding = Encoding.UTF8;

                client.Send(mail);

                return "帳號註冊成功, 即將進入驗證階段";
            }
        }


        [HttpPost]
        [Route("/Account/OpenUserAccount")]
        public string OpenUserAccount(EmailVerificationVM model)
        {
            User? LockUser = _context.Users.FirstOrDefault(x => x.UserVerificationCode == model.UserAccountVerificationCode);
            if (LockUser != null)
            {
                LockUser.UserAuthenticated = 1;
            }
            _context.SaveChanges();
            return "驗證帳號成功，已為您開通帳號，請再次重新登入";
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        public IActionResult ResetPassword()
        {
            return View();
        }
        public IActionResult RegisterConfirmation()
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
