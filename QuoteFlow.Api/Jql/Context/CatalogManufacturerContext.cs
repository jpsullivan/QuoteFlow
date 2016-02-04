namespace QuoteFlow.Api.Jql.Context
{
    public class CatalogManufacturerContext : ICatalogManufacturerContext
    {
        private static readonly ICatalogManufacturerContext Instance = new CatalogManufacturerContext(AllCatalogsContext.Instance, AllManufacturersContext.Instance);

        public virtual ICatalogContext CatalogContext { get; private set; }
        public IManufacturerContext ManufacturerContext { get; private set; }

		public static ICatalogManufacturerContext CreateGlobalContext()
		{
			return Instance;
		}

        public CatalogManufacturerContext(ICatalogContext catalogContext, IManufacturerContext manufacturerContext)
		{
            CatalogContext = catalogContext;
            ManufacturerContext = manufacturerContext;
		}

        protected bool Equals(CatalogManufacturerContext other)
        {
            return Equals(CatalogContext, other.CatalogContext) && Equals(ManufacturerContext, other.ManufacturerContext);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((CatalogManufacturerContext) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((CatalogContext != null ? CatalogContext.GetHashCode() : 0)*397) ^ (ManufacturerContext != null ? ManufacturerContext.GetHashCode() : 0);
            }
        }

        public override string ToString()
        {
            return $"CatalogContext: {CatalogContext}, ManufacturerContext: {ManufacturerContext}";
        }
    }
}