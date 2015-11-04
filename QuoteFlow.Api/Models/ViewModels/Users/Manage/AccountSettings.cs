using System.ComponentModel.DataAnnotations;

namespace QuoteFlow.Api.Models.ViewModels.Users.Manage
{
    /// <summary>
    /// 
    /// </summary>
    public class AccountSettings
    {
        [Display(Name = "Name")]
        public string FullName { get; set; }

        [Required]
        [Display(Name = "Email")]
        [StringLength(255)]
        [RegularExpression(RegisterViewModel.EmailValidationRegex, ErrorMessage = RegisterViewModel.EmailValidationErrorMessage)]
        public string EmailAddress { get; set; }
    }
}
