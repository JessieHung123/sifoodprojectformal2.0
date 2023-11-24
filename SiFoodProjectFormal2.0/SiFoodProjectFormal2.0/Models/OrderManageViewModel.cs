
namespace SiFoodProjectFormal2._0.Models
{
    public class OrderManageViewModel
    {
        
        public string? OrderAddress { get; set; }
        public string? OrderId { get; set; } 
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string? UserName { get; set; }
        public int Quantity { get; set; }
        public string? UserPhone { get; set; }
        public int OrderDetailId { get; set; }
        public ICollection<UserAddress>? UserAddress { get; internal set; }
        public DateTime OrderDate { get; set; }       
        public string? StatusName { get; internal set; }
        public decimal ProductUnitPrice { get; internal set; }
        public string StoreName { get; internal set; }
        public string StorePhone { get; internal set; }
        public string StoreAddress { get; internal set; }
    }
}
