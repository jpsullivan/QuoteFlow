using System.ComponentModel.DataAnnotations;
using System.Web;

namespace QuoteFlow.Models.RequestModels
{
    public class ImportCatalogRequest
    {
        [Required]
        public HttpPostedFile CatalogFile { get; set; }
    }
}