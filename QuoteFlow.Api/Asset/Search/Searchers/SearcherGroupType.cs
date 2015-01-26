using System.ComponentModel.DataAnnotations;

namespace QuoteFlow.Api.Asset.Search.Searchers
{
    /// <summary>
    /// Represents the type of a <see cref="SearcherGroup"/>
    /// </summary>
    public enum SearcherGroupType
    {
        Text,
        Context,

        [Display(Name = "navigator.filter.subheading.catalogcomponents")]
        Catalog,

        [Display(Name = "navigator.filter.subheading.assetattributes")]
        Asset,

        [Display(Name = "navigator.filter.subheading.datesandtimes")]
        Date,

        [Display(Name = "navigator.filter.subheading.customfields")]
        Custom
    }
}