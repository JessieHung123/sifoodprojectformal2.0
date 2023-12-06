using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using SiFoodProjectFormal2._0.Areas.Stores.ViewModels;
using SiFoodProjectFormal2._0.Areas.Users.Models.ViewModels;
using SiFoodProjectFormal2._0.Models;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace SiFoodProjectFormal2._0.Areas.Stores.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ReviewapiController : ControllerBase
    {
        string targetStoreId = "S001";
        private readonly Sifood3Context _context;



        public ReviewapiController(Sifood3Context context)
        {
            _context = context;

        }

        //public async Task<List<ReviewVM>> GetAll()
        //{
        //    var review = _context.Orders.Include(p => p.Comment)
        //        .Include(p => p.User)
        //        .Where(x => x.Comment != null)
        //        .Where(e => e.StoreId == targetStoreId);

        //    return await review
        //        .Select(x => new ReviewVM
        //    {
        //        UserId = x.UserId,
        //        OrderId = x.OrderId,
        //        CommentRank = x.Comment.CommentRank,
        //        CommentTime = x.Comment.CommentTime,
        //        Contents = x.Comment.Contents,
        //        StoreId = x.StoreId,
        //        UserName = x.User.UserName
        //    }).ToListAsync();
        //}

        public async Task<List<ReviewVM>> Filter(string? text)
        {
            var review = _context.Orders.Include(p => p.Comment)
                .Include(p => p.User)
                .Where(x => x.Comment != null)
                .Where(e => e.StoreId == targetStoreId);

            if (!string.IsNullOrEmpty(text))
            {
                review = review.Where(e => e.User.UserName.Contains(text) || e.Comment.Contents.Contains(text));
            }
            return await review.Select(x => new ReviewVM
                {
                    UserId = x.UserId,
                    OrderId = x.OrderId,
                    CommentRank = x.Comment.CommentRank,
                    CommentTime = x.Comment.CommentTime,
                    Contents = x.Comment.Contents,
                    StoreId = x.StoreId,
                    UserName = x.User.UserName
                }).ToListAsync();
        }

        public async Task<List<ReviewVM>> SelectStars(int? num)
        {
            var review = _context.Orders.Include(p => p.Comment)
                .Include(p => p.User)
                .Where(x => x.Comment != null)
                .Where(e => e.StoreId == targetStoreId);
            if (num != 0)
            {
                review = review.Where(e => e.Comment.CommentRank == num);
            }
            return await review.Select(x => new ReviewVM
            {
                UserId = x.UserId,
                OrderId = x.OrderId,
                CommentRank = x.Comment.CommentRank,
                CommentTime = x.Comment.CommentTime,
                Contents = x.Comment.Contents,
                StoreId = x.StoreId,
                UserName = x.User.UserName
            }).ToListAsync();
        }

    }
}
