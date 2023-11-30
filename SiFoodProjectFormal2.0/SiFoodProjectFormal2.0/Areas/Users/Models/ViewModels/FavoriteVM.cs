using SiFoodProjectFormal2._0.Models;

namespace SiFoodProjectFormal2._0.Areas.Users.Models.ViewModels
{
    public class FavoriteVM
    {
        public string UserId { get; set; } = null!;

        public string StoreId { get; set; } = null!;

        public int FavoriteId { get; set; }
    }
}
