using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace QuoteFlow.Models.ViewModels
{
    public class NewAssetModel
    {
        [Required]
        [Display(Name = "Catalog")]
        public int CatalogId { get; set; }
        public IEnumerable<SelectListItem> Catalogs { get; set; }
            
        [Required]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Display(Name = "Cost")]
        public decimal Cost { get; set; }

        [Display(Name = "Description")]
        public string Description { get; set; }

        [Display(Name = "Markup")]
        public decimal Markup { get; set; }

        [Display(Name = "Price")]
        public decimal Price { get; set; }

        [Display(Name = "Manufacturer")]
        public string Manufacturer { get; set; }

        [Display(Name = "Product Images")]
        public List<AssetImage> AssetImages { get; set; }

        [Required(ErrorMessage = "You must select an asset type.")]
        [Display(Name = "Asset Type")]
        public AssetType? AssetType{ get; set; }
        public IEnumerable<AssetType> AssetTypeChoices { get; set; }

    }

    public class AssetDetailsModel
    {
        public Asset Asset { get; set; }
        public Catalog Catalog { get; set; }
    }
}
