using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using QuoteFlow.Infrastructure.Helpers;
using QuoteFlow.Models.CatalogImport;

namespace QuoteFlow.Models.ViewModels
{
    public class CatalogShowModel
    {
        public IEnumerable<Asset> Assets { get; set; }
        public Catalog Catalog { get; set; }
        public User CatalogCreator { get; set; }
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

    public class CatalogShowAssetsModel
    {
        public IEnumerable<User> AssetCreators { get; set; } 
        public PagedList<Asset> Assets { get; set; }
        public Catalog Catalog { get; set; }
        public IEnumerable<Catalog> Catalogs { get; set; } 
        public User CatalogCreator { get; set; }
        public int CurrentPage { get; set; }
        public IEnumerable<Manufacturer> Manufacturers { get; set; }
        public string PaginationUrl { get; set; }
    }

    public class CatalogShowImportSummaryModel
    {
        public PagedList<CatalogImportSummaryRecord> Summary { get; set; }
        public Catalog Catalog { get; set; }
        public User CatalogCreator { get; set; }
        public int CurrentPage { get; set; }
        public string Filter { get; set; }
        public string PaginationUrl { get; set; }

        // total counts are strings so they can include commas
        public string TotalSuccess { get; set; }
        public string TotalSkipped { get; set; }
        public string TotalFailed { get; set; }
    }
}
