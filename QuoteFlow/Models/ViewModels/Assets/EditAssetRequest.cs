using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace QuoteFlow.Models.ViewModels.Assets
{
    public class EditAssetRequest
    {
        [Required]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Display(Name = "Description")]
        public string Description { get; set; }

        [Required]
        [Display(Name = "SKU")]
        public string SKU { get; set; }

        [Display(Name = "Cost ($)")]
        public decimal Cost { get; set; }

        [Display(Name = "Markup")]
        public decimal Markup { get; set; }

        [Display(Name = "Manufacturer")]
        public int ManufacturerId { get; set; }

        public IEnumerable<AssetVar> AssetVars { get; set; }

        // non-modifiable fields
        public int Id { get; set; }
        public IEnumerable<SelectListItem> Manufacturers { get; set; }
        public IEnumerable<SelectListItem> AssetVarNames { get; set; }

        public Dictionary<int, object> AssetVarsData { get; set; }
        public Dictionary<int, object> AssetVarValuesData { get; set; }
    }
}