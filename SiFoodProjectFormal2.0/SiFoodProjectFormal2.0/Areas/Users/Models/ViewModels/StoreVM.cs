using SiFoodProjectFormal2._0.Models;
using System.ComponentModel.DataAnnotations;

namespace SiFoodProjectFormal2._0.Areas.Users.Models.ViewModels
{
    public class StoreVM
    {
        [Key]
        public string StoreId { get; set; } = null!;

        public string StoreName { get; set; } = null!;

        public string Description { get; set; } = null!;
        public string? LogoPath { get; set; } = null!;

        public decimal? CommentRank { get; set; }

        public int CommentCount { get; set; } 

        public string CategoryName { get; set; } = null!;

        public string City { get; set; } = null!;

        public string Region { get; set; } = null!;

        public string WeekdayOpeningTime { get; set; } = null!;
        public string WeekdayClosingTime { get; set; } = null!;

        public string WeekendOpeningTime { get; set; } = null!;

        public string WeekendClosingTime { get; set; } = null!;


        public int? Inventory { get; set; }
    }
}
