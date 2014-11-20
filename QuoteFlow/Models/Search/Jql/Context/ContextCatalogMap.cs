using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Lucene.Net.Search.Vectorhighlight;

namespace QuoteFlow.Models.Search.Jql.Context
{
    /// <summary>
    /// Helper class to simplify the job of combining multiple contexts.
    /// </summary>
    public class ContextCatalogMap
    {
        private enum CombineType
		{
			Intersect,
			Union
		}

		internal readonly IDictionary<ICatalogContext, ISet<IAssetTypeContext>> AssetTypeContextsPerCatalog = new HashMap<ICatalogContext, ISet<IAssetTypeContext>>();
		internal readonly ISet<IAssetTypeContext> AssetTypeContextsContainedInGlobalCatalogContexts = new HashSet<IAssetTypeContext>();
		internal readonly ISet<ICatalogContext> CatalogsWithAllAssetTypes = new HashSet<ICatalogContext>();
		internal readonly bool ContainsGlobalContext;

		public ContextCatalogMap(IClauseContext context)
		{
            ISet<ICatalogAssetTypeContext> catalogAssetTypeContexts = context.Contexts;
			ContainsGlobalContext = context.ContainsGlobalContext();
			if (catalogAssetTypeContexts != null)
			{
				foreach (ICatalogAssetTypeContext catalogAssetTypeContext in catalogAssetTypeContexts)
				{
					ICatalogContext catalogContext = catalogAssetTypeContext.CatalogContext;
					IAssetTypeContext issueTypeContext = catalogAssetTypeContext.AssetTypeContext;
					if (catalogContext.IsAll())
					{
						if (!issueTypeContext.All)
						{
							AssetTypeContextsContainedInGlobalCatalogContexts.Add(issueTypeContext);
						}
					}
					else
					{
						if (issueTypeContext.All)
						{
							CatalogsWithAllAssetTypes.Add(catalogContext);
						}
					}
					if (AssetTypeContextsPerCatalog.ContainsKey(catalogContext))
					{
						AssetTypeContextsPerCatalog[catalogContext].Add(issueTypeContext);
					}
					else
					{
                        ISet<IAssetTypeContext> issueTypeContexts = new HashSet<IAssetTypeContext> { issueTypeContext };
						AssetTypeContextsPerCatalog[catalogContext] = issueTypeContexts;
					}
				}
			}
		}

		public virtual IClauseContext Intersect(ContextCatalogMap contextProjectMap)
		{
			ISet<ICatalogAssetTypeContext> intersection = ToCatalogAssetTypeContextSet(CombineContextMaps(AssetTypeContextsPerCatalog, contextProjectMap.AssetTypeContextsPerCatalog, CombineType.Intersect));
			IEnumerable<ICatalogAssetTypeContext> explicitCatalogAssetTypeContexts = HandleGlobals(contextProjectMap);
		    foreach (var explicitCatalogAssetTypeContext in explicitCatalogAssetTypeContexts)
		    {
		        intersection.Add(explicitCatalogAssetTypeContext);
		    }
			return new ClauseContext(intersection);
		}

		public virtual IClauseContext Union(ContextCatalogMap contextProjectMap)
		{
			ISet<ICatalogAssetTypeContext> union = ToCatalogAssetTypeContextSet(CombineContextMaps(AssetTypeContextsPerCatalog, contextProjectMap.AssetTypeContextsPerCatalog, CombineType.Union));
			return new ClauseContext(union);
		}

        private IDictionary<ICatalogContext, ISet<IAssetTypeContext>> CombineContextMaps(IDictionary<ICatalogContext, ISet<IAssetTypeContext>> map1, IDictionary<ICatalogContext, ISet<IAssetTypeContext>> map2, CombineType combineType)
        {
            var projectContexts = new HashSet<ICatalogContext>(map1.Keys);
            foreach (var key in map2.Keys) projectContexts.Add(key);
			
            var results = new HashMap<ICatalogContext, ISet<IAssetTypeContext>>();
			foreach (ICatalogContext catalogContext in projectContexts)
			{
                var assetTypesContextSet1 = map1[catalogContext] ?? new HashSet<IAssetTypeContext>();
                var assetTypesContextSet2 = map2[catalogContext] ?? new HashSet<IAssetTypeContext>();
				var resultSet = new List<IAssetTypeContext>();
				if (combineType == CombineType.Union)
				{
					resultSet = assetTypesContextSet1.Union(assetTypesContextSet2).ToList();
				}
				else if (combineType == CombineType.Intersect)
				{
					resultSet = assetTypesContextSet1.Intersect(assetTypesContextSet2).ToList();
				}
				if (resultSet.Any())
				{
					results[catalogContext] = resultSet.ToImmutableHashSet();
				}
			}
			return results;
		}

