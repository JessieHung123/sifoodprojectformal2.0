using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SiFoodProjectFormal2._0.Models;
using SiFoodProjectFormal2._0.ViewModels.Users;

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


        [HttpPost]
        public string DeliverOrder(CreateOrderVM model)
        {




            return "新增外送訂單成功";
        }


        [HttpPost]
        public string TakeOutOrder(CreateOrderVM model)
        {



            return "新增自取訂單成功";
        }

        public IActionResult Payment()
        {
            return View();
        }
    }
}
