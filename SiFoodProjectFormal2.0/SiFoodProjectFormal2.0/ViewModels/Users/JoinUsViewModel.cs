using System.ComponentModel.DataAnnotations;

namespace SiFoodProjectFormal2._0.ViewModels.Users
{
    public class JoinUsViewModel
    {
        //[Required(ErrorMessage="店名未填寫")]
        [Display(Name ="平台顯示店名")]
        public string StoreName { get; set; }


        //[Required(ErrorMessage = "聯絡人姓名未填寫")]
        [Display(Name = "聯絡人姓名")]
        public string ContactName { get; set; }

        [EmailAddress(ErrorMessage = "Email格式錯誤")]
        //[Required(ErrorMessage = "Email未填寫")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        //[Required(ErrorMessage = "聯絡電話未填寫")]
        //[RegularExpression(@"^09\d{8}$", ErrorMessage = "無效的手機號碼格式")]
        [Display(Name = "聯絡電話")]
        public string Phone { get; set; }

        //[Required(ErrorMessage = "公司統編未填寫")]
       // [RegularExpression(@"^\d{8}$", ErrorMessage = "無效的公司統編格式")]
        [Display(Name = "公司統編")]
        public string TaxId { get; set; }

        //[Required(ErrorMessage = "店家所在城市未填寫")]
        //[RegularExpression("新北市|臺北市", ErrorMessage = "城市必須是新北市或臺北市")]
        [Display(Name = "店家地址-市")]
        public string City { get; set; }

       // [Required(ErrorMessage = "店家所在區未填寫")]
        [Display(Name = "店家地址-區")]
        public string Region { get; set; }

       // [Required(ErrorMessage = "店家地址未填寫")]
        [Display(Name = "店家地址")]
        public string Address { get; set; }

       // [Required(ErrorMessage = "店家介紹未填寫")]
        [Display(Name = "店家介紹")]
        public string Description { get; set; }

       // [Required(ErrorMessage = "營業時間未填寫")]
        [Display(Name = "營業時間")]
        public string OpeningTime { get; set; }

       // [Required(ErrorMessage = "營業日未填寫")]
        [Display(Name = "公休日")]
        public string ClosingDay { get; set; }

        [Display(Name = "LOGO")]
        public string? LogoPath { get; set; }

        [Display(Name = "店家照片")]
        public string? PhotosPath { get; set; }
    }
}