using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace QuoteFlow.Models.ViewModels
{
    [Serializable]
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

        /// <summary>
        /// The total number of records that a catalog manifest
        /// contains.
        /// </summary>
        public int TotalRows { get; set; }

        public NewCatalogModel CatalogInformation { get; set; }

        /// <summary>
        /// The model that contains the data received in the first
        /// step of catalog manifest imports.
        /// </summary>
        public PrimaryCatalogFieldsViewModel PrimaryCatalogFields { get; set; }

        /// <summary>
        /// The model that contains the data received from the second
        /// step of the catalog import process.
        /// </summary>
        public SecondaryCatalogFieldsViewModel SecondaryCatalogFields { get; set; }
    }

    [Serializable]
    public class PrimaryCatalogFieldsViewModel
    {
        [Required]
        [Display(Name = "Catalog Name")]
        public string CatalogName { get; set; }

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

    [Serializable]
    public class SecondaryCatalogFieldsViewModel
    {
        public SecondaryCatalogFieldsViewModel() { }

        public SecondaryCatalogFieldsViewModel(List<OptionalImportField> optionalFields)
        {
            OptionalImportFields = optionalFields;
        }

        public List<OptionalImportField> OptionalImportFields { get; set; }
    }
}