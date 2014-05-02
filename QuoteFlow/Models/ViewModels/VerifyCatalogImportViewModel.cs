using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
        /// The catalog manifest records for previewing purposes. 
        /// Should be capped at 100.
        /// </summary>
        public IEnumerable<string[]> Rows { get; set; }

        /// <summary>
        /// The total number of records that a catalog manifest
        /// contains.
        /// </summary>
        public int TotalRows { get; set; }

        [Required]
        [Display(Name = "Asset Name")]
        public int AssetNameHeaderId { get; set; }

        [Required]
        [Display(Name = "Manufacturer")]
        public int ManufacturerHeaderId { get; set; }

        [Required]
        [Display(Name = "SKU")]
        public int SkuHeaderId { get; set; }

        [Display(Name = "Description")]
        public int DescriptionHeaderId { get; set; }

        [Required]
        [Display(Name = "Cost")]
        public int CostHeaderId { get; set; }

        [Required]
        [Display(Name = "Markup")]
        public int MarkupHeaderId { get; set; }


    }
}