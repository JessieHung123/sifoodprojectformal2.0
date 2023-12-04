namespace SiFoodProjectFormal2._0.Areas.Users.Models.ViewModels
{
    public class HistoryOrderDetailsVM
    {
        public string OrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal? ShippingFee { get; set; }
        public int? TotalPrice { get; set; }
        public List<HistoryOrderDetailItemVM> Items { get; set; }
        public int? CommentRank { get; set; }
        public string? CommentContents { get; set; }
    }
}