using Microsoft.AspNetCore.Mvc;

namespace sifoodprojectformal2._0.Areas.Stores.Controllers
{
    [Area("Stores")]
    public class HomeController : Controller
    {
        public IActionResult Main()
        {
            return View();
        }
        public IActionResult RealTimeOrders()
        {
            return View();
        }
        public IActionResult History()
        {
            return View();
        }
        public IActionResult ProductManage()
        {
            return View();
        }
        public IActionResult InfoModify()
        {
            return View();
        }
        public IActionResult Review()
        {
            return View();
        }
        public IActionResult FAQ()
        {
            return View();
        }
    }
}
