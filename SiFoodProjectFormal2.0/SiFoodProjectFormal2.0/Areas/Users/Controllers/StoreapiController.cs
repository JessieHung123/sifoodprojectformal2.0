using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Castle.Core.Resource;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.EntityFrameworkCore;
using SiFoodProjectFormal2._0.Areas.Users.Models.ViewModels;
using SiFoodProjectFormal2._0.Models;

namespace SiFoodProjectFormal2._0.Areas.Users.Controllers
{
    [Route("api/Storeapi/[action]")]
    [Area("Users")]
    public class StoreapiController
    {
        private readonly Sifood3Context _context;
        private readonly IUserIdentityService _userIdentityService;
        public StoreapiController(Sifood3Context context, IUserIdentityService userIdentityService)
        {
            _context = context;
            _userIdentityService = userIdentityService;
        }

        [EnableQuery]
        public object Main2()
        {
            return _context.Stores.Include(x => x.Products).ThenInclude(x => x.Category).Include(x => x.Orders)
                .ThenInclude(x => x.Comment).Where(x=>x.StoreIsAuthenticated==1)
                .Select(z => new StoreVM
                {
                    StoreId = z.StoreId,
                    StoreName = z.StoreName,
                    Description = z.Description,
                    LogoPath = z.LogoPath,
                    CommentCount = z.Orders.Where(x => x.Comment != null).Count(),
                    CommentRank = z.Orders.Sum(x => x.Comment.CommentRank),
                    Inventory = z.Products.Where(x => x.RealeasedTime.Date == DateTime.Now.Date && x.SuggestPickEndTime >= DateTime.Now.TimeOfDay).Select(x => x.ReleasedQty - x.OrderedQty).Sum(),
                    WeekdayOpeningTime = z.OpeningTime.Substring(3, 5),
                    WeekdayClosingTime = z.OpeningTime.Substring(11, 5),
                    WeekendOpeningTime = z.OpeningTime.Substring(20, 5),
                    WeekendClosingTime = z.OpeningTime.Substring(28, 5),
                    City = z.City,
                    Region = z.Region,
                    CategoryName = z.Products.Where(x => x.RealeasedTime.Date == DateTime.Today && x.SuggestPickEndTime >= DateTime.Now.TimeOfDay).Select(x => x.Category.CategoryName).Distinct(),
                }).ToList();
        }

        //找到店家是否已被收藏

        [HttpGet]
        public async Task<string[]> GetFavoriteStoreId()
        {

            string userId = _userIdentityService.GetUserId();
            return await _context.Favorites.Where(f => f.UserId == userId).Select(f => f.StoreId).ToArrayAsync();

        }

        [HttpPost]
        public async Task<bool> AddToFavorite([FromBody] Favorite favorite)
        {
            if (favorite == null) return false;

            try
            {
                string userId = _userIdentityService.GetUserId();
                _context.Favorites.Add(new Favorite
                {
                    UserId = userId,
                    StoreId = favorite.StoreId,

                });
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        [HttpDelete]
        public async Task<bool> DeleteFavorite([FromBody] Favorite favorite)
        {

            if (favorite == null) return false;
            try
            {
                string userId = _userIdentityService.GetUserId();
                var likeItem = await _context.Favorites.FirstOrDefaultAsync(c =>
                c.UserId == userId && c.StoreId == favorite.StoreId);
                if (likeItem == null) return false;

                _context.Favorites.Remove(likeItem);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        [HttpGet]
        public IEnumerable<StoreVM> GetStoreOnMap()
        {
            return  _context.Stores.Select(s=>new StoreVM { 
                StoreId = s.StoreId,
                StoreName = s.StoreName,
                Description = s.Description,
                LogoPath = s.LogoPath,
                City = s.City,
                Region = s.Region,
                Latitude= (decimal)s.Latitude,
                Longitude= (decimal)s.Longitude,
            });

        }
        public object FilterInMap()
        {
            return _context.Stores.Include(x => x.Orders).ThenInclude(x => x.Comment).Select(z => new StoreLocationVM
                {
                    StoreId = z.StoreId,
                    StoreName = z.StoreName,
                    Description = z.Description,
                    LogoPath = z.LogoPath,
                    City = z.City,
                    Region = z.Region,
                    Latitude=(decimal)z.Latitude==null?0:(decimal) z.Latitude,
                    Longitude= (decimal)z.Longitude==null?0: (decimal)z.Longitude,
                CommentCount = z.Orders.Where(x => x.Comment != null).Count(),
                CommentRank = z.Orders.Sum(x => x.Comment.CommentRank),
            }).ToList();
        }

    }
}
