namespace QuoteFlow.Api.Jql.Context
{
    public class CatalogManufacturerContext : ICatalogManufacturerContext
    {
        private static readonly ICatalogManufacturerContext Instance = new CatalogManufacturerContext(AllCatalogsContext.Instance, AllManufacturersContext.Instance);

		public static ICatalogManufacturerContext CreateGlobalContext()
		{
			return Instance;
		}

        public CatalogManufacturerContext(ICatalogContext catalogContext, IManufacturerContext manufacturerContext)
		{
            CatalogContext = catalogContext;
            ManufacturerContext = manufacturerContext;
		}

		public virtual ICatalogContext CatalogContext { get; private set; }

        public IManufacturerContext ManufacturerContext { get; private set; }
    }
}