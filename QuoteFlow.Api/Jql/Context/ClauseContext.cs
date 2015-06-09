using System.Collections.Generic;

namespace QuoteFlow.Api.Jql.Context
{
    public class ClauseContext : IClauseContext
    {
        private static readonly ClauseContext GlobalContext = new ClauseContext(new HashSet<ICatalogManufacturerContext>{ CatalogManufacturerContext.CreateGlobalContext() });

		private readonly ISet<ICatalogManufacturerContext> _contexts;

		/// <returns> a <see cref="ClauseContext"/> containing a single
		/// <see cref="CatalogManufacturerContext"/> which represents the All Catalogs/All Manufacturers context.</returns>
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

        protected bool Equals(ClauseContext other)
        {
            return Equals(_contexts, other._contexts);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ClauseContext) obj);
        }

        public override int GetHashCode()
        {
            return (_contexts != null ? _contexts.GetHashCode() : 0);
        }
    }
}