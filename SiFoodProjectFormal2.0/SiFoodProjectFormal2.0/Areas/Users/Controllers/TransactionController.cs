using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SiFoodProjectFormal2._0.Areas.Users.Models.ViewModels;
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

        public IActionResult CheckOut()
        {
            return View();
        }

        [HttpGet]
        [Route("/Transaction/GetCheckoutData")]
        public IQueryable<CheckOutVM> GetCheckoutData()
        {
            string id = "U002";
            var CheckOutData = _context.Carts.Include(x => x.User).Where(y => y.UserId == id).Select(y => new CheckOutVM
            {
                
                UserAddress = y.User.UserAddresses.Select(y=>y.UserDetailAddress).FirstOrDefault(),
                ProductId = y.ProductId,
                ProductName = _context.Products.Where(c => c.ProductId == y.ProductId).Select(x => x.ProductName).Single(),
                Quantity = y.Quantity,
                UnitPrice = _context.Products.Where(c => c.ProductId == y.ProductId).Select(x => x.UnitPrice).FirstOrDefault(),
                TotalPrice = y.Quantity * _context.Products.Where(c => c.ProductId == y.ProductId).Select(x => x.UnitPrice).FirstOrDefault()
            });

            return CheckOutData;
        }

        [HttpPost]
        public string TakeOutOrder(CreateOrderVM model)
        {
            return "新增自取訂單成功";
        }

        [HttpPost]
        public string DeliverOrder(CreateOrderVM model)
        {
            return "新增外送訂單成功";
        }

        public IActionResult Payment()
        {
            return View();
        }
    }
}
