using Microsoft.AspNetCore.Mvc;

namespace sifoodprojectformal.Areas.DriversPlatform.Controllers
{
    [Area("Drivers")]
    public class HomeController : Controller
    {
        public IActionResult Login()
        {
            return View();
        }
        public IActionResult OrderList()
        {
            return View();
        }
        public IActionResult DeliveryOrder()
        {
            return View();
        }
        public IActionResult FinishOrder()
        {
            return View();
        }
    }
}
