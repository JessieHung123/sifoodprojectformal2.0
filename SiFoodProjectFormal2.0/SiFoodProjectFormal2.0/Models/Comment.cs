using System;
using System.Collections.Generic;

namespace SiFoodProjectFormal2._0.Models;

public partial class Comment
{
    public string OrderId { get; set; } = null!;

    public short CommentRank { get; set; }

    public DateTime CommentTime { get; set; }

    public string? Contents { get; set; }

    public virtual Order Order { get; set; } = null!;
}
