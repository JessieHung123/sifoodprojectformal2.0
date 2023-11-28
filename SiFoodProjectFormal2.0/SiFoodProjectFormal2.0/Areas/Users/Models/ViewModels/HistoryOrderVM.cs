namespace SiFoodProjectFormal2._0.Areas.Users.Models.ViewModels
{
    internal class HistoryOrderVM
    {
        public string StoreId { get; set; }
        public string OrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public string Status { get; set; }
        public int Quantity { get; set; }
        public int TotalPrice { get; set; }
    }
}