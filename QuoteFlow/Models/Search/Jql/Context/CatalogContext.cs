using System;

namespace QuoteFlow.Models.Search.Jql.Context
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