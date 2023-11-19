using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SiFoodProjectFormal2._0.Models;
using SiFoodProjectFormal2.Models;

namespace sifoodprojectformal2._0.Areas.Users.Controllers
{
    [Area("Users")]
    public class TransactionController : Controller
    {
        private readonly Sifood3Context _context;
        public TransactionController(Sifood3Context context)
        {
            _context = context;
        }
        [HttpGet]

        public IActionResult Checkout(int id)
        {
            var _address = _context.UserAddresses.Find(id)?.UserDetailAddress;
            ViewBag.ad = _address;




            return View();
        }
        public IActionResult Payment()
        {
            return View();
        }
    }
}
