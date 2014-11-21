using System.Collections.Generic;

namespace QuoteFlow.Models.Search.Jql.Context
{
    public class ClauseContext : IClauseContext
    {
        private static readonly ClauseContext GlobalContext = new ClauseContext(new HashSet<ICatalogAssetTypeContext>{ CatalogAssetTypeContext.CreateGlobalContext() });

		private readonly HashSet<ICatalogAssetTypeContext> _contexts;

		/// <returns> a <seealso cref="ClauseContext"/> containing a single
		/// <seealso cref="CatalogAssetTypeContext"/> which represents the All Catalogs/All Asset Types context.</returns>
		public static ClauseContext CreateGlobalClauseContext()
		{
			return GlobalContext;
		}

		public ClauseContext() : this(new HashSet<ICatalogAssetTypeContext>())
		{
		}

        public ClauseContext(IEnumerable<ICatalogAssetTypeContext> contexts)
		{
			_contexts = new HashSet<ICatalogAssetTypeContext>(contexts);
		}

        public virtual ISet<ICatalogAssetTypeContext> Contexts
		{
			get { return _contexts; }
		}

        public virtual bool ContainsGlobalContext()
		{
            return _contexts.Contains(CatalogAssetTypeContext.CreateGlobalContext());
		}
    }
}