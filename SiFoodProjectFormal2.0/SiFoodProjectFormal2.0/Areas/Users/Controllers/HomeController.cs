using Microsoft.AspNetCore.Mvc;

namespace sifoodprojectformal.Areas.UsersPlateform.Controllers
{
    [Area("Users")]
    public class HomeController : Controller
    {
        public IActionResult Main()
        {
            return View();
        }
        public IActionResult MapFind()
        {
            return View();
        }
        public IActionResult FAQ()
        {
            return View();

        }
        public IActionResult MemberShip()
        {
            return View();
        }
        public IActionResult Stores()
        {
            return View();
        }
        public IActionResult JoinUs()
        {
            return View();
        }
        public IActionResult Products()
        {
            return View();
        }
        public IActionResult RealTimeOrders()
        {
            return View();
        }
    }
}
