namespace QuoteFlow.Models.Search.Jql.Context
{
    /// <summary>
    /// Specifies a catalog to manufacturer context.
    /// </summary>
    public interface ICatalogManufacturerContext
    {
        ICatalogContext CatalogContext { get; }

        IManufacturerContext ManufacturerContext { get; }
    }
}