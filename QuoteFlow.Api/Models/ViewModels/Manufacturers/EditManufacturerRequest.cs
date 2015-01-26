using System.ComponentModel.DataAnnotations;
using System.Web;

namespace QuoteFlow.Api.Models.ViewModels.Manufacturers
{
    public class EditManufacturerRequest
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Display(Name = "Description")]
        public string Description { get; set; }

        public HttpPostedFileBase ManufacturerLogo { get; set; }
    }
}