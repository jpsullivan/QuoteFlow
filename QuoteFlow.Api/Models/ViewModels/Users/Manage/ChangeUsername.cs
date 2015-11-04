using System.ComponentModel.DataAnnotations;

namespace QuoteFlow.Api.Models.ViewModels.Users.Manage
{
    public class ChangeUsername
    {
        [Required]
        public string Username { get; set; }
    }
}