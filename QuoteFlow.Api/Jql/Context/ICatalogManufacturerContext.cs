namespace QuoteFlow.Api.Jql.Context
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