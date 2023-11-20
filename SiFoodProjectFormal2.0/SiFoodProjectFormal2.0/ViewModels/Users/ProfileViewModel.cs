using System.ComponentModel.DataAnnotations;

namespace SiFoodProjectFormal2._0.ViewModels.Users
{
    public class ProfileViewModel
    {
        [Required(ErrorMessage = "請輸入用戶名")]
        [Display(Name = "用戶名")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "請輸入電子郵件")]
        [EmailAddress(ErrorMessage = "請輸入有效的電子郵件地址")]
        [Display(Name = "電子郵件")]
        public string UserEmail { get; set; }



        [Display(Name = "電話號碼")]
        public string UserPhone { get; set; }

        [Display(Name = "生日")]
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
