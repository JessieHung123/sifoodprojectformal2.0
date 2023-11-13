using Microsoft.AspNetCore.Mvc;

namespace sifoodprojectformal.Areas.Admin.Controllers
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
    }
}
