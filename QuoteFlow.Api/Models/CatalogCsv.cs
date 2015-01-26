using System.Collections.Generic;

namespace QuoteFlow.Api.Models
{
    public class CatalogCsv
    {
        public List<SelectListItem> Headers { get; set; }

        public List<string[]> Rows { get; set; }
    }
}