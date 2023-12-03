using Microsoft.AspNetCore.Mvc;

namespace sifoodprojectformal2._0.Areas.Drivers.Controllers
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
        
        public IActionResult ChooseOrder(string OrderId)
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
