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
using SiFoodProjectFormal2._0.Areas.Users.Models.ViewModels;


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
            User? EmailAccount = _context.Users.FirstOrDefault(x => x.UserEmail == model.Account);

            if (EmailAccount != null && EmailAccount.UserAuthenticated == 1)
            {
                string PasswordWithSalt = $"{model.Password}{EmailAccount.UserPasswordSalt}";
                Byte[] RealPasswordBytes = Encoding.ASCII.GetBytes(PasswordWithSalt);

                using (SHA256 Sha256 = SHA256.Create())
                {
                    Byte[] RealPasswordHash = Sha256.ComputeHash(RealPasswordBytes);

                    if (Enumerable.SequenceEqual(RealPasswordHash, EmailAccount.UserPasswordHash))
                    {
                        List<Claim> claims = new List<Claim>()
                        {
                        new Claim(ClaimTypes.Name, $"{EmailAccount.UserId}"),
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

                SHA256 sha256 = SHA256.Create();
                byte[] passwordBytes = Encoding.ASCII.GetBytes($"{model?.Password}{saltBytes}");
                byte[] hashBytes = sha256.ComputeHash(passwordBytes);

                Random UserVerification = new Random();

                User user = new User
                {
                    UserEmail = $"{model?.EmailAccount}",
                    UserPasswordSalt = saltBytes,
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
        public string OpenUserAccount([FromBody] EmailVerificationVM model)
        {
            User? LockUser = _context.Users.FirstOrDefault(x => x.UserVerificationCode == model.UserAccountVerificationCode);
            if (LockUser != null)
            {
                LockUser.UserAuthenticated = 1;
                _context.SaveChanges();
                return "驗證帳號成功，已為您開通帳號，請再次重新登入";
            }
            else
            {
                return "驗證帳號失敗，請重新輸入驗證碼";
            }
        }

        [HttpPost]
        [Route("/Account/ForgotPasswordSendEmail")]
        public string ForgotPasswordSendEmail([FromBody] LoginVM model)
        {
            User? user = _context.Users.FirstOrDefault(x => x.UserEmail == model.Account);
            if (user == null)
            {
                return "找不到此使用者";
            }
            else
            {
                Random UserVerification = new Random();
                user.ForgotPasswordRandom = UserVerification.Next(100000, 999999).ToString();
                _context.SaveChanges();

                SmtpClient client = new SmtpClient();
                client.Host = "smtp.gmail.com";
                client.Port = 587;
                client.EnableSsl = true;
                client.Credentials = new NetworkCredential("brad881112@gmail.com", "cttl rkeu vveh ojtv");

                MailMessage mail = new MailMessage();

                mail.Subject = "Sifood會員驗證信";
                mail.From = new MailAddress("brad881112@gmail.com", "Sifood官方帳號");
                mail.To.Add($"{model.Account}");
                string MailHtmlBody = $"<div id=\"body\" class=\"sc-jlZhew fuwAaz body\" data-name=\"body\" data-draggable=\"false\" data-empty=\"false\" style=\"background-color: rgb(237, 237, 237);\">\r\n    <div id=\"nsDEeKWBv5AT25uLHham4W\" class=\"sc-jlZhew fuwAaz vertical-frame\" data-name=\"vertical-frame\" data-draggable=\"true\" data-empty=\"false\" style=\"align-self: center; padding: 60px 30px 100px; border-radius: 0px; background-color: rgb(26, 33, 32); margin-bottom: unset; --width: 680px;\">\r\n        <div id=\"nC73z7rDaZtb1DnPoZmesl\" class=\"sc-jlZhew fuwAaz vertical-frame\" data-name=\"vertical-frame\" data-draggable=\"true\" data-empty=\"false\" style=\"align-self: center; padding: 0px; border-radius: 0px; background-color: unset; --width: 475px;\">\r\n            <div id=\"nWyg-VBoKGfIpL9Xj0qwoq\" src=\"https://uploads.tabular.email/u/88f987f4-4b2f-49a3-bfd1-56a8c4319a80/51fea2b2-7c41-41de-86af-dd2b48532c62.png\" alt=\"\" class=\"sc-jlZhew fuwAaz image\" data-name=\"image\" data-draggable=\"true\" data-empty=\"false\" style=\"align-self: flex-start; text-align: left; margin-top: 0px; margin-bottom: 90px; padding: 0px; --width: 40px;\">\r\n                     </div>\r\n            <div id=\"nzk_5G7mxamMm4foEnLJHm\" class=\"sc-jlZhew fuwAaz heading-1\" data-name=\"heading-1\" data-draggable=\"true\" data-empty=\"false\" contenteditable=\"false\" spellcheck=\"false\" autocomplete=\"off\" autocorrect=\"off\" aria-multiline=\"true\" role=\"textbox\" style=\"align-self: center; margin-top: 0px; margin-bottom: 16px; font-family: &quot;Albert Sans&quot;; font-weight: 700; font-style: normal; color: rgb(255, 255, 255); font-size: 35px; letter-spacing: 0px; word-spacing: 0px; line-height: 39px; text-align: left; text-transform: none; text-decoration: none; direction: ltr; --width: 475px; padding-bottom: 0px;\">\r\n                <span id=\"nCayFSnnkGGRlSTMgTwG7k\" data-type=\"TEXT_NODE\" data-name=\"#text\" style=\"\">帳號驗證</span>\r\n            </div>\r\n            <div id=\"n8eO6sS8lPQIrh69cr9M49\" class=\"sc-jlZhew fuwAaz paragraph\" data-name=\"paragraph\" data-draggable=\"true\" data-empty=\"false\" contenteditable=\"false\" spellcheck=\"false\" autocomplete=\"off\" autocorrect=\"off\" aria-multiline=\"true\" role=\"textbox\" style=\"align-self: flex-start; margin-top: 0px; margin-bottom: 56px; font-family: &quot;Albert Sans&quot;; font-weight: 400; font-style: normal; color: rgb(255, 255, 255); font-size: 21px; letter-spacing: 0px; word-spacing: 0px; line-height: 32px; text-align: left; text-transform: none; text-decoration: none; direction: ltr; --width: 430px;\">\r\n                <span id=\"nkqntWTTVhUjDKRCzOLEKf\" data-type=\"TEXT_NODE\" data-name=\"#text\" style=\"\">為了驗證您的帳號, 請幫我在驗證畫面輸入以下驗證碼</span>\r\n            </div>\r\n            <div id=\"nCyOjg661zdZ7IUK7vV47T\" href=\"https://tabular.email\" class=\"sc-jlZhew fuwAaz button\" data-name=\"button\" data-draggable=\"true\" data-empty=\"false\" contenteditable=\"false\" spellcheck=\"false\" autocomplete=\"off\" autocorrect=\"off\" aria-multiline=\"true\" role=\"textbox\" style=\"align-self: flex-start; font-family: &quot;Inter Tight&quot;; font-weight: 800; font-style: normal; text-align: center; text-decoration: none; font-size: 14px; line-height: 48px; color: rgb(255, 255, 255); background-color: rgb(17, 152, 114); margin-top: 0px; margin-bottom: 0px; padding: 0px; letter-spacing: 0.65px; word-spacing: 0px; direction: ltr; border-radius: 4px; text-transform: uppercase; --width: 246px;\">\r\n                <span id=\"nONbX9vLZTyWRQAlCXgYjO\" data-type=\"TEXT_NODE\" data-name=\"#text\" style=\"\">您的驗證碼為 : {user.ForgotPasswordRandom}</span>\r\n            </div>\r\n        </div>\r\n    </div>\r\n</div>";
                mail.Body = MailHtmlBody;
                mail.IsBodyHtml = true;
                mail.BodyEncoding = Encoding.UTF8;

                client.Send(mail);
                return "驗證碼寄送成功";
            }
        }

        [HttpPost]
        [Route("/Account/ConfirmForgotPasswordRandom")]
        public string ConfirmForgotPasswordRandom([FromBody] EmailVerificationVM model)
        {
            User? user = _context.Users.FirstOrDefault(x => x.ForgotPasswordRandom == model.UserAccountVerificationCode);
            if (user == null)
            {
                return "驗證碼核對失敗";
            }
            else
            {
                return "驗證碼核對成功";
            }
        }

        [HttpPost]
        [Route("/Account/ResetPassword")]

        public string ResetPassword([FromForm] ResetPasswordVM model)
        {
            User? user = _context.Users.FirstOrDefault(x => x.UserEmail == model.UserConfirmEmail);

            if (user != null)
            {
                byte[] NewSaltBytes = new byte[8];
                using (RandomNumberGenerator ran = RandomNumberGenerator.Create())
                {
                    ran.GetBytes(NewSaltBytes);
                }
                user.UserPasswordSalt = NewSaltBytes;
                SHA256 Sha256 = SHA256.Create();
                byte[] PasswordBytes = Encoding.ASCII.GetBytes($"{model?.NewPassword}{NewSaltBytes}");
                byte[] NewHashBytes = Sha256.ComputeHash(PasswordBytes);
                user.UserPasswordHash = NewHashBytes;
                _context.SaveChanges();
                return "密碼重設成功, 即將回到登入畫面";
            }
            else
            {
                return "使用者核對失敗";
            }
        }

        [HttpGet]
        [Route("/Account/Logout")]
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
    }
}