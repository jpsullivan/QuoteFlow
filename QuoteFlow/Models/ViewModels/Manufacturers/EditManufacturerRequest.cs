using System.ComponentModel.DataAnnotations;

namespace QuoteFlow.Models.ViewModels.Manufacturers
{
    public class EditManufacturerRequest
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Display(Name = "Description")]
        public string Description { get; set; }
    }
}