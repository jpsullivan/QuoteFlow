namespace QuoteFlow.Models.Search.Jql.Context
{
    public class CatalogAssetTypeContext : ICatalogAssetTypeContext
    {
        private static readonly ICatalogAssetTypeContext INSTANCE = new CatalogAssetTypeContext(AllCatalogsContext.INSTANCE);

		private readonly ICatalogContext catalogContext;
		//private readonly IssueTypeContext issueTypeContext;

		public static ICatalogAssetTypeContext CreateGlobalContext()
		{
			return INSTANCE;
		}

        public CatalogAssetTypeContext(ICatalogContext catalogContext)
		{
			this.catalogContext = catalogContext;
		}

		public virtual ICatalogContext CatalogContext
		{
			get
			{
				return catalogContext;
			}
		}
    }
}