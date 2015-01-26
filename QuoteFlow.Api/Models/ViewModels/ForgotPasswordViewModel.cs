using System.ComponentModel.DataAnnotations;

namespace QuoteFlow.Api.Models.ViewModels
{
    public class ForgotPasswordViewModel
    {
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }
}