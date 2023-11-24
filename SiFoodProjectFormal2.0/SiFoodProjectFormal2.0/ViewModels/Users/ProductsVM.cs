namespace SiFoodProjectFormal2._0.ViewModels.Users
{
    public class ProductsVM
    {
        public string ProductName { get; set; } = null!;

        public int CategoryId { get; set; }

        public string CategoryName { get; set; }

        public int avalibleQty { get; set; }

        public string? PhotoPath { get; set; }

        public string? SuggestPickUpTime { get; set; }

        public DateTime RealeasedTime { get; set; }

        public decimal UnitPrice { get; set; }

    }
}
