using System.ComponentModel.DataAnnotations;

namespace SiFoodProjectFormal2._0.Areas.Users.Models.ViewModels
{
    public class ProductVM
    {
        public int ProductId { get; set; }

        public string ProductName { get; set; }
        [DisplayFormat(DataFormatString = "{0:0}")]
        public decimal UnitPrice { get; set; }

        public string StoreName { get; set; }

        public string Address { get; set; }

        public string Description { get; set; }

        public string PhotoPath { get; set; }

        public string SuggestPickUpTime { get; set; }

        public string SuggestPickEndTime { get; set; }

        public int ReleasedQty { get; set; }

        public int OrderedQty { get; set; }

    }
}
