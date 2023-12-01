using SiFoodProjectFormal2._0.Models;

namespace SiFoodProjectFormal2._0.Areas.Stores.ViewModels
{
    public class ReviewVM
    {
        public string UserId { get; set; } = null!;

        public string OrderId { get; set; } = null!;

        public short CommentRank { get; set; }

        public DateTime CommentTime { get; set; }

        public string? Contents { get; set; }

        public string StoreId { get; set; } = null!;

        public virtual Order Order { get; set; } = null!;

        public virtual Store Store { get; set; } = null!;

        public string? UserName { get; set; }


    }
}
