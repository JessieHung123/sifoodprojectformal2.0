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
                UserName = y.User.UserName,
                UserAddress = y.User.UserAddresses.Select(y => y.UserDetailAddress).FirstOrDefault(),
                ProductId = y.ProductId,
                ProductName = _context.Products.Where(c => c.ProductId == y.ProductId).Select(x => x.ProductName).Single(),
                Quantity = y.Quantity,
                UnitPrice = _context.Products.Where(c => c.ProductId == y.ProductId).Select(x => x.UnitPrice).FirstOrDefault(),
                TotalPrice = y.Quantity * _context.Products.Where(c => c.ProductId == y.ProductId).Select(x => x.UnitPrice).FirstOrDefault(),
                StoreName = _context.Stores.Where(s => s.StoreId == y.Product.StoreId).Select(p => p.StoreName).Single(),
            });
            return CheckOutData;
        }

        [HttpPost]
        [Route("/Transaction/TakeOutOrder")]
        public string TakeOutOrder(CreateOrderVM model)
        {
            string StoreId = _context.Stores.Where(s=>s.StoreName == model.StoreName).Select(s => s.StoreId).Single();
            string UserId = _context.Users.Where(x=>x.UserName == model.UserName).Select(x => x.UserId).Single();
            
            Order order = new Order
            {
                OrderDate = DateTime.Now,
                StoreId = StoreId,
                UserId = UserId,
                DeliveryMethod = "自取",
                StatusId = 1,
                TotalPrice = model.TotalPrice
            };

            foreach (var product in model.ProductName)
            {
                OrderDetail orderDetail = new OrderDetail
                {
                    OrderId = order.OrderId,
                    ProductId = product.ProductId,
                    Quantity = model.Quantity,
                };

                _context.OrderDetails.Add(orderDetail);
                _context.SaveChanges();
            }
            _context.Orders.Add(order);
            _context.SaveChanges();

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
