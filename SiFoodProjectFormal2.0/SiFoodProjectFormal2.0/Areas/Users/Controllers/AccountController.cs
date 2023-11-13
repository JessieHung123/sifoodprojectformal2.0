using Microsoft.AspNetCore.Mvc;

namespace sifoodprojectformal.Areas.Users.Controllers
{
    [Area("Users")]
    public class AccountController : Controller
    {
        public IActionResult LoginRegister()
        {
            return View();
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
