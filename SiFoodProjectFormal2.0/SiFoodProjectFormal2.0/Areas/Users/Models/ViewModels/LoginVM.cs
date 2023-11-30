using System.ComponentModel.DataAnnotations;

namespace SiFoodProjectFormal2._0.Areas.Users.Models.ViewModels
{
    public class LoginVM
    {
        [Required(ErrorMessage = "請輸入登入帳號")]
        public string? Account { get; set; }

        [Required(ErrorMessage = "請輸入登入密碼")]
        public string? Password { get; set; }
    }
}
