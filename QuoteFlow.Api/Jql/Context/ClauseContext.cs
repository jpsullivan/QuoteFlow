using System.Collections.Generic;

namespace QuoteFlow.Api.Jql.Context
{
    public class ClauseContext : IClauseContext
    {
        private static readonly ClauseContext GlobalContext = new ClauseContext(new HashSet<ICatalogManufacturerContext>{ CatalogManufacturerContext.CreateGlobalContext() });

		private readonly ISet<ICatalogManufacturerContext> _contexts;

		/// <returns> a <seealso cref="ClauseContext"/> containing a single
		/// <seealso cref="CatalogManufacturerContext"/> which represents the All Catalogs/All Asset Types context.</returns>
		public static IClauseContext CreateGlobalClauseContext()
		{
			return GlobalContext;
		}

		public ClauseContext() : this(new HashSet<ICatalogManufacturerContext>())
		{
		}

        public ClauseContext(IEnumerable<ICatalogManufacturerContext> contexts)
		{
			_contexts = new HashSet<ICatalogManufacturerContext>(contexts);
		}

        public virtual ISet<ICatalogManufacturerContext> Contexts
		{
			get { return _contexts; }
		}

        public virtual bool ContainsGlobalContext()
		{
            return _contexts.Contains(CatalogManufacturerContext.CreateGlobalContext());
		}
    }
}