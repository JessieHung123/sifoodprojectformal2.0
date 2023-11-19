using System;
using System.Collections.Generic;

namespace SiFoodProjectFormal2._0.Models;

public partial class Order
{
    public string OrderId { get; set; } = null!;

    public DateTime OrderDate { get; set; }

    public string Address { get; set; } = null!;

    public string StoreId { get; set; } = null!;

    public string DriverId { get; set; } = null!;

    public string UserId { get; set; } = null!;

    public string DeliveryMethod { get; set; } = null!;

    public decimal ShippingFee { get; set; }

    public int StatusId { get; set; }

    public virtual Comment? Comment { get; set; }

    public virtual Driver Driver { get; set; } = null!;

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

    public virtual Payment? Payment { get; set; }

    public virtual Status Status { get; set; } = null!;

    public virtual Store Store { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
