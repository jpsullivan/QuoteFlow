namespace QuoteFlow.Models.Search.Jql.Context
{
    /// <summary>
    /// Represents the specicial case of all catalogs, in an unenumerated form.
    /// </summary>
    public class AllCatalogsContext : ICatalogContext
    {
        public static readonly AllCatalogsContext INSTANCE = new AllCatalogsContext();

        public virtual AllCatalogsContext Instance
        {
            get { return INSTANCE; }
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
            return "All Projects Context";
        }
    }

}