using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SiFoodProjectFormal2._0.Areas.Users.Models.ViewModels;
using SiFoodProjectFormal2._0.Models;

namespace SiFoodProjectFormal2._0.Areas.Users.Controllers
{
    [Route("odata/Productapi/[action]")]

    [Area("Users")]
    public class ProductapiController : Controller
    {
        
        Sifood3Context _context;

        public ProductapiController(Sifood3Context context)
        {
            _context = context;

        }
        [HttpGet("{id}")]
        public async Task<IEnumerable<ProductVM>> GetProduct(int id)
        {
            return await _context.Products.Where(p=>p.ProductId==id).Include(p => p.Store).Select(p=>new ProductVM
            {
                ProductId = p.ProductId,
                ProductName = p.ProductName,
                PhotoPath=p.PhotoPath,
                UnitPrice = p.UnitPrice,
                StoreName=p.Store.StoreName,
                Address=p.Store.Address,
                SuggestPickUpTime=p.SuggestPickUpTime.ToString().Substring(0, 5),
                SuggestPickEndTime=p.SuggestPickEndTime.ToString().Substring(0,5),
                ReleasedQty=p.ReleasedQty,
                OrderedQty=p.OrderedQty,
            }).ToListAsync();


        }
        ////取得單一商品資訊
        //[HttpGet("{id}")]
        //public async Task<List<TransportPlanDto>> GetProduct(int id)
        //{
        //    return await _dbContext.Products.Where(p => p.ProductId == id).Select(p => new TransportPlanDto
        //    {
        //        ProductName = p.ProductName,
        //        MainDescribe = p.MainDescribe,
        //        SubDescribe = p.SubDescribe,
        //        ShortDescribe = p.ShortDescribe,
        //        Img = p.Img
        //    }).ToListAsync();
        //}
    }
}
