using System.ComponentModel.DataAnnotations;

namespace SiFoodProjectFormal2._0.Areas.Users.Models.ViewModels
{
    public class ProfileVM
    {

        public string UserId { get; set; } = null!;

        // [Required(ErrorMessage = "請輸入您的姓名")]
        [Display(Name = "姓名")]
        public string UserName { get; set; }

        // [Required(ErrorMessage = "電子郵件未填寫")]
        // [EmailAddress(ErrorMessage = "請輸入有效的電子郵件地址")]
        [Display(Name = "Email")]
        public string UserEmail { get; set; }


        // [Required(ErrorMessage = "聯絡電話未填寫")]
        //[RegularExpression(@"^09\d{8}$", ErrorMessage = "無效的手機號碼格式")]
        [Display(Name = "電話號碼")]
        public string UserPhone { get; set; }

        // [Required(ErrorMessage = "生日未填寫")]
        //[Display(Name = "生日")]
        [DataType(DataType.Date)]
        public DateTime? UserBirthDate { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "當前密碼")]
        public string CurrentPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "新密碼")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "確認新密碼")]
        [Compare("NewPassword", ErrorMessage = "新密碼和確認密碼不匹配")]
        public string ConfirmPassword { get; set; }
    }
}
