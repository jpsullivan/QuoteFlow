using System.Collections.Generic;
using System.Web.Mvc;

namespace QuoteFlow.Models.ViewModels
{
    public class VerifyCatalogImportViewModel
    {
        /// <summary>
        /// The catalog manifest headers.
        /// </summary>
        public IEnumerable<SelectListItem> Headers { get; set; }

        /// <summary>
        /// The catalog manifest records.
        /// </summary>
        public IEnumerable<string[]> Rows { get; set; }



        public string PriceHeader { get; set; }
    }
}