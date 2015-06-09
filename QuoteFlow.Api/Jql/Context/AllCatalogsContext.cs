namespace QuoteFlow.Api.Jql.Context
{
    /// <summary>
    /// Represents the specicial case of all catalogs, in an unenumerated form.
    /// </summary>
    public class AllCatalogsContext : ICatalogContext
    {
        private static readonly AllCatalogsContext _instance = new AllCatalogsContext();

        public static AllCatalogsContext Instance
        {
            get { return _instance; }
        }

        private AllCatalogsContext()
        {
            // Don't create me.
        }

        public int? CatalogId
        {
            get { return null; }
        }

        public bool IsAll()
        {
            return true;
        }

        public override string ToString()
        {
            return "All Catalogs Context";
        }
    }
}