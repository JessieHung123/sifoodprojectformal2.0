using SiFoodProjectFormal2._0.Models;
using SiFoodProjectFormal2._0.ViewModels.Users;

namespace SiFoodProjectFormal2._0.DTO
{
    public class OrderDTO
    {
        public string OrderId { get; set; } = null!;

        public DateTime OrderDate { get; set; }

        public string Address { get; set; } = null!;


        public string Status { get; set; } = null!;

        public string UserName { get; set; } = null!;

        public string UserEmail { get; set; } = null!;

        public string UserPhone { get; set; } = null!;

        public string PaymentMethodＮame { get; set; } = null!;

        public string PaymentTime { get; set; } = null!;

        public IEnumerable<OrderDetailsVM> OrderDetails { get; set; }

        public string DriverFullName { get; set; } = null!;

        public decimal Subtotal { get; set; }

        public int TotalQuantity { get; set; }

    }
}
