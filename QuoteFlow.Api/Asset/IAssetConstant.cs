using QuoteFlow.Api.Models;

namespace QuoteFlow.Api.Asset
{
    /// <summary>
    /// Abstraction to represent any of the various constants like <see cref="Manufacturer"/>.
    /// </summary>
    public interface IAssetConstant
    {
        int Id { get; set; }
        string Name { get; set; }
        string Description { get; set; }
        string IconUrl { get; set; }
        string IconUrlHtml { get; set; }
    }
}