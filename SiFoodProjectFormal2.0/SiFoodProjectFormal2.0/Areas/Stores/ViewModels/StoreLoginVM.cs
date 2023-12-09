using System.ComponentModel.DataAnnotations;

namespace SiFoodProjectFormal2._0.Areas.Stores.ViewModels
{
    public class StoreLoginVM
    {
        [Required(ErrorMessage = "請輸入登入帳號")]
        public string? StoreAccount { get; set; }

        [Required(ErrorMessage = "請輸入登入密碼")]
        public string? SetPassword { get; set; }
    }
}
