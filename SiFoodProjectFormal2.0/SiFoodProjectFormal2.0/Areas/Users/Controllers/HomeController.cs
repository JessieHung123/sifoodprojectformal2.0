using Microsoft.AspNetCore.Mvc;
using SiFoodProjectFormal2._0.Models;
using SiFoodProjectFormal2._0.ViewModels.Users;

namespace sifoodprojectformal2._0.Areas.Users.Controllers
{
    [Area("Users")]
    public class HomeController : Controller
    {
        Sifood3Context _context;

        public HomeController(Sifood3Context context)
        {
            _context = context;
        }

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

        [HttpPost]
        [ValidateAntiForgeryToken]

        //Address>now only one input
        public IActionResult JoinUs([Bind("StoreName, ContactName,TaxID,Email,Phone,Address,Description,OpeningTime,OpeningDay")]JoinUsViewModel joinUsViewModel)
        {
            if (ModelState.IsValid)
            {
                return RedirectToAction("JoinUs");
            }
           return View(joinUsViewModel);
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
