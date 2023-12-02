using SiFoodProjectFormal2._0.Areas.Users.Models.ViewModels;

namespace SiFoodProjectFormal2._0.Areas.Drivers.NewFolder3
{
    public class DeliveryOrderVM
    {
        public string OrderId { get; set; } = null!;

        public string? Address { get; set; }
        public string? StoreAddress { get; set; }
        public string OrderDate { get; set; }
        public string StoreId { get; set; } = null!;

        public string StoreName { get; set; } = null!;

        public string? DriverId { get; set; }

        public string UserId { get; set; } = null!;

        public string UserName { get; set; } = null!;
        public string UserPhone { get; set; } = null!;

        public int StatusId { get; set; }

        public IEnumerable<OrderDetailsVM> OrderDetails { get; set; }

        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }

    }
}
