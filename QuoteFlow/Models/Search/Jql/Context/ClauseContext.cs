using System.Collections.Generic;

namespace QuoteFlow.Models.Search.Jql.Context
{
    public class ClauseContext : IClauseContext
    {
        private static readonly ClauseContext GLOBAL_CONTEXT = new ClauseContext(new HashSet<ICatalogAssetTypeContext>{ CatalogAssetTypeContext.CreateGlobalContext() });

		private readonly HashSet<ICatalogAssetTypeContext> contexts;

		/// <returns> a <seealso cref="ClauseContext"/> containing a single
		/// <seealso cref="CatalogAssetTypeContext"/> which represents the All Catalogs/All Asset Types context.</returns>
		public static ClauseContext CreateGlobalClauseContext()
		{
			return GLOBAL_CONTEXT;
		}

		public ClauseContext() : this(new HashSet<ICatalogAssetTypeContext>())
		{
		}

        public ClauseContext(IEnumerable<ICatalogAssetTypeContext> contexts)
		{
			this.contexts = new HashSet<ICatalogAssetTypeContext>(contexts);
		}

        public virtual HashSet<ICatalogAssetTypeContext> Contexts
		{
			get
			{
				return contexts;
			}
		}

        public virtual bool ContainsGlobalContext()
		{
            return contexts.Contains(CatalogAssetTypeContext.CreateGlobalContext());
		}
    }
}