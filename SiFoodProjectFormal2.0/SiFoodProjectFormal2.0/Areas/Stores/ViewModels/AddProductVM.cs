namespace SiFoodProjectFormal2._0.Areas.Stores.ViewModels
{
    public class AddProductVM
    {
        public int ProductId { get; set; }

        public string ProductName { get; set; } = null!;

        public int CategoryId { get; set; }

        public int ReleasedQty { get; set; }

        public string? Description { get; set; }

        public DateTime RealeasedTime { get; set; }

        public TimeSpan SuggestPickUpTime { get; set; }

        public TimeSpan SuggestPickEndTime { get; set; }

        public decimal UnitPrice { get; set; }

        public IFormFile ImageFile { get; set; }


    }
}
