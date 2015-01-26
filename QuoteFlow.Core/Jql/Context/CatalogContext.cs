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
    }
}