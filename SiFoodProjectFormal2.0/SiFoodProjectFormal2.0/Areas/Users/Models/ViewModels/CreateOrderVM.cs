using SiFoodProjectFormal2._0.Models;

namespace SiFoodProjectFormal2._0.Areas.Users.Models.ViewModels
{
    public class CreateOrderVM
    { 

        public string? StoreName { get; set; }

        public string? UserName { get; set;}

        public IEnumerable<Product> ProductName { get; set; }

        public int Quantity { get; set; }

        public int TotalPrice { get; set;}

    }
}
