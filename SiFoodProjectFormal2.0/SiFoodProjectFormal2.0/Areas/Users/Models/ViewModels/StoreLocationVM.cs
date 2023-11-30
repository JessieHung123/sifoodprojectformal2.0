using System.ComponentModel.DataAnnotations;

namespace SiFoodProjectFormal2._0.Areas.Users.Models.ViewModels
{
    public class StoreLocationVM
    {
        
        public string StoreId { get; set; } = null!;

        public string StoreName { get; set; } = null!;

        public string Description { get; set; } = null!;
        public string? LogoPath { get; set; }
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public decimal? CommentRank { get; set; }

        public int CommentCount { get; set; }

        public string City { get; set; } = null!;

        public string Region { get; set; } = null!;

        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
    }
}
