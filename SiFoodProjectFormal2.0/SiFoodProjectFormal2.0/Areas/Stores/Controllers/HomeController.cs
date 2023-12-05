using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SiFoodProjectFormal2._0.Areas.Stores.ViewModels;
using SiFoodProjectFormal2._0.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace sifoodprojectformal2._0.Areas.Stores.Controllers
{
    [Area("Stores")]
    public class HomeController : Controller
    {

        //----指定特定Stores-到時候要從login裡面拿到sotresId---//
        string targetStoreId = "S001";
        //------------------------//
        private readonly Sifood3Context _context;

        public HomeController(Sifood3Context context)
        {
            _context = context;
        }

        // GET: Products
        [HttpGet] //uri:/
        public async Task<IActionResult> Main()
        {
            //var sifoodContext = _context.Products.Include(p => p.Category).Include(p => p.Store).Where(p => p.Store.StoreId == targetStoreId);
            var sifoodContext = _context.Products.Where(p => p.StoreId == targetStoreId);
            string storeName = await _context.Stores.Where(s => s.StoreId == targetStoreId).Select(s => s.StoreName).FirstOrDefaultAsync();
            int SumReleasedQty = _context.Products.Where(p => p.StoreId == targetStoreId).Sum(p => p.OrderedQty);
            //int ReleasedQty = await _context.Products.Where(od => od.StoreId == targetStoreId).Sum(od => od.ReleasedQty);
            int status1Count = await _context.Orders.CountAsync(od => od.StatusId == 1 && od.StoreId == targetStoreId);
            int status2Count = await _context.Orders.CountAsync(od => od.StatusId == 2 && od.StoreId == targetStoreId);
            int status3Count = await _context.Orders.CountAsync(od => od.StatusId == 3 && od.StoreId == targetStoreId);
            int status4Count = await _context.Orders.CountAsync(od => od.StatusId == 4 && od.StoreId == targetStoreId);
            ViewBag.StoreName = storeName;
            ViewBag.status1 = status1Count;
            ViewBag.status2 = status2Count;
            ViewBag.status3 = status3Count;
            ViewBag.status4 = status4Count;
            ViewBag.SumReleasedQty = SumReleasedQty;
            return View(await sifoodContext.ToListAsync());
        }

        public async Task<IActionResult> Main2()
        {
            var sifoodContext2 = _context.OrderDetails.Include(d => d.Product).Select(x => new { StoreId = x.Product.StoreId, UnitPrice = x.Product.UnitPrice, OrderId = x.OrderId, ProductId = x.ProductId, ProductName = x.Product.ProductName }).Where(e => e.StoreId == targetStoreId);
            return Json(sifoodContext2);

        }

        //public async Task<IActionResult> Main2()
        //{
        //    var sifoodContext2 = _context.OrderDetails.Include(d => d.Product).Select(x => new { StoreId = x.Product.StoreId, OrderDetailId = x.OrderDetailId, OrderId = x.OrderId, ProductId = x.ProductId, ProductName = x.Product.ProductName }).Where(e => e.StoreId == targetStoreId);
        //    return Json(sifoodContext2);

        //}

        //public async Task<IActionResult> Main3()
        //{
        //    var sifoodContext3 = _context.Products.Select(x => new { StoreId = x.StoreId, Description = x.Description, ProductName = x.ProductName, ReleasedQty = x.ReleasedQty }).Where(p => p.StoreId == targetStoreId);
        //    return Json(sifoodContext3);

        //}

        //----------------------------------//
        //public IActionResult Main()
        //{
        //    return View();
        //}
        public IActionResult RealTimeOrders()
        {
            return View();
        }

        //=========歷史訂單========//
        public IActionResult History(string searchTerm = null, string sortOption = "Status", int pageSize = 20)
        {
            // 假定的用戶ID，之後需要替換為當前登入用戶的ID
            var loginuserId = "S001";

            IQueryable<Order> historyOrdersQuery = _context.Orders
                // 添加這行以過濾該用戶的訂單
                .Where(o => o.StoreId == loginuserId)

            .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Product)
            .Include(o => o.Status);

            // 應用關鍵字過濾
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                historyOrdersQuery = historyOrdersQuery.Where(o =>
                    o.OrderDetails.Any(od => od.Product.ProductName.Contains(searchTerm)));
            }

            //保持搜尋關鍵字在搜尋欄
            ViewBag.SearchTerm = searchTerm;

            //計算總訂單數
            var totalOrdersCount = historyOrdersQuery.Count();
            ViewBag.TotalOrdersCount = totalOrdersCount;


            // Sort排序
            switch (sortOption)
            {
                case "Status":
                    historyOrdersQuery = historyOrdersQuery.OrderBy(o => o.Status.StatusName);
                    break;
                case "Low to High":
                    historyOrdersQuery = historyOrdersQuery.OrderBy(o =>
                        _context.OrderDetails
                        .Where(od => od.OrderId == o.OrderId)
                        .Sum(od => od.Quantity * od.Product.UnitPrice) +
                        (o.ShippingFee));
                    break;
                case "High to Low":
                    historyOrdersQuery = historyOrdersQuery.OrderByDescending(o =>
                        _context.OrderDetails
                        .Where(od => od.OrderId == o.OrderId)
                        .Sum(od => od.Quantity * od.Product.UnitPrice) +
                        (o.ShippingFee));
                    break;
                case "Newest":
                    historyOrdersQuery = historyOrdersQuery.OrderByDescending(o => o.OrderDate);
                    break;
                case "Oldest":
                    historyOrdersQuery = historyOrdersQuery.OrderBy(o => o.OrderDate);
                    break;
                default:
                    // 默認排序：按訂購日期由新到舊排序
                    historyOrdersQuery = historyOrdersQuery.OrderByDescending(o => o.OrderDate);
                    break;
            }

            // 在過濾後的結果上應用分頁
            var historyOrders = historyOrdersQuery
                .Take(pageSize)
                .Select(o => new HistoryOrderVM
                {
                    // ViewModel的初始化
                    StoreId = o.StoreId,
                    OrderId = o.OrderId,
                    OrderDate = o.OrderDate,
                    Status = o.Status.StatusName,
                    Quantity = o.OrderDetails.Sum(od => od.Quantity),
                    TotalPrice = Convert.ToInt32(_context.OrderDetails
                                                    .Where(od => od.OrderId == o.OrderId)
                                                    .Sum(od => od.Quantity * od.Product.UnitPrice) +
                                                    o.ShippingFee),
                    FirstProductPhotoPath = o.OrderDetails.FirstOrDefault().Product.PhotoPath,
                    FirstProductName = o.OrderDetails.FirstOrDefault().Product.ProductName
                }).ToList();

            return View(historyOrders);
        }


        //訂單明細方法GetOrderDetails
        public async Task<IActionResult> GetOrderDetails(string orderId)
        {
            var order = await _context.Orders
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Product)
               
                .FirstOrDefaultAsync(o => o.OrderId == orderId);

            if (order == null)
            {
                return NotFound();
            }

            var historyOrderDetailsVM = new HistoryOrderDetailVM
            {
                OrderId = order.OrderId,
                OrderDate = order.OrderDate,
                ShippingFee = Convert.ToInt32(order.ShippingFee),

                Items = order.OrderDetails.Select(od => new HistoryOrderDetailItemVM
                {
                    PhotoPath = od.Product.PhotoPath,
                    ProductName = od.Product.ProductName,
                    UnitPrice = Convert.ToInt32(od.Product.UnitPrice),
                    Quantity = od.Quantity,
                }).ToList(),
            };

            return PartialView("_OrderDetailPartialS", historyOrderDetailsVM);
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
