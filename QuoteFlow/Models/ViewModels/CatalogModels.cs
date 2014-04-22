using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace QuoteFlow.Models.ViewModels
{
    public class CatalogShowModel
    {
        public List<Asset> Assets { get; set; }
        public Catalog Catalog { get; set; }
        public SubHeader SubHeader { get; set; }
    }

    public class CatalogHeader
    {
        public Catalog Catalog { get; set; }
        public SubHeader SubHeader { get; set; }
    }

    public class NewCatalogModel
    {
        [Required]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Display(Name = "Description")]
        public string Description { get; set; }

        [Display(Name = "Expirable")]
        public bool Expirable { get; set; }

        [Display(Name = "Expiration Date")]
        public DateTime? ExpirationDate { get; set; }

        [Display(Name = "Organization")]
        public Organization Organization { get; set; }
    }

    public class CatalogShowAssets
    {
        public List<Asset> Assets { get; set; }
        public Catalog Catalog { get; set; }
        public SubHeader SubHeader { get; set; }
    }
}
