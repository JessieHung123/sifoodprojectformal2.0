using System;
using System.Collections.Generic;

namespace SiFoodProjectFormal2._0.Models;

public partial class OrderDetail
{
    public int OrderDetailId { get; set; }

    public string OrderId { get; set; } = null!;

    public int ProductId { get; set; }

    public int Quantity { get; set; }

    public int StatusId { get; set; }

    public virtual Order Order { get; set; } = null!;

    public virtual Status Status { get; set; } = null!;
}
