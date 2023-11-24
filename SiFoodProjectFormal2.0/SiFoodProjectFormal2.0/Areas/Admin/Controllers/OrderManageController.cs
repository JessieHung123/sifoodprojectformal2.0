using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SiFoodProjectFormal2._0.Models;

namespace SiFoodProjectFormal2._0.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("/OrderManage/{action}/{OrderId?}/{ProductId?}")]
    public class OrderManageController : Controller
    {
        private readonly Sifood3Context _context;
       
        public OrderManageController(Sifood3Context context)
        {
            _context = context;
        }

        // GET: Admin/OrderManage
        public async Task<IActionResult> Index()
        {
            var sifood3Context = _context.OrderDetails.Include(o => o.Order).ThenInclude(o => o.User).Include(o => o.Product)
                .Select(o => new OrderManageViewModel
                {
                    UserName = o.Order.User.UserName,
                    OrderAddress=o.Order.Address,
                    ProductName=o.Product.ProductName,
                    Quantity=o.Quantity,
                    UserPhone=o.Order.User.UserPhone,
                    OrderDetailId=o.OrderDetailId,
                    OrderId=o.OrderId,
                    OrderDate=o.Order.OrderDate,
                    StatusName=o.Order.Status.StatusName,
                    ProductId=o.ProductId,
                    ProductUnitPrice=o.Product.UnitPrice,
                    StoreName=o.Order.Store.StoreName,
                    StorePhone=o.Order.Store.Phone,
                    StoreAddress=o.Order.Store.Address,

                }).ToListAsync();
                return View(await sifood3Context);
        }

        // GET: Admin/OrderManage/Details/5
        public async Task<IActionResult> Details(string? OrderId,int? ProductId)
        {
            if (OrderId == null || ProductId==null || _context.OrderDetails == null)
            {
                return NotFound();
            }

            var orderDetail = await _context.OrderDetails
             .FirstOrDefaultAsync(od => od.OrderId == OrderId && od.ProductId == ProductId);

           if (orderDetail == null)
            {
                return NotFound();
            }

            return View();
        }

        //// GET: Admin/OrderManage/Create
        //public IActionResult Create()
        //{
        //    ViewBag.OrderId = new SelectList(_context.Orders, "OrderId", "OrderId");
        //    ViewBag.ProductId = new SelectList(_context.Products, "ProductId", "ProductId");
        //    return View();
        //}

        //// POST: Admin/OrderManage/Create
        //// To protect from overposting attacks, enable the specific properties you want to bind to.
        //// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create([Bind("OrderId,ProductId,Quantity")] OrderDetail orderDetail)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _context.Add(orderDetail);
        //        await _context.SaveChangesAsync();
        //        return RedirectToAction(nameof(Index));
        //    }
        //    ViewBag.OrderId = new SelectList(_context.Orders, "OrderId", "OrderId", orderDetail.OrderId);
        //    ViewBag.ProductId = new SelectList(_context.Products, "ProductId", "ProductId", orderDetail.ProductId);
        //    return View(orderDetail);
        //}

        // GET: Admin/OrderManage/Edit/5
        public object Edit(string? OrderId,int? ProductId)
        {
            if (OrderId==null || ProductId ==null || _context.OrderDetails == null)
            {
                return NotFound();
            }

            var orderDetail = _context.OrderDetails.Where(x => x.OrderId == OrderId && x.ProductId == ProductId);
            var orderDetailVM =  orderDetail.Select(o => new OrderManageViewModel
            {
                UserName = o.Order.User.UserName,
                OrderAddress = o.Order.Address,
                ProductName = o.Product.ProductName,
                Quantity = o.Quantity,
                UserPhone = o.Order.User.UserPhone,
                OrderDetailId = o.OrderDetailId,
                OrderId = o.OrderId,
                OrderDate = o.Order.OrderDate,
                StatusName = o.Order.Status.StatusName,
                ProductId = o.ProductId,
                ProductUnitPrice = o.Product.UnitPrice,
                StoreName = o.Order.Store.StoreName,
                StorePhone = o.Order.Store.Phone,
                StoreAddress = o.Order.Store.Address,

            });
            OrderManageViewModel om = new OrderManageViewModel { };
            if (orderDetail == null)
            {
                return NotFound();
            }
            return View(orderDetailVM);
        }

        // POST: Admin/OrderManage/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string OrderId, int ProductId, [Bind("OrderId,ProductId,Quantity")] OrderManageViewModel orderDetail)
        {
            if (OrderId != orderDetail.OrderId || ProductId != orderDetail.ProductId)
            {
                return NotFound();
            }

            //if (ModelState.IsValid)
            //{
                try
                {
                    _context.Update(orderDetail);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderDetailExists(orderDetail.OrderId, orderDetail.ProductId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            //    return RedirectToAction(nameof(Index));
            //}

            return View(orderDetail);
        }


        // GET: Admin/OrderManage/Delete/5
        public async Task<IActionResult> Delete(string? OrderId,int ProductId)
        {
            if (OrderId == null || ProductId==null || _context.OrderDetails == null)
            {
                return NotFound();
            }

            var orderDetail = await _context.OrderDetails
                .Include(o => o.Order)
                .Include(o => o.Product)
                .FirstOrDefaultAsync(od => od.OrderId ==OrderId && od.ProductId==ProductId);
            if (orderDetail == null)
            {
                return NotFound();
            }

            return View(orderDetail);
        }

        // POST: Admin/OrderManage/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string OrderId,int ProductId)
        {
            //if (_context.OrderDetails == null)
            //{
            //    return Problem("Entity set 'Sifood3Context.OrderDetails'  is null.");
            //}
            var orderDetail = await _context.OrderDetails.FindAsync(OrderId,ProductId);
            if (orderDetail != null)
            {
                _context.OrderDetails.Remove(orderDetail);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OrderDetailExists(string OrderId,int ProductId)
        {
          return (_context.OrderDetails?.Any(e =>e.OrderId == OrderId && e.ProductId==ProductId)).GetValueOrDefault();
        }
    }
}
