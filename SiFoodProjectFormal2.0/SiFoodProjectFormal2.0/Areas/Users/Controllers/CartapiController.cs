using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using SiFoodProjectFormal2._0.Areas.Users.Models.ViewModels;
using SiFoodProjectFormal2._0.Models;
using System.Security.Cryptography.X509Certificates;

namespace SiFoodProjectFormal2._0.Areas.Users.Controllers
{
    [Route("api/CartVMapi/[action]")]
    [ApiController]
    public class CartapiController : ControllerBase
    {
        private readonly Sifood3Context _context;
        

        public CartapiController(Sifood3Context context)
        {
            _context = context;
            
        }


        //取得購物車商品
        // GET: api/CartVMapi
        [EnableQuery]
        [HttpGet("{id}")]
        public IEnumerable<CartVM> GetCarts(string id)
        {

            id = "U002";//先寫死
            
            var cart = _context.Carts.Where(c => c.UserId == id).Select(c => new CartVM
            {
                ProductId = c.ProductId,
                ProductName = _context.Products
                            .Where(p => p.ProductId == c.ProductId).Select(p => p.ProductName).Single(),
                Quantity = c.Quantity,
                TotalPrice = (c.Quantity) * _context.Products.Where(p => p.ProductId == c.ProductId).Select(p => p.UnitPrice).FirstOrDefault(),
                UnitPrice = _context.Products
                            .Where(p => p.ProductId == c.ProductId).Select(p => p.UnitPrice).Single(),
                StoreName = _context.Stores.Where(s => s.StoreId == c.Product.StoreId).Select(p => p.StoreName).Single(),

            });
            return cart;

        }

        
        // PUT: api/CartVMapi/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCart(string id, Cart cart)
        {
            if (id != cart.UserId)
            {
                return BadRequest();
            }

            _context.Entry(cart).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CartExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/CartVMapi
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Cart>> PostCart(Cart cart)
        {
          if (_context.Carts == null)
          {
              return Problem("Entity set 'SifoodContext.Carts'  is null.");
          }
            _context.Carts.Add(cart);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (CartExists(cart.UserId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetCart", new { id = cart.UserId }, cart);
        }

        // DELETE: api/CartVMapi/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCart(string id)
        {
            if (_context.Carts == null)
            {
                return NotFound();
            }
            var cart = await _context.Carts.FindAsync(id);
            if (cart == null)
            {
                return NotFound();
            }

            _context.Carts.Remove(cart);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CartExists(string id)
        {
            return (_context.Carts?.Any(e => e.UserId == id)).GetValueOrDefault();
        }
    }
}
