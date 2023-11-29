using Microsoft.AspNetCore.Mvc;
using SiFoodProjectFormal2._0.ViewModels.Users;

namespace sifoodprojectformal2._0.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class HomeController : Controller
    {
     
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
        public IActionResult Login()
        {
            return View();
        }

        public IActionResult Login(LoginVM model)
        {

            return RedirectToAction("", "");
        }
    }
}
