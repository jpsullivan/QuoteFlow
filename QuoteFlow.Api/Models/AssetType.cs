using System.ComponentModel;

namespace QuoteFlow.Api.Models
{
    public enum AssetType
    {
        [Description("Standard")]
        Standard,

        [Description("Kit (Multiple assets built-in)")]
        Kit
    }
}