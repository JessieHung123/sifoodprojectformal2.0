using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SiFoodProjectFormal2._0.Areas.Stores.ViewModels;
using SiFoodProjectFormal2._0.Models;
using System.Drawing;
using System.Net;
using System.Security.Cryptography.X509Certificates;

namespace SiFoodProjectFormal2._0.Areas.Stores.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class InfoModifyController : ControllerBase
    {
        string targetStoreId = "S002";
        private readonly Sifood3Context _context;

        public InfoModifyController(Sifood3Context context)
        {
            _context = context;
        }

        public async Task<List<InfoModifyVM>> GetAll()
        {
            return await _context.Stores
                .Where(e => e.StoreId == targetStoreId).Select(x => new InfoModifyVM
                {
                    StoreId = x.StoreId,
                    StoreName = x.StoreName,
                    ContactName = x.ContactName,
                    Email = x.Email,
                    Phone = x.Phone,
                    City = x.City,
                    Region = x.Region,
                    Address = x.Address,
                    Description = x.Description,
                    OpeningTime = x.OpeningTime,
                    ClosingDay = x.ClosingDay,
                    PhotosPath = x.PhotosPath,
                    LogoPath = x.LogoPath,
                    PhotosPath2 = x.PhotosPath2,
                    PhotosPath3 = x.PhotosPath3
                }).ToListAsync();
        }
    }




}
