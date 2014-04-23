using System.ComponentModel.DataAnnotations;

namespace QuoteFlow.Models.ViewModels
{
    public class NewQuoteModel
    {
        [Required]
        [Display(Name = "Quote Name")]
        public string Name { get; set; }

        [Display(Name = "Description")]
        public string Description { get; set; }

        public Organization Organization { get; set; }
    }
}