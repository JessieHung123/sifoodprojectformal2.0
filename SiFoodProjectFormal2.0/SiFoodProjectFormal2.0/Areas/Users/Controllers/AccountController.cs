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
            var account = _context.Users.FirstOrDefault(x => x.UserEmail == model.Account);

            if (account != null && account.UserAuthenticated == 1)
            {
                string passwordWithSalt = $"{model.Password}{account.UserPasswordSalt}";
                var RealPasswordBytes = Encoding.ASCII.GetBytes(passwordWithSalt);

                using (var sha256 = SHA256.Create())
                {
                    var RealPasswordHash = sha256.ComputeHash(RealPasswordBytes);

                    if (Enumerable.SequenceEqual(RealPasswordHash, account.UserPasswordHash))
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
                }
            }
            return View();
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
                byte[] saltBytes = new byte[8];
                using (var ran = RandomNumberGenerator.Create())
                {
                    ran.GetBytes(saltBytes);
                }
                model.PasswordSalt = saltBytes;

                var sha256 = SHA256.Create();
                byte[] passwordBytes = Encoding.ASCII.GetBytes($"{model?.Password}{saltBytes}");
                byte[] hashBytes = sha256.ComputeHash(passwordBytes);

                var UserVerification = new Random();

                User user = new User
                {
                    UserEmail = $"{model?.EmailAccount}",
                    UserPasswordSalt = model?.PasswordSalt,
                    UserPasswordHash = hashBytes,
                    UserVerificationCode = UserVerification.ToString(),
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
                mail.Body = "<div id=\"body\" class=\"sc-jlZhew fuwAaz body\" data-name=\"body\" data-draggable=\"false\" data-empty=\"false\"\r\n    style=\"background-color: rgb(237, 237, 237);\">\r\n    <div id=\"nsDEeKWBv5AT25uLHham4W\" class=\"sc-jlZhew fuwAaz vertical-frame\" data-name=\"vertical-frame\"\r\n        data-draggable=\"true\" data-empty=\"false\"\r\n        style=\"align-self: center; padding: 60px 30px 100px; border-radius: 0px; background-color: rgb(26, 33, 32); margin-bottom: unset; --width: 680px;\">\r\n        <div id=\"nC73z7rDaZtb1DnPoZmesl\" class=\"sc-jlZhew fuwAaz vertical-frame\" data-name=\"vertical-frame\"\r\n            data-draggable=\"true\" data-empty=\"false\"\r\n            style=\"align-self: center; padding: 0px; border-radius: 0px; background-color: unset; --width: 475px;\">\r\n            <div id=\"nWyg-VBoKGfIpL9Xj0qwoq\"\r\n                src=\"https://uploads.tabular.email/u/88f987f4-4b2f-49a3-bfd1-56a8c4319a80/51fea2b2-7c41-41de-86af-dd2b48532c62.png\"\r\n                alt=\"\" class=\"sc-jlZhew fuwAaz image\" data-name=\"image\" data-draggable=\"true\" data-empty=\"false\"\r\n                style=\"align-self: flex-start; text-align: left; margin-top: 0px; margin-bottom: 90px; padding: 0px; --width: 40px;\">\r\n                <img src=\"https://uploads.tabular.email/u/88f987f4-4b2f-49a3-bfd1-56a8c4319a80/51fea2b2-7c41-41de-86af-dd2b48532c62.png\"\r\n                    data-name=\"#image\" draggable=\"false\">\r\n            </div>\r\n            <div id=\"nzk_5G7mxamMm4foEnLJHm\" class=\"sc-jlZhew fuwAaz heading-1\" data-name=\"heading-1\"\r\n                data-draggable=\"true\" data-empty=\"false\" contenteditable=\"false\" spellcheck=\"false\" autocomplete=\"off\"\r\n                autocorrect=\"off\" aria-multiline=\"true\" role=\"textbox\"\r\n                style=\"align-self: center; margin-top: 0px; margin-bottom: 16px; font-family: &quot;Albert Sans&quot;; font-weight: 700; font-style: normal; color: rgb(255, 255, 255); font-size: 35px; letter-spacing: 0px; word-spacing: 0px; line-height: 39px; text-align: left; text-transform: none; text-decoration: none; direction: ltr; --width: 475px; padding-bottom: 0px;\">\r\n                <span id=\"nCayFSnnkGGRlSTMgTwG7k\" data-type=\"TEXT_NODE\" data-name=\"#text\" style=\"\">帳號驗證</span>\r\n            </div>\r\n            <div id=\"n8eO6sS8lPQIrh69cr9M49\" class=\"sc-jlZhew fuwAaz paragraph\" data-name=\"paragraph\"\r\n                data-draggable=\"true\" data-empty=\"false\" contenteditable=\"false\" spellcheck=\"false\" autocomplete=\"off\"\r\n                autocorrect=\"off\" aria-multiline=\"true\" role=\"textbox\"\r\n                style=\"align-self: flex-start; margin-top: 0px; margin-bottom: 56px; font-family: &quot;Albert Sans&quot;; font-weight: 400; font-style: normal; color: rgb(255, 255, 255); font-size: 21px; letter-spacing: 0px; word-spacing: 0px; line-height: 32px; text-align: left; text-transform: none; text-decoration: none; direction: ltr; --width: 430px;\">\r\n                <span id=\"nkqntWTTVhUjDKRCzOLEKf\" data-type=\"TEXT_NODE\" data-name=\"#text\" style=\"\">為了驗證您的帳號,\r\n                    請幫我點選以下按鈕進行驗證</span>\r\n            </div>\r\n            <div id=\"nCyOjg661zdZ7IUK7vV47T\" href=\"https://tabular.email\" class=\"sc-jlZhew fuwAaz button\"\r\n                data-name=\"button\" data-draggable=\"true\" data-empty=\"false\" contenteditable=\"false\" spellcheck=\"false\"\r\n                autocomplete=\"off\" autocorrect=\"off\" aria-multiline=\"true\" role=\"textbox\"\r\n                style=\"align-self: flex-start; font-family: &quot;Inter Tight&quot;; font-weight: 800; font-style: normal; text-align: center; text-decoration: none; font-size: 14px; line-height: 48px; color: rgb(255, 255, 255); background-color: rgb(17, 152, 114); margin-top: 0px; margin-bottom: 0px; padding: 0px; letter-spacing: 0.65px; word-spacing: 0px; direction: ltr; border-radius: 4px; text-transform: uppercase; --width: 246px;\">\r\n                <button v-on:click=\"sendVerificationBack\" id=\"nONbX9vLZTyWRQAlCXgYjO\" data-type=\"TEXT_NODE\" data-name=\"#text\" style=\"\">驗證帳戶</button>\r\n            </div>\r\n        </div>\r\n    </div>\r\n    <div id=\"nrOP8IFI6-rIFGshlYHblw\" class=\"sc-jlZhew fuwAaz vertical-frame\" data-name=\"vertical-frame\"\r\n        data-draggable=\"true\" data-empty=\"false\"\r\n        style=\"align-self: center; padding: 80px 30px; border-radius: 0px; background-color: rgb(17, 152, 114); --width: 680px;\">\r\n        <div id=\"n0MgbgMYT8yG2Yeyhy2Mw7\" class=\"sc-jlZhew fuwAaz vertical-frame\" data-name=\"vertical-frame\"\r\n            data-draggable=\"true\" data-empty=\"false\"\r\n            style=\"align-self: center; padding: 0px; border-radius: 0px; background-color: unset; --width: 475px;\">\r\n            <div id=\"nyD8QQS-BO_AkNPgLSDm8f\" class=\"sc-jlZhew fuwAaz paragraph vTSH3FbiZV4WD2FpIDXcO4_Footer\"\r\n                data-name=\"paragraph\" data-draggable=\"true\" data-empty=\"false\" contenteditable=\"false\"\r\n                spellcheck=\"false\" autocomplete=\"off\" autocorrect=\"off\" aria-multiline=\"true\" role=\"textbox\"\r\n                style=\"align-self: center; margin-top: 0px; margin-bottom: 20px; font-family: &quot;Albert Sans&quot;; font-weight: 400; font-style: normal; color: rgb(255, 255, 255); font-size: 14px; letter-spacing: 0px; word-spacing: 0px; line-height: 22px; text-align: left; text-transform: none; text-decoration: none; direction: ltr; --width: 600px;\">\r\n                <span id=\"nYuZthpR5gKsOCTD34PATY\" data-type=\"TEXT_NODE\" data-name=\"#text\" style=\"\"></span>\r\n            </div>\r\n            <div id=\"nwnSPa4KZh5THE03WU_LvI\" class=\"sc-jlZhew fuwAaz paragraph vTSH3FbiZV4WD2FpIDXcO4_Footer\"\r\n                data-name=\"paragraph\" data-draggable=\"true\" data-empty=\"false\" contenteditable=\"false\"\r\n                spellcheck=\"false\" autocomplete=\"off\" autocorrect=\"off\" aria-multiline=\"true\" role=\"textbox\"\r\n                style=\"align-self: center; margin-top: 0px; margin-bottom: 0px; font-family: &quot;Albert Sans&quot;; font-weight: 400; font-style: normal; color: rgb(255, 255, 255); font-size: 14px; letter-spacing: 0px; word-spacing: 0px; line-height: 22px; text-align: left; text-transform: none; text-decoration: none; direction: ltr; --width: 600px;\">\r\n                <span id=\"ngU5YAoAe9kczK68gcZ3ou\" data-type=\"TEXT_NODE\" data-name=\"#text\" style=\"\"> © 2022~2023 All\r\n                    rights reserved. Powered by THM103.</span>\r\n            </div>\r\n        </div>\r\n    </div>\r\n</div>\r\n\r\n\r\n<script src=\"https://unpkg.com/vue@3.2.36/dist/vue.global.js\"></script>\r\n<script src=\"https://cdnjs.cloudflare.com/ajax/libs/axios/1.6.0/axios.min.js\"\r\n    integrity=\"sha512-WrdC3CE9vf1nBf58JHepuWT4x24uTacky9fuzw2g/3L9JkihgwZ6Cfv+JGTtNyosOhEmttMtEZ6H3qJWfI7gIQ==\"\r\n    crossorigin=\"anonymous\" referrerpolicy=\"no-referrer\"></script>\r\n\r\n<script>\r\n    var RegisterApp = Vue.createApp({\r\n        data: function () {\r\n            return {\r\n                vueSendVerificationBack: $\"{model.UserVerificationCode}\"\r\n            }\r\n        },\r\n        methods: {\r\n            sendVerificationBack(){\r\n                _this = this;\r\n                var form = new FormData;\r\n                form.append(\"UserAccountVerificationCode\", _this.vueSendVerificationBack)\r\n                axios.post(`/Account/OpenUserAccount`, form, {\r\n                    headers:{\r\n                        \"Content-Type\": \"multipart/form-data\"\r\n                    }\r\n                }).then(response => {\r\n                    alert(response.data);\r\n                    var success = \"驗證帳號成功，已為您開通帳號，請再次重新登入\";\r\n                    if(response.data == success){\r\n                        window.location.href = 'https://localhost:7042/Users/Account/LoginRegister';\r\n                    }\r\n                }).catch(err=>{\r\n                    alert(err);\r\n                });\r\n            },\r\n        },\r\n    }).mount(\"#body\")\r\n</script>";
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
            var LockUser = _context.Users.FirstOrDefault(x => x.UserVerificationCode == model.UserAccountVerificationCode);
            if (LockUser != null)
            {
                LockUser.UserAuthenticated = 1;
            }
            return "驗證帳號成功，已為您開通帳號，請再次重新登入";
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
