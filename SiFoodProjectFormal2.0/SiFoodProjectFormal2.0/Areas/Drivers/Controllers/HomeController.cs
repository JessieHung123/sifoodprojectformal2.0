using Microsoft.AspNetCore.Mvc;

namespace sifoodprojectformal2._0.Areas.Drivers.Controllers
{
    [Area("Drivers")]
    public class HomeController : Controller
    {
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
