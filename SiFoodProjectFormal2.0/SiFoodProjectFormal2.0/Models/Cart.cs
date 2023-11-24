﻿using System;
using System.Collections.Generic;

namespace SiFoodProjectFormal2._0.Models;

public partial class Cart
{
    public string UserId { get; set; } = null!;

    public int ProductId { get; set; }

    public int Quantity { get; set; }

    public string StoreId { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;

    public virtual Store Store { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
