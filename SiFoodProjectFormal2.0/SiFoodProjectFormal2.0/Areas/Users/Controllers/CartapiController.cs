﻿using Microsoft.AspNetCore.Authorization;
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
        private readonly IUserIdentityService _userIdentityService;


        public CartapiController(Sifood3Context context, IUserIdentityService userIdentityService)
        {
            _context = context;
            _userIdentityService = userIdentityService;
        }


        //取得購物車商品
        // GET: api/CartVMapi
        [HttpGet]
        //[Authorize]
        public async Task<List<CartVM>> GetCarts()
        {
            string userId = _userIdentityService.GetUserId();
            var cart = await _context.Carts.Where(c => c.UserId == userId).Select(c => new CartVM
            {
                UserId = userId,
                ProductId = c.ProductId,
                ProductName = _context.Products.Where(p => p.ProductId == c.ProductId).Select(p => p.ProductName).Single(),
                Quantity = c.Quantity,
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
            string userId = _userIdentityService.GetUserId();
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
        {
            string userId = _userIdentityService.GetUserId();
            Cart? cart = new Cart
            {
                ProductId = cartVM.ProductId,
                Quantity = cartVM.Quantity,
                UserId = userId,   //cartVM.UserId寫死
            };
            try
            {
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
                string userId = _userIdentityService.GetUserId();
                var cartItem= await _context.Carts.FirstOrDefaultAsync(c=>c.ProductId==cartVM.ProductId&&c.UserId==userId);
                _context.Carts.Remove(cartItem);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception) { return false; }
        }
        //刪除購物車商品

        [HttpDelete]
        public async Task<bool> DeleteUserAllCart()
        {
            string userId = _userIdentityService.GetUserId();
            var cart = await _context.Carts.Where(c => c.UserId == userId).ToListAsync();
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
