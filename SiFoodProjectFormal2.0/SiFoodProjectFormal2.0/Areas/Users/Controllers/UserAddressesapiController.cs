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
                UserPhone = x.User.UserPhone
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
        public async Task<string> PostUserAddress([FromBody] UserAddressesVM userAddressesVM)
        {
            UserAddress userAddresses = new UserAddress
            {
                UserId = userAddressesVM.UserId,
                UserDetailAddress = userAddressesVM.UserDetailAddress,
                UserRegion = userAddressesVM.UserRegion,
                UserCity = userAddressesVM.UserCity,
            };
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
           catch (DbUpdateException ex)
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
