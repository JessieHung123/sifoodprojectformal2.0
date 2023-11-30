using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SiFoodProjectFormal2._0.Areas.Users.Models.ViewModels;
using SiFoodProjectFormal2._0.Models;
using SiFoodProjectFormal2._0.ViewModels.Users;

namespace SiFoodProjectFormal2._0.Areas.Users.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RealTimeOrdersapiController : ControllerBase
    {
        private readonly Sifood3Context _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public RealTimeOrdersapiController(Sifood3Context context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: api/RealTimeOrdersapi
        [HttpGet]
        public async Task<IEnumerable<Order>> GetOrders()
        {
            return _context.Orders;
        }

        // GET: api/RealTimeOrdersapi/5
        [HttpGet("{id}")]
        public object GetOrder(string id)
        {
            TimeZoneInfo taiwanTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Taipei Standard Time");
            DateTime utcNow = DateTime.UtcNow;
            DateTime taiwanTime = TimeZoneInfo.ConvertTimeFromUtc(utcNow, taiwanTimeZone);
            //List<int> StatusIdToCheck = new List<int> {1, 2, 3, 4};
            //var order = await _context.Orders.FindAsync(id);
            return _context.Orders.AsNoTracking().Include(x => x.User).Include(x => x.OrderDetails).ThenInclude(x => x.Product).Where(c => c.UserId == id && c.Status.StatusId != 5 && c.Status.StatusId != 6 && c.Status.StatusId != 7)
                 .Select(z => new OrderVM
                 {
                     OrderId = z.OrderId,
                     OrderDuration = (taiwanTime - z.OrderDate).TotalMinutes,                
                     OrderDate = z.OrderDate.ToString("yyyy-MM-dd"),
                     OrderTime = z.OrderDate.ToString("HH:mm"),
                     DeliveryMethod = z.DeliveryMethod,
                     Address = z.Address,
                     Status = z.Status.StatusName,
                     StatusId = z.StatusId,
                     UserName = z.User.UserName,
                     UserEmail = z.User.UserEmail,
                     UserPhone = z.User.UserPhone,
                     PaymentMethodＮame = z.Payment.PaymentMethodＮame,
                     PaymentDate = z.Payment.PaymentTime.ToString("yyyy-MM-dd"),
                     PaymentTime = z.Payment.PaymentTime.ToString("HH:mm"),
                     OrderDetails = z.OrderDetails.Select(p => new OrderDetailsVM
                     {
                         PhotoPath = p.Product.PhotoPath,
                         ProductName = p.Product.ProductName,
                         UnitPrice = p.Product.UnitPrice,
                         Quantity = p.Quantity,
                         Total = p.Quantity * p.Product.UnitPrice,
                     }),
                     ShippingFee = z.ShippingFee,
                     Subtotal = z.OrderDetails.Sum(p => p.Quantity * p.Product.UnitPrice),
                     TotalQuantity = z.OrderDetails.Sum(p => p.Quantity),
                     DriverFullName = z.Driver.FullName
                 });
        }

        // PUT: api/RealTimeOrdersapi/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<string> PutOrder(string id, [FromBody] RealTimeOrderVM realTimeOrderVM)
        {
            if (id != realTimeOrderVM.OrderId)
            {
                return "失敗";
            }
            Order? order = await _context.Orders.FindAsync(id);
            order.StatusId = realTimeOrderVM.StatusId;
            _context.Entry(order).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrderExists(id))
                {
                    return "取消失敗";
                }
                else
                {
                    throw;
                }
            }
            return "已完成";
        }

        // POST: api/RealTimeOrdersapi
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Order>> PostOrder(Order order)
        {
            if (_context.Orders == null)
            {
                return Problem("Entity set 'Sifood3Context.Orders'  is null.");
            }
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetOrder", new { id = order.OrderId }, order);
        }

        // DELETE: api/RealTimeOrdersapi/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(string id)
        {
            if (_context.Orders == null)
            {
                return NotFound();
            }
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool OrderExists(string id)
        {
            return (_context.Orders?.Any(e => e.OrderId == id)).GetValueOrDefault();
        }
    }
}
