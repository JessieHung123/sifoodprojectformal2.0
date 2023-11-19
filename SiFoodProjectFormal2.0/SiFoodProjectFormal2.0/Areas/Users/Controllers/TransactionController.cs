using Microsoft.AspNetCore.Mvc;
using SiFoodProjectFormal2._0.Models;

namespace sifoodprojectformal2._0.Areas.Users.Controllers
{
    [Area("Users")]
    public class TransactionController : Controller
    {
        private readonly SifoodContext _context;
        public TransactionController(SifoodContext context)
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
