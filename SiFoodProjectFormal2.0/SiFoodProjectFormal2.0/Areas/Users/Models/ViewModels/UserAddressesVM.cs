using SiFoodProjectFormal2._0.Models;

namespace SiFoodProjectFormal2._0.Areas.Users.Models.ViewModels
{
    public class UserAddressesVM
    {
        public int UserAddressId { get; set; }

        public string UserCity { get; set; } = null!;

        public string UserId { get; set; } = null!;

        public string UserRegion { get; set; } = null!;

        public string UserDetailAddress { get; set; } = null!;

        public string? UserPhone { get; set; }
    }
}