		/// <param name="contextProjectMap">The map containing sets of <seealso cref="ICatalogAssetTypeContext"/>  keyed by <seealso cref="ICatalogContext"/></param>
		/// <returns>A set that represents IMPLICIT type contexts that have been replaced by EXPLICIT type contexts.
		/// 
		/// The rules are as follows
		/// 
		/// an ALL.ALL - any combination is replaced by any
		/// an ALL.x - y.x - is replaced by y.x
		/// an x.ALL - x.y - is replaced by x.y
		/// an ALL.x - x.ALL - is replaced by x.x
		/// 
		/// This replacement needs to take place in both directions.
		/// </returns>
		private IEnumerable<ICatalogAssetTypeContext> HandleGlobals(ContextCatalogMap contextProjectMap)
		{
            var catalogAssetTypeContexts = new HashSet<ICatalogAssetTypeContext>();
			if (ContainsGlobalContext)
			{
                foreach (var catalogAssetTypeContext in ToCatalogAssetTypeContextSet(contextProjectMap.AssetTypeContextsPerCatalog))
                {
                    catalogAssetTypeContexts.Add(catalogAssetTypeContext);
                }
			}
			if (contextProjectMap.ContainsGlobalContext)
			{
                foreach (var catalogAssetTypeContext in ToCatalogAssetTypeContextSet(AssetTypeContextsPerCatalog))
                {
                    catalogAssetTypeContexts.Add(catalogAssetTypeContext);
                }
			}

            foreach (var globalContext in GetCatalogAssetTypeContextsForCatalogGlobals(CatalogsWithAllAssetTypes, contextProjectMap.AssetTypeContextsPerCatalog))
            {
                catalogAssetTypeContexts.Add(globalContext);
            }

            foreach (var globalContext in GetCatalogAssetTypeContextsForCatalogGlobals(contextProjectMap.CatalogsWithAllAssetTypes, AssetTypeContextsPerCatalog))
            {
                catalogAssetTypeContexts.Add(globalContext);
            }

            foreach (var context in GetCatalogAssetTypeContextsForAllAssetTypes(AssetTypeContextsContainedInGlobalCatalogContexts, contextProjectMap.AssetTypeContextsPerCatalog))
            {
                catalogAssetTypeContexts.Add(context);
            }

            foreach (var context in GetCatalogAssetTypeContextsForAllAssetTypes(contextProjectMap.AssetTypeContextsContainedInGlobalCatalogContexts, AssetTypeContextsPerCatalog))
            {
                catalogAssetTypeContexts.Add(context);
            }

			return catalogAssetTypeContexts;
		}

		private IEnumerable<ICatalogAssetTypeContext> GetCatalogAssetTypeContextsForCatalogGlobals(IEnumerable<ICatalogContext> catalogsWithAllAssetTypes, IDictionary<ICatalogContext, ISet<IAssetTypeContext>> assetTypeContextsMap)
		{
			var catalogAssetTypeContexts = new HashSet<ICatalogAssetTypeContext>();
			foreach (ICatalogContext projectContext in catalogsWithAllAssetTypes)
			{
				var assetTypeContexts = assetTypeContextsMap[projectContext];
				var assetTypeContextsInAllCatalogContexts = assetTypeContextsMap[AllCatalogsContext.INSTANCE];
				if (assetTypeContexts != null)
				{
                    foreach (var catalogAssetTypeContext in ToCatalogAssetTypeContextSet(projectContext, assetTypeContexts))
                    {
                        catalogAssetTypeContexts.Add(catalogAssetTypeContext);
                    }
				}
				if (assetTypeContextsInAllCatalogContexts != null)
				{
                    foreach (var catalogAssetTypeContext in ToCatalogAssetTypeContextSet(projectContext, assetTypeContextsInAllCatalogContexts))
                    {
                        catalogAssetTypeContexts.Add(catalogAssetTypeContext);
                    }
				}
			}
			return catalogAssetTypeContexts;
		}

		private IEnumerable<ICatalogAssetTypeContext> ToCatalogAssetTypeContextSet(ICatalogContext catalogContext, ISet<IAssetTypeContext> issueTypeContexts)
		{
			var catalogAssetTypeContexts = new HashSet<ICatalogAssetTypeContext>();
			foreach (IAssetTypeContext assetTypeContext in issueTypeContexts)
			{
				if (assetTypeContext != AllAssetTypesContext.Instance)
				{
					catalogAssetTypeContexts.Add(new CatalogAssetTypeContext(catalogContext, assetTypeContext));
				}
			}
			return catalogAssetTypeContexts;
		}

		private IEnumerable<ICatalogAssetTypeContext> GetCatalogAssetTypeContextsForAllAssetTypes(ISet<IAssetTypeContext> issueTypeContextsInAllProjects, IDictionary<ICatalogContext, ISet<IAssetTypeContext>> issueTypeContextsMap)
		{
			var catalogAssetTypeContexts = new HashSet<ICatalogAssetTypeContext>();
			if (issueTypeContextsInAllProjects.Any())
			{
				foreach (ICatalogContext projectContext in issueTypeContextsMap.Keys)
				{
					if (projectContext != AllCatalogsContext.INSTANCE)
					{
						var issueTypeContexts = issueTypeContextsMap[projectContext];
						if (issueTypeContexts != null)
						{
							foreach (IAssetTypeContext issueTypeContext in issueTypeContexts)
							{
								if (issueTypeContextsInAllProjects.Contains(issueTypeContext))
								{
									catalogAssetTypeContexts.Add(new CatalogAssetTypeContext(projectContext, issueTypeContext));
								}
							}
						}
					}
				}
			}
			return catalogAssetTypeContexts;
		}

		private ISet<ICatalogAssetTypeContext> ToCatalogAssetTypeContextSet(IDictionary<ICatalogContext, ISet<IAssetTypeContext>> projectIssueTypeContextMap)
		{
			var catalogAssetTypeContexts = new HashSet<ICatalogAssetTypeContext>();
			if (projectIssueTypeContextMap.Count > 0)
			{
				foreach (KeyValuePair<ICatalogContext, ISet<IAssetTypeContext>> entry in projectIssueTypeContextMap)
				{
					foreach (IAssetTypeContext issueTypeContext in entry.Value)
					{
						catalogAssetTypeContexts.Add(new CatalogAssetTypeContext(entry.Key, issueTypeContext));
					}
				}
			}

			return catalogAssetTypeContexts;
		}
    }
}