using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
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
        [HttpGet]
        public async Task<List<CartVM>> GetCarts()
        {

            string GetUserId = "U001";//先寫死
            
            var cart = await _context.Carts.Where(c => c.UserId == GetUserId).Select(c => new CartVM
            {
                ProductId = c.ProductId,
                ProductName =  _context.Products.Where(p => p.ProductId == c.ProductId).Select(p => p.ProductName).Single(),
                Quantity = c.Quantity,
                TotalPrice = (c.Quantity) * _context.Products.Where(p => p.ProductId == c.ProductId).Select(p => p.UnitPrice).FirstOrDefault(),
                UnitPrice = _context.Products.Where(p => p.ProductId == c.ProductId).Select(p => p.UnitPrice).Single(),
                StoreName = _context.Stores.Where(s => s.StoreId == c.Product.StoreId).Select(p => p.StoreName).Single(),
                PhotoPath= _context.Products.Where(p => p.ProductId == c.ProductId).Select(p => p.PhotoPath).Single(),

            }).ToListAsync();
            return cart;

        }

        //修改商品數量
        // PUT: api/CartVMapi/
        [HttpPut]
        public async Task<bool> ChangeQty([FromBody] CartVM cartVM)
        {
            string userId = "U001";//cartVM.UserId寫死
            Cart? cart = await _context.Carts.FirstOrDefaultAsync(c => c.UserId == userId&&c.ProductId == cartVM.ProductId);
            cart.Quantity = cartVM.Quantity;
            try {
                _context.Entry(cart).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return true;
            }
            catch(Exception) { return false; }
            

        }
        //加入購物車:一個user的購物車只能限定一間商店，不然要alert(購物車只能放一間商店，是否要更換店家?)
        // POST: api/CartVMapi
        [HttpPost]
        public string AddToCart([FromBody]CartVM cartVM)
        {//只能限制加一間>>還沒寫
            string userId = "U001";//cartVM.UserId寫死
            
            try
            {
                
                Cart? cart = new Cart
                {
                    ProductId = cartVM.ProductId,
                    Quantity = cartVM.Quantity,
                    UserId = userId,   //cartVM.UserId寫死
                    
                };
                _context.Carts.Add(cart);
                 _context.SaveChangesAsync();
                    return "新增商品成功!";
                
            }
            catch (Exception){ return "新增商品失敗!"; }
        }
        //刪除購物車商品
        
        [HttpDelete]
        public async Task<bool> DeleteCartItem([FromBody] CartVM cartVM)
        {

            if (cartVM == null) return false;
            try
            {
                string userId = cartVM.UserId;
                var cartItem= await _context.Carts.FirstOrDefaultAsync(c=>c.ProductId==cartVM.ProductId&&c.UserId==userId);
                _context.Carts.Remove(cartItem);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception) { return false; }
        }
        //刪除購物車商品

        [HttpDelete("{userid}")]
        public async Task<bool> DeleteUserAllCart(string userid)
        {
            var cart = await _context.Carts.Where(c => c.UserId == userid).ToListAsync();
            try
            {
                 _context.Carts.RemoveRange(cart);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception) { return false; }
        }
        
    }
}
