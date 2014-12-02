using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Wintellect.PowerCollections;

namespace QuoteFlow.Models.Search.Jql.Context
{
    public class QueryContext : IQueryContext
    {
        public IEnumerable<QueryContextCatalogManufacturerContexts> CatalogManufacturerContexts { get; set; }

        public QueryContext(IClauseContext clauseContext)
        {
            CatalogManufacturerContexts = Init(clauseContext);
        }

        private static IEnumerable<QueryContextCatalogManufacturerContexts> Init(IClauseContext clauseContext)
        {
            //var contextSetMap = new Multimap<ICatalogContext, IManufacturerContext>();
            var contextSetMap = new MultiDictionary<ICatalogContext, IManufacturerContext>(true);

            ISet<ICatalogManufacturerContext> contexts = clauseContext.Contexts;
            foreach (ICatalogManufacturerContext context in contexts)
            {
                contextSetMap.Add(context.CatalogContext, context.ManufacturerContext);
            }

            IList<QueryContextCatalogManufacturerContexts> ctxs = new List<QueryContextCatalogManufacturerContexts>(contextSetMap.Count());
            foreach (var entry in contextSetMap.ToImmutableHashSet())
            {
                ctxs.Add(new QueryContextCatalogManufacturerContexts(entry.Key, entry.Value.ToList()));
            }
            return ctxs;
        }
    }
}