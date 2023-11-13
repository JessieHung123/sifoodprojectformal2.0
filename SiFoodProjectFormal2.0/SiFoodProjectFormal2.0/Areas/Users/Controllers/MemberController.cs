using Microsoft.AspNetCore.Mvc;

namespace sifoodprojectformal.Areas.Users.Controllers
{
    [Area("Users")]
    public class MemberController : Controller
    {
        public IActionResult Profile()
        {
            return View();
        }
        public IActionResult Favorite()
        {
            return View();
        }
        public IActionResult HistoryOrders()
        {
            return View();
        }
        public IActionResult Address()
        {
            return View();
        }
    }
}
