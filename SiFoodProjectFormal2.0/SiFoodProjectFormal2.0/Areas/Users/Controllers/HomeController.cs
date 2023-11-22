using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using SiFoodProjectFormal2._0.Models;

namespace sifoodprojectformal2._0.Areas.Users.Controllers
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
            //ViewBag.ProductName=
            //    ViewBag.StoreName =
            //    ViewBag.Address =
            //    ViewBag.Description =
            //    ViewBag.UnitPrice =
            //    ViewBag.PhotoPath =
            //    ViewBag.SuggestPickUpTime =
            //    ViewBag.SuggestPickEndTime =
            //    ViewBag.RemainingStock=
            return View();
        }

        
        public IActionResult RealTimeOrders()
        {
            return View();
        }
    }
}
