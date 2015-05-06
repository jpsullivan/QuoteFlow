using QuoteFlow.Api.Models;

namespace QuoteFlow.Api.Infrastructure.Extensions
{
    public static class AssetExtensions
    {
        /// <summary>
        /// Simply converts an <see cref="IAsset"/> into a <see cref="AssetTableRow"/>.
        /// </summary>
        /// <param name="asset">The asset to convert</param>
        /// <returns></returns>
        public static AssetTableRow ToTableRow(this IAsset asset)
        {
            return new AssetTableRow
            {
                Id = asset.Id,
                Name = asset.Name,
                SKU = asset.SKU,
                Cost = asset.Cost
            };
        }
    }
}