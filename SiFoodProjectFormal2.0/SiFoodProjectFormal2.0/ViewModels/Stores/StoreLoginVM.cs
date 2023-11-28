using System.ComponentModel.DataAnnotations;

namespace SiFoodProjectFormal2._0.ViewModels.Stores
{
    public class StoreLoginVM
    {
        [Required(ErrorMessage = "請輸入登入帳號")]
        public string? StoreAccount { get; set; }

        [Required(ErrorMessage = "請輸入登入密碼")]
        public byte[]? SetPassword { get; set; }
    }
}
