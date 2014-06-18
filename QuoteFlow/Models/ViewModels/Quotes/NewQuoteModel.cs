using System.ComponentModel.DataAnnotations;

namespace QuoteFlow.Models.ViewModels.Quotes
{
    public class NewQuoteModel
    {
        [Required]
        [Display(Name = "Name")]
        public string QuoteName { get; set; }

        [Display(Name = "Description")]
        public string QuoteDescription { get; set; }

        public Organization Organization { get; set; }
    }
}