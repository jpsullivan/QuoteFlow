using System;

namespace QuoteFlow.Models.Search.Jql.Context
{
    public class CatalogContext : ICatalogContext
    {
        public CatalogContext(int? catalogId)
        {
            CatalogId = catalogId;
        }

        public int? CatalogId { get; private set; }

        public bool IsAll()
        {
            throw new NotImplementedException();
        }
    }
}