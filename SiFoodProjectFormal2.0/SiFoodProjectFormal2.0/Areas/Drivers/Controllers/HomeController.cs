using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SiFoodProjectFormal2._0.Models;

namespace sifoodprojectformal2._0.Areas.Drivers.Controllers
{
    [Area("Drivers")]
    public class HomeController : Controller
    {
        Sifood3Context _context;

        public HomeController(Sifood3Context context)
        {
            _context = context;
        }
        public IActionResult Login()
        {
            return View();
        }
        public IActionResult OrderList()
        {
            return View();
        }
        [Route("/Drivers/Home/ChooseOrder/{OrderId}")]
        public IActionResult ChooseOrder(string OrderId)
        {
            var orderdetail = _context.Orders.Where(o => o.StatusId == 2 && o.OrderId == OrderId);
            return View(orderdetail);
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
