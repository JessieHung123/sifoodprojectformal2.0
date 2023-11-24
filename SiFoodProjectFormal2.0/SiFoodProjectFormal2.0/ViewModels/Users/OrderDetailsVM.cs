using SiFoodProjectFormal2._0.Models;

namespace SiFoodProjectFormal2._0.ViewModels.Users
{
    public class OrderDetailsVM
    {
        public string PhotoPath { get; set; } = null!;

        public string ProductName { get; set; } 

        public decimal UnitPrice { get; set; }

        public int Quantity { get; set; }

        public decimal Total { get; set; }
    }
}
