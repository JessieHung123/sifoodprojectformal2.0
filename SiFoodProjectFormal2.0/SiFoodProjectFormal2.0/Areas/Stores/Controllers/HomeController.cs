using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        private readonly IWebHostEnvironment _webHostEnvironment;

        public HomeController(Sifood3Context context, IWebHostEnvironment webHostenvironment)
        {
            _context = context;
            _webHostEnvironment = webHostenvironment;
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
            var sifoodContext2 = _context.OrderDetails.Include(d => d.Product)
                .Select(x => new { 
                    StoreId = x.Product.StoreId,
                    UnitPrice = x.Product.UnitPrice,
                    OrderId = x.OrderId,
                    ProductId = x.ProductId,
                    ProductName = x.Product.ProductName })
                .Where(e => e.StoreId == targetStoreId);
            return Json(sifoodContext2);
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

        public IActionResult GetProduct()
        {
            var ProductContext = _context.Products.Include(d => d.Category).
                Select(x => new {
                    StoreId = x.StoreId,
                    ProductId = x.ProductId,
                    ProductName = x.ProductName,
                    Category = x.Category.CategoryName,
                    RealeasedQty = x.ReleasedQty,
                    RealeasedTime = x.RealeasedTime,
                    UnitPrice = x.UnitPrice,
                    PhotoPath = x.PhotoPath
                }).Where(e => e.StoreId == targetStoreId);
            return Json(ProductContext);
        }

        //public async Task<FileResult> GetPicture(int id)
        //{
        //    string WebRootPath = _webHostEnvironment.WebRootPath;
        //    string Filename = Path.Combine(WebRootPath, "images", "Noimage.png");
        //    Product? c = await _context.Products.FindAsync(id);
        //    byte[] ImageContent = c.PhotoPath != null ? c.PhotoPath : System.IO.File.ReadAllBytes(Filename);
        //    return File(ImageContent, "image/jpeg");
        //}

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
