using System.Collections.Generic;
using System.Web.Mvc;

namespace QuoteFlow.Models
{
    public class CatalogCsv
    {
        public List<SelectListItem> Headers { get; set; }

        public List<string[]> Rows { get; set; }
    }
}