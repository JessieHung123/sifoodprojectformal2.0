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

        //[EnableQuery]
        //public async Task<IQueryable<StoreVM>> FilterBy()
        //{

        //    var storelocation = _context.Stores.Select(s => new StoreVM
        //    {
        //        StoreName = s.StoreName,
        //        Description = s.Description,
        //        LogoPath = s.LogoPath,
        //        CommentCount = _context.Orders.Where(o => o.StoreId == s.StoreId).Join(
        //        _context.Orders,
        //        store => store.StoreId,
        //        order => order.StoreId,
        //        (store, order) => order.OrderId
        //        )
        //        .Join(
        //            _context.Comments,
        //            orderId => orderId,
        //            comment => comment.OrderId,
        //            (orderId, comment) => comment
        //        )
        //        .Count(),
        //        CommentRank = _context.Orders.Where(o => o.StoreId == s.StoreId)

        //        .Join(
        //            _context.Comments,
        //            order => order.OrderId,
        //            comment => comment.OrderId,
        //            (order, comment) => (decimal)comment.CommentRank
        //        )
        //        .Average(),
        //        CategoryName = _context.Products.Where(p => p.StoreId == s.StoreId).Join(
        //            _context.Categories,
        //            product => product.CategoryId,
        //            category => category.CategoryId,
        //            (product, category) => category.CategoryName
        //        ).Single(),
        //        City = s.City,
        //        Region = s.Region,
        //        Inventory = _context.Products.Where(p => p.StoreId == s.StoreId).Select(p => p.ReleasedQty).Single() - _context.Products.Where(p => p.StoreId == s.StoreId).Select(p => p.OrderedQty).Single(),
        //    });

        //    return storelocation;
        //}

        //public bool GetBusinessTime()
        //{

        //    //現在時間
        //    //DateTime currentTime = DateTime.Now;
        //    DateTime currentTime = new DateTime(2013, 9, 14, 9, 28, 0);

        //    //_context

        //    foreach (var store in _context.Stores)
        //    {
        //        string openingTime = store.OpeningTime;
        //        if (!string.IsNullOrEmpty(openingTime))
        //        {
        //            string open = openingTime.ToString();
        //            string starttimeweekday = open.Substring(3, 5);
        //            string endtimeweekday = open.Substring(11, 5);
        //            string starttimeweekend = open.Substring(20, 5);
        //            string endtimeweekend = open.Substring(28, 5);

        //            // 店家營業時間
        //            TimeSpan[] weekdayBusinessHours = {
        //            TimeSpan.Parse(starttimeweekday),//10:00
        //            TimeSpan.Parse(endtimeweekday)//23:00
        //            };

        //            TimeSpan[] weekendBusinessHours = {
        //            TimeSpan.Parse(starttimeweekend),
        //            TimeSpan.Parse(endtimeweekend)
        //            };


        //            if (currentTime.DayOfWeek == DayOfWeek.Saturday || currentTime.DayOfWeek == DayOfWeek.Sunday)
        //            {

        //                bool isBusinessHours = IsBusinessHours(currentTime, weekendBusinessHours);
        //                return isBusinessHours;
        //            }
        //            else
        //            {

        //                bool isBusinessHours = IsBusinessHours(currentTime, weekdayBusinessHours);
        //                return isBusinessHours;
        //            }


        //        }

        //    }
        //    return false;

        //}

        //public bool IsBusinessHours(DateTime currentTime, TimeSpan[] businessHours)
        //{

        //    if (currentTime.DayOfWeek == DayOfWeek.Saturday || currentTime.DayOfWeek == DayOfWeek.Sunday)
        //    {
        //        TimeSpan startTime = businessHours[0];
        //        TimeSpan endTime = businessHours[1];
        //        return currentTime.TimeOfDay >= startTime && currentTime.TimeOfDay <= endTime;
        //    }
        //    else
        //    {

        //        TimeSpan startTime = businessHours[0];
        //        TimeSpan endTime = businessHours[1];

        //        return currentTime.TimeOfDay >= startTime && currentTime.TimeOfDay <= endTime;
        //    }
        //}

        //[EnableQuery]
        //public async Task<IQueryable<StoreVM>> FilterIsOpen()
        //{
        //    Sifood3Context context = new Sifood3Context();
        //    if (GetBusinessTime())
        //    {
        //        var storeopen = _context.Stores.Select(s => new StoreVM
        //        {
        //            StoreName = s.StoreName,
        //            Description = s.Description,
        //            LogoPath = s.LogoPath,
        //            CommentCount = _context.Orders.Where(o => o.StoreId == s.StoreId).Join(
        //        _context.Orders,
        //        store => store.StoreId,
        //        order => order.StoreId,
        //        (store, order) => order.OrderId
        //        )
        //        .Join(
        //            _context.Comments,
        //            orderId => orderId,
        //            comment => comment.OrderId,
        //            (orderId, comment) => comment
        //        )
        //        .Count(),
        //            CommentRank = _context.Orders.Where(o => o.StoreId == s.StoreId)

        //        .Join(
        //            _context.Comments,
        //            order => order.OrderId,
        //            comment => comment.OrderId,
        //            (order, comment) => (decimal)comment.CommentRank
        //        )
        //        .Average(),
        //            CategoryName = _context.Products.Where(p => p.StoreId == s.StoreId).Join(
        //            _context.Categories,
        //            product => product.CategoryId,
        //            category => category.CategoryId,
        //            (product, category) => category.CategoryName
        //        ).Single(),
        //            City = s.City,
        //            Region = s.Region,
        //            //OpeningTime="營業中",
        //        });

        //        return storeopen;
        //    }
        //    else
        //    {
        //        var storeopen = _context.Stores.Select(s => new StoreVM
        //        {
        //            StoreName = s.StoreName,
        //            Description = s.Description,
        //            LogoPath = s.LogoPath,
        //            CommentCount = _context.Orders.Where(o => o.StoreId == s.StoreId).Join(
        //        _context.Orders,
        //        store => store.StoreId,
        //        order => order.StoreId,
        //        (store, order) => order.OrderId
        //        )
        //        .Join(
        //            _context.Comments,
        //            orderId => orderId,
        //            comment => comment.OrderId,
        //            (orderId, comment) => comment
        //        )
        //        .Count(),
        //            CommentRank = _context.Orders.Where(o => o.StoreId == s.StoreId)

        //        .Join(
        //            _context.Comments,
        //            order => order.OrderId,
        //            comment => comment.OrderId,
        //            (order, comment) => (decimal)comment.CommentRank
        //        )
        //        .Average(),
        //            CategoryName = _context.Products.Where(p => p.StoreId == s.StoreId).Join(
        //            _context.Categories,
        //            product => product.CategoryId,
        //            category => category.CategoryId,
        //            (product, category) => category.CategoryName
        //        ).Single(),
        //            City = s.City,
        //            Region = s.Region,
        //            //OpeningTime = "未營業",
        //        });

        //        return storeopen;
        //    }

        //}


    }
}
