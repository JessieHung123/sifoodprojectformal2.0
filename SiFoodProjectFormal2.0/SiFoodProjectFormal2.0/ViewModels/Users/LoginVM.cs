using System.ComponentModel.DataAnnotations;

namespace SiFoodProjectFormal2._0.ViewModels.Users
{
    public class LoginVM
    {
        [Required(ErrorMessage = "請輸入登入帳號")]
        public string? Account {  get; set; }

        [Required(ErrorMessage = "請輸入登入密碼")]
        public byte[]? Password { get; set; }
    }
}
