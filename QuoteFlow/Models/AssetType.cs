using System.ComponentModel;

namespace QuoteFlow.Models
{
    public enum AssetType
    {
        [Description("Standard")]
        Standard,

        [Description("Kit (Multiple assets built-in)")]
        Kit
    }
}