﻿using Microsoft.AspNetCore.Hosting;
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

        public HomeController(Sifood3Context context)
        {
            _context = context;
        }
        //[Route("Main")]
        // GET: Products
        [HttpGet] //uri:/
        public async Task<IActionResult> Main()
        {
            //var sifoodContext = _context.Products.Include(p => p.Category).Include(p => p.Store).Where(p => p.Store.StoreId == targetStoreId);
            var products = _context.Products.Where(p => p.StoreId == targetStoreId);
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
            ViewBag.Storephoto = await _context.Stores.Where(s => s.StoreId == targetStoreId).Select(s => s.LogoPath).FirstOrDefaultAsync();
            ViewBag.SumReleasedQty = SumReleasedQty;
            return View(await products.ToListAsync());
        }

        public async Task<IActionResult> SaleInfo()
        {
            var sifoodContext2 = _context.OrderDetails.Include(d => d.Order).Include(d => d.Product).Select(x => new 
            { StoreId = x.Product.StoreId,
                UnitPrice = x.Product.UnitPrice,
                OrderId = x.OrderId,
                OrderDetailId = x.OrderDetailId,
                ProductId = x.ProductId,
                Quantity = x.Quantity,
                ProductName = x.Product.ProductName,
                OrderStatus = x.Order.StatusId
            })
                .Where(e => e.StoreId == targetStoreId && e.OrderStatus != 1 && e.OrderStatus != 7);
            return Json(sifoodContext2);

        }
        [Route("RealTimeOrders")]
        public IActionResult RealTimeOrders()
        {
            return View();
        }
        [Route("History")]
        public IActionResult History()
        {
            return View();
        }
        //[Route("ProductManage")]
        public IActionResult ProductManage()
        {
            return View();
        }
        public IActionResult InfoModify()
        {
            return View();
        }
        [Route("Review")]
        public IActionResult Review()
        {
            return View();
        }
        [Route("FAQ")]
        public IActionResult FAQ()
        {
            return View();
        }
    }
}
