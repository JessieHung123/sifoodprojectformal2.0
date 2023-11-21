using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SiFoodProjectFormal2._0.ViewModels.Users
{
    public class RegisterVM
    {
        public string? UserId { get; set; }
        
        [Required(ErrorMessage = "請輸入註冊帳號")]
        public string? EmailAccount { get; set; }

        [Required(ErrorMessage = "請輸入註冊密碼")]
        public byte[]? Password { get; set;}


    }
}
