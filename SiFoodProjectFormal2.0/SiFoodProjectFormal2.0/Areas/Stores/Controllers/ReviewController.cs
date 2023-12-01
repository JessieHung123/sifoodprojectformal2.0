using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using SiFoodProjectFormal2._0.Areas.Stores.ViewModels;
using SiFoodProjectFormal2._0.Areas.Users.Models.ViewModels;
using SiFoodProjectFormal2._0.Models;

namespace SiFoodProjectFormal2._0.Areas.Stores.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ReviewController : ControllerBase
    {
        string targetStoreId = "S001";
        private readonly Sifood3Context _context;

        public ReviewController(Sifood3Context context)
        {
            _context = context;
        }

        public async Task<List<ReviewVM>> GetAll()
        {
            var review = _context.Orders.Include(p => p.Comment)
                .Include(p => p.User)
                .Where(e => e.StoreId == targetStoreId);
            return await review.Select(x => new ReviewVM
            {
                UserId = x.UserId,
                OrderId = x.OrderId,
                CommentRank = x.Comment.CommentRank,
                CommentTime = x.Comment.CommentTime,
                Contents = x.Comment.Contents,
                StoreId = x.StoreId,
                UserName =x.User.UserName
            }).ToListAsync();
        }
    }
}
