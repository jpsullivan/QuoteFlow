namespace QuoteFlow.Models.Search.Jql.Context
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
            ManufacturerContext = ManufacturerContext;
		}

		public virtual ICatalogContext CatalogContext { get; private set; }

        public IManufacturerContext ManufacturerContext { get; private set; }
    }
}