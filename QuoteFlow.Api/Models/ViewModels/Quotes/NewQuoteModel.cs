using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace QuoteFlow.Api.Models.ViewModels.Quotes
{
    public class NewQuoteModel
    {
        [Required]
        [Display(Name = "Name")]
        public string QuoteName { get; set; }

        [Display(Name = "Description")]
        public string QuoteDescription { get; set; }

        public Organization Organization { get; set; }

        [Required]
        [Display(Name = "Customer")]
        public int CustomerId { get; set; }

        /// <summary>
        /// For populating the contacts dropdown.
        /// </summary>
        public IEnumerable<Customer> Customers { get; set; }
    }
}