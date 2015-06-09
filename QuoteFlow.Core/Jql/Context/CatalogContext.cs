using System;
using QuoteFlow.Api.Jql.Context;

namespace QuoteFlow.Core.Jql.Context
{
    public class CatalogContext : ICatalogContext
    {
        public CatalogContext(int? catalogId)
        {
            if (catalogId == null)
            {
                throw new ArgumentNullException("catalogId");
            }

            CatalogId = catalogId;
        }

        public int? CatalogId { get; private set; }

        public bool IsAll()
        {
            return false;
        }

        private bool Equals(ICatalogContext other)
        {
            return CatalogId == other.CatalogId;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((CatalogContext) obj);
        }

        public override int GetHashCode()
        {
            return CatalogId.GetHashCode();
        }
    }
}