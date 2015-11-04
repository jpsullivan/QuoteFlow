using System.ComponentModel.DataAnnotations;

namespace QuoteFlow.Api.Models.ViewModels.Users.Manage
{
    public class ChangePassword
    {
        [Required]
        [Display(Name = "Old Password")]
        public string OldPassword { get; set; }

        [Required]
        [Display(Name = "New Password")]
        public string NewPassword { get; set; }

        [Required]
        [Display(Name = "Confirm Password")]
        public string NewPasswordConfirm { get; set; }
    }
}