using System.Collections.Generic;
using System.Linq;
using Lucene.Net.Search.Vectorhighlight;

namespace QuoteFlow.Models.Search.Jql.Context
{
    /// <summary>
    /// Performs set utilities on <see cref="IClauseContext"/>'s
    /// </summary>
    public static class ContextSetUtil
    {
        private static readonly ICatalogAssetTypeContext GlobalContext = CatalogAssetTypeContext.CreateGlobalContext();

        /// <summary>
        /// Performs an itersection of the ClauseContext's passed in.
        /// 
        /// NOTE: When <see cref="CatalogAssetTypeContext"/>'s are compared they are considered
        /// equivilent if the id values are the same, we do not compare if they are Explicit or Implicit. When combined
        /// an Explicit flag will always replace an Implicit flag.
        /// </summary>
        /// <param name="childClauseContexts">The child clause contexts to intersect, must never be null or contain null elements.</param>
        /// <returns>The intersection of ClauseContext's that were passed in.</returns>
        public static IClauseContext Intersect<T>(this ISet<T> childClauseContexts) where T : IClauseContext
        {
            // todo: need to throw argumentnullexception if list has any null values

            if (!childClauseContexts.Any())
            {
                return new ClauseContext();
            }

            var iter = childClauseContexts.GetEnumerator();

            // Our initial result set is the first set of CatalogAssetTypeContext's in our childClauseContexts
            iter.MoveNext();
            IClauseContext intersection = iter.Current;

            if (childClauseContexts.Count == 1)
            {
                return new ClauseContext(intersection.Contexts);
            }

            while (iter.MoveNext())
            {
                IClauseContext toIntersect = iter.Current;
                intersection = Intersect(intersection, toIntersect);
            }

            return intersection;
        }

        /// <summary>
        /// Performs a union of the ClauseContext's passed in.
        /// 
        /// NOTE: When <seea cref="CatalogAssetTypeContext"/>'s are compared they are considered
        /// equivilent if the id values are the same, we do not compare if they are Explicit or Implicit. When combined
        /// an Explicit flag will always replace an Implicit flag.
        /// </summary>
        public static IClauseContext Union<T>(this ISet<T> childClauseContexts) where T : IClauseContext
		{
			// todo: need to throw argumentnullexception if list has any null values

			if (!childClauseContexts.Any())
			{
				return new ClauseContext();
			}

			var iter = childClauseContexts.GetEnumerator();

			// Our initial result set is the first set of ProjectIssueTypeContext's in out childClauseContexts
            iter.MoveNext();
            IClauseContext union = iter.Current;

			while (iter.MoveNext())
			{
				IClauseContext toUnion = iter.Current;
				union = Union(union, toUnion);
			}

			return union;
		}

        private static IClauseContext Intersect(IClauseContext context1, IClauseContext context2)
        {
            IClauseContext clauseContext = ShortCircuitIfBothGlobal(context1, context2);
            if (clauseContext != null)
            {
                return clauseContext;
            }
            var contextProjectMap1 = new ContextCatalogMap(context1);
            var contextProjectMap2 = new ContextCatalogMap(context2);
            clauseContext = contextProjectMap1.Intersect(contextProjectMap2);
            return clauseContext;
        }

        private static IClauseContext Union(IClauseContext context1, IClauseContext context2)
        {
            var contextProjectMap1 = new ContextCatalogMap(context1);
            var contextProjectMap2 = new ContextCatalogMap(context2);
            return contextProjectMap1.Union(contextProjectMap2);
        }

        private static IDictionary<ICatalogContext, ISet<IAssetTypeContext>> HandleCatalogGlobals(ISet<IAssetTypeContext> issueTypeContexts, ISet<ICatalogContext> projectContexts)
        {
            var resultsMap = new HashMap<ICatalogContext, ISet<IAssetTypeContext>>();
            if (issueTypeContexts != null)
            {
                foreach (ICatalogContext projectContext in projectContexts)
                {
                    resultsMap[projectContext] = issueTypeContexts;
                }
            }

            return resultsMap;
        }

        private static IClauseContext ShortCircuitIfBothGlobal(IClauseContext context1, IClauseContext context2)
        {
            if (context1.Contexts.Contains(GlobalContext) && context2.Contexts.Contains(GlobalContext))
            {
                return new ClauseContext(context1.Contexts.Union(context2.Contexts));
            }
            return null;
        }
    }
}