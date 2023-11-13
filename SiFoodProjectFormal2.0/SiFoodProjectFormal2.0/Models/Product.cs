using System;
using System.Collections.Generic;

namespace SiFoodProjectFormal2._0.Models;

public partial class Product
{
    public int ProductId { get; set; }

    public string ProductName { get; set; } = null!;

    public string StoreId { get; set; } = null!;

    public int CategoryId { get; set; }

    public int ReleasedQty { get; set; }

    public int OrderedQty { get; set; }

    public string? PhotoPath { get; set; }

    public string? Description { get; set; }

    public DateTime RealeasedTime { get; set; }

    public decimal UnitPrice { get; set; }

    public TimeSpan SuggestPickUpTime { get; set; }

    public TimeSpan SuggestPickEndTime { get; set; }

    public virtual ICollection<Cart> Carts { get; set; } = new List<Cart>();

    public virtual Category Category { get; set; } = null!;

    public virtual Store Store { get; set; } = null!;
}
