using System.Collections.Generic;
using Wintellect.PowerCollections;

namespace QuoteFlow.Models.Search.Jql.Context
{
    public class QueryContext : IQueryContext
    {
        public IEnumerable<CatalogAssetTypeContexts> CatalogAssetTypeContexts { get; set; }

        public QueryContext(IEnumerable<CatalogAssetTypeContexts> catalogAssetTypeContexts)
        {
            CatalogAssetTypeContexts = catalogAssetTypeContexts;
        }

        private static IEnumerable<QueryContextCatalogManufacturerContexts> Init(IClauseContext clauseContext)
        {
            //var contextSetMap = new Multimap<ICatalogContext, IManufacturerContext>();

            ISet<ICatalogManufacturerContext> contexts = clauseContext.Contexts;
            foreach (ProjectIssueTypeContext context in contexts)
            {
                contextSetMap.putSingle(context.ProjectContext, context.IssueTypeContext);
            }

            IList<ProjectIssueTypeContexts> ctxs = new List<ProjectIssueTypeContexts>(contextSetMap.size());
            foreach (KeyValuePair<ProjectContext, Set<IssueTypeContext>> entry in contextSetMap.entrySet())
            {
                ctxs.Add(new ProjectIssueTypeContexts(entry.Key, entry.Value));
            }
            return ctxs;

        }
    }
}