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
    public class StoreProductsapiController : ControllerBase
    {
        private readonly Sifood3Context _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public StoreProductsapiController(Sifood3Context context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: api/StoreProductsapi
        [HttpGet]
        public async Task<IEnumerable<Store>> GetStores()
        {
            return _context.Stores;
        }

        // GET: api/StoreProductsapi/5
        [HttpGet("{id}")]
        public object GetStore(string id)
        {
            var today = DateTime.Today;
            var currentTime = DateTime.Now.TimeOfDay;
            var store = _context.Stores.AsNoTracking()
                .Include(x => x.Products)
                .Include(x => x.Orders)
                .ThenInclude(x => x.Comment)
                .Where(c => c.StoreId == id);
            return store.Select(z => new StoreProductsVM
            {
                StoreName = z.StoreName,
                StoreId = z.StoreId,
                Email = z.Email,
                Phone = $"{z.Phone.Substring(0, 2)} {z.Phone.Substring(2)}",
                Address = z.Address,
                OpeningTime = z.OpeningTime,
                PhotosPath = z.PhotosPath,
                Description = z.Description,
                LogoPath = z.LogoPath,
                CommentCount = z.Orders.Where(x => x.Comment != null).Count(),
                CommentRank = z.Orders.Sum(x => x.Comment.CommentRank),
                WeekdayOpeningTime = z.OpeningTime.Substring(0, 16),
                WeekendOpeningTime = z.OpeningTime.Substring(17, 16),

                Products = z.Products.Where(p => p.RealeasedTime.Date == today &&
                                                 p.RealeasedTime.TimeOfDay < currentTime &&
                                                 p.SuggestPickEndTime > currentTime &&
                                                 p.IsDelete == 1)
                                     .Select(p => new ProductsVM
                                     {
                                         UnitPrice = p.UnitPrice,
                                         ProductName = p.ProductName,
                                         CategoryId = p.CategoryId,
                                         CategoryName = p.Category.CategoryName,
                                         avalibleQty = p.ReleasedQty - p.OrderedQty,
                                         SuggestPickUpTime = $"{p.SuggestPickUpTime.ToString(@"hh\:mm")} ~ {p.SuggestPickEndTime.ToString(@"hh\:mm")}",
                                         RealeasedTime = p.RealeasedTime,
                                         PhotoPath = p.PhotoPath,
                                     }),

                CategoryList = z.Products.Where(p => p.RealeasedTime.Date == today &&
                                                     p.RealeasedTime.TimeOfDay < currentTime &&
                                                     p.SuggestPickEndTime > currentTime &&
                                                     p.IsDelete == 1)
                                         .Select(y => y.Category.CategoryName).Distinct().ToArray(),

                Comment = z.Orders
                    .Where(x => x.Comment != null)
                    .Select(d => new CommentVM
                    {
                        Contents = d.Comment.Contents,
                        CommentRank = d.Comment.CommentRank,
                        User = d.User.UserName,
                        DeliveryMethod = d.DeliveryMethod
                    })
            });
        }
        [HttpGet("favorite/status/{userId}/{storeId}")]
        public object GetFavoriteStatus(string userId, string storeId)
        {
            //return _context.Favorites.Any(f => f.UserId == userId && f.StoreId == storeId);
            bool isFavorite = _context.Favorites.Any(f => f.UserId == userId && f.StoreId == storeId);
            return Ok(new { IsFavorite = isFavorite });
        }
        [HttpPost("favorite/add")]
        public async Task<string> SaveToFavorites([FromBody] FavoriteVM favoriteVM)
        {
            string userId = favoriteVM.UserId;
            string storeId = favoriteVM.StoreId;
            if (favoriteVM == null || string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(storeId))
            {
                return "無法新增";
            }
            if (!_context.Favorites.Any(f => f.UserId == userId && f.StoreId == storeId))
            {
                Favorite favorite = new Favorite
                {
                    UserId = favoriteVM.UserId,
                    StoreId = favoriteVM.StoreId
                };
                _context.Favorites.Add(favorite);
                await _context.SaveChangesAsync();
            }
            return "收藏成功!";
        }
        [HttpDelete("favorite/remove")]
        public async Task<string> RemoveFromFavorites([FromBody] FavoriteVM favoriteVM)
        {
            if (favoriteVM == null || string.IsNullOrEmpty(favoriteVM.UserId) || string.IsNullOrEmpty(favoriteVM.StoreId))
            {
                return "未收藏";
            }

            var existingFavorite = _context.Favorites
                .FirstOrDefault(f => f.UserId == favoriteVM.UserId && f.StoreId == favoriteVM.StoreId);

            if (existingFavorite != null)
            {
                _context.Favorites.Remove(existingFavorite);
                await _context.SaveChangesAsync();
                return "已取消收藏";
            }

            return "未收藏";

        }
        //[HttpGet("Search")]
        //public async Task<object> SearchProducts(string? query)
        //{
        //    string storeId = "S001";
        //    DateTime today = DateTime.Today;
        //    TimeSpan currentTime = DateTime.Now.TimeOfDay;

        //    var filterProducts =  _context.Products
        //        .Where(p => p.StoreId == storeId &&
        //                    p.RealeasedTime.Date == today &&
        //                    p.RealeasedTime.TimeOfDay < currentTime &&
        //                    p.SuggestPickEndTime > currentTime &&
        //                    p.IsDelete == 1 &&
        //                    (string.IsNullOrEmpty(query) ||
        //                     p.ProductName.Contains(query) ||
        //                     p.Category.CategoryName.Contains(query)));


        //    var products =  await filterProducts.Select(x => new ProductsVM
        //        {
        //            UnitPrice = x.UnitPrice,
        //            ProductName = x.ProductName,
        //            CategoryId = x.CategoryId,
        //            CategoryName = x.Category.CategoryName,
        //            avalibleQty = x.ReleasedQty - x.OrderedQty,
        //            SuggestPickUpTime = $"{x.SuggestPickUpTime:hh\\:mm} ~ {x.SuggestPickEndTime:hh\\:mm}",
        //            RealeasedTime = x.RealeasedTime,
        //            PhotoPath = x.PhotoPath,
        //        })
        //        .ToListAsync();
        //    var categories = await filterProducts.Select(x => x.Category.CategoryName)
        //                                         .Distinct()
        //                                         .ToListAsync();

        //    var searchResults = new
        //    {
        //        Products = products,
        //        Categories = categories,
        //    };

        //    return searchResults;
        
        //}
        [HttpPut("{id}")]
        public async Task<IActionResult> PutStore(string id, Store store)
        {
            if (id != store.StoreId)
            {
                return BadRequest();
            }

            _context.Entry(store).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!StoreExists(id))
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

        // POST: api/StoreProductsapi
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Store>> PostStore(Store store)
        {
            if (_context.Stores == null)
            {
                return Problem("Entity set 'Sifood3Context.Stores'  is null.");
            }
            _context.Stores.Add(store);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetStore", new { id = store.StoreId }, store);
        }

        // DELETE: api/StoreProductsapi/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStore(string id)
        {
            if (_context.Stores == null)
            {
                return NotFound();
            }
            var store = await _context.Stores.FindAsync(id);
            if (store == null)
            {
                return NotFound();
            }

            _context.Stores.Remove(store);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool StoreExists(string id)
        {
            return (_context.Stores?.Any(e => e.StoreId == id)).GetValueOrDefault();
        }
    }
}
