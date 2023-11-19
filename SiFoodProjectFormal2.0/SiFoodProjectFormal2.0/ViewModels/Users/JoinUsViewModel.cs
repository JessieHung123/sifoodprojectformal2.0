using System.ComponentModel.DataAnnotations;

namespace SiFoodProjectFormal2._0.ViewModels.Users
{
    public class JoinUsViewModel
    {
        [Required(ErrorMessage="店名未填寫")]
        [Display(Name ="店名")]
        public string StoreName { get; set; }

        [EmailAddress(ErrorMessage ="Email格式錯誤")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        public string Phone { get; set; }

    }
}