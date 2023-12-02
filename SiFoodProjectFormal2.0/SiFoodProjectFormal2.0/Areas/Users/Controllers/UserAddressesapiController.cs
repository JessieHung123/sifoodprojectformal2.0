using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SiFoodProjectFormal2._0.Areas.Users.Models.ViewModels;
using SiFoodProjectFormal2._0.Models;

namespace SiFoodProjectFormal2._0.Areas.Users.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserAddressesapiController : ControllerBase
    {
        private readonly Sifood3Context _context;
        private readonly IWebHostEnvironment _WebHostEnvironment;
        public UserAddressesapiController(Sifood3Context context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _WebHostEnvironment = webHostEnvironment;
        }

        // GET: api/UserAddressesapi
        [HttpGet]
        public async Task<IEnumerable<UserAddress>> GetUserAddresses()
        {       
            return  _context.UserAddresses;
        }

        // GET: api/UserAddressesapi/5
        [HttpGet("{id}")]
        public object GetUserAddress(string id)
        {
            return _context.UserAddresses.Where(u => u.UserId == id).Select(x => new UserAddressesVM
            {
                UserAddressId = x.UserAddressId,
                UserId = x.UserId,
                UserDetailAddress = x.UserDetailAddress,
                UserRegion = x.UserRegion,
                UserCity = x.UserCity,
                UserPhone = x.User.UserPhone,
                IsDefault = x.IsDefault
            });
        }

        // PUT: api/UserAddressesapi/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<string> PutUserAddress(int id, [FromBody] UserAddressesVM userAddressesVM)
        {
            if (id != userAddressesVM.UserAddressId)
            {
                return "修改地址失敗!";
            }
            UserAddress? userAddresses = await _context.UserAddresses.FindAsync(id);
            if (userAddresses == null)
            {
                return "修改地址失敗!";
            }
            if (userAddressesVM.IsDefault)
            {
                var existingDefaultAddresses = await _context.UserAddresses.Where(a => a.UserId == userAddressesVM.UserId && a.IsDefault && a.UserAddressId != id).ToListAsync();

                foreach (var existingDefaultAddress in existingDefaultAddresses)
                {
                    existingDefaultAddress.IsDefault = false;
                    _context.Entry(existingDefaultAddress).State = EntityState.Modified;
                }
            }
            userAddresses.UserDetailAddress = userAddressesVM.UserDetailAddress;
            userAddresses.IsDefault = userAddressesVM.IsDefault;
            _context.Entry(userAddresses).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserAddressExists(id))
                {
                    return "修改地址失敗!";
                }
                else
                {
                    throw;
                }
            }

            return "修改成功!";
        }

        // POST: api/UserAddressesapi
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<string> PostUserAddress([FromForm] UserAddressesVM userAddressesVM)
        {
            int minLength = 5; 
            int maxLength = 100;
            if (string.IsNullOrEmpty(userAddressesVM.UserRegion) || string.IsNullOrEmpty(userAddressesVM.UserCity))
            {
                return "請選擇縣市和行政區";
            }
            if (userAddressesVM.UserDetailAddress.Length < minLength || userAddressesVM.UserDetailAddress.Length > maxLength)
            {
                return $"地址長度為{minLength}到{maxLength}之間";
            }
            UserAddress userAddresses = new UserAddress
            {
                UserId = userAddressesVM.UserId,
                UserDetailAddress = userAddressesVM.UserDetailAddress,
                UserRegion = userAddressesVM.UserRegion,
                UserCity = userAddressesVM.UserCity,
                IsDefault = userAddressesVM.IsDefault
            };
            if (userAddresses.IsDefault)
            {
                var existingDefaultAddresses = await _context.UserAddresses.Where(a => a.UserId == userAddresses.UserId && a.IsDefault).ToListAsync();
                foreach (var existingDefaultAddress in existingDefaultAddresses)
                {
                    existingDefaultAddress.IsDefault = false;
                    _context.Entry(existingDefaultAddress).State = EntityState.Modified;
                }
            }
            _context.UserAddresses.Add(userAddresses);
            await _context.SaveChangesAsync();
            return "新增商品種類成功";
        }

        // DELETE: api/UserAddressesapi/5
        [HttpDelete("{id}")]
        public async Task<string> DeleteUserAddress(int id)
        {
            if (_context.UserAddresses == null)
            {
                return "刪除地址失敗!";
            }
            var userAddress = await _context.UserAddresses.FindAsync(id);
            if (userAddress == null)
            {
                return "刪除地址失敗!";
            }
            try
            {
                _context.UserAddresses.Remove(userAddress);
                await _context.SaveChangesAsync();
            }
           catch (DbUpdateException)
            {
                return "刪除地址關聯紀錄失敗!";
            }

            return "刪除地址成功!";
        }

        private bool UserAddressExists(int id)
        {
            return (_context.UserAddresses?.Any(e => e.UserAddressId == id)).GetValueOrDefault();
        }
    }
}
