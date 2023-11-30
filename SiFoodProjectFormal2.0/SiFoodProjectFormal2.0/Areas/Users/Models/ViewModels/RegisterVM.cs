using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SiFoodProjectFormal2._0.Areas.Users.Models.ViewModels
{
    public class RegisterVM
    {
        [Required(ErrorMessage = "請輸入欲註冊帳號")]
        public string? EmailAccount { get; set; }

        [Required(ErrorMessage = "請輸入欲註冊密碼")]
        public string? Password { get; set; }
    }
}