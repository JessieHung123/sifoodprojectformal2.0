using System;
using System.Collections.Generic;

namespace SiFoodProjectFormal2._0.Models;

public partial class Admin
{
    public int Id { get; set; }

    public string? Account { get; set; }

    public string? Password { get; set; }

    public string? Name { get; set; }
}
