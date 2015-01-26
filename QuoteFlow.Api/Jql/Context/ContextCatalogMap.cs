using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Lucene.Net.Support;

namespace QuoteFlow.Api.Jql.Context
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

		internal readonly IDictionary<ICatalogContext, ISet<IManufacturerContext>> AssetTypeContextsPerCatalog = new HashMap<ICatalogContext, ISet<IManufacturerContext>>();
		internal readonly ISet<IManufacturerContext> AssetTypeContextsContainedInGlobalCatalogContexts = new HashSet<IManufacturerContext>();
		internal readonly ISet<ICatalogContext> CatalogsWithAllAssetTypes = new HashSet<ICatalogContext>();
		internal readonly bool ContainsGlobalContext;

		public ContextCatalogMap(IClauseContext context)
		{
            ISet<ICatalogManufacturerContext> catalogAssetTypeContexts = context.Contexts;
			ContainsGlobalContext = context.ContainsGlobalContext();
			if (catalogAssetTypeContexts != null)
			{
				foreach (ICatalogManufacturerContext catalogAssetTypeContext in catalogAssetTypeContexts)
				{
					ICatalogContext catalogContext = catalogAssetTypeContext.CatalogContext;
					IManufacturerContext issueTypeContext = catalogAssetTypeContext.ManufacturerContext;
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
                        ISet<IManufacturerContext> issueTypeContexts = new HashSet<IManufacturerContext> { issueTypeContext };
						AssetTypeContextsPerCatalog[catalogContext] = issueTypeContexts;
					}
				}
			}
		}

		public virtual IClauseContext Intersect(ContextCatalogMap contextProjectMap)
		{
			ISet<ICatalogManufacturerContext> intersection = ToCatalogManufacturerContextSet(CombineContextMaps(AssetTypeContextsPerCatalog, contextProjectMap.AssetTypeContextsPerCatalog, CombineType.Intersect));
			IEnumerable<ICatalogManufacturerContext> explicitCatalogAssetTypeContexts = HandleGlobals(contextProjectMap);
		    foreach (var explicitCatalogAssetTypeContext in explicitCatalogAssetTypeContexts)
		    {
		        intersection.Add(explicitCatalogAssetTypeContext);
		    }
			return new ClauseContext(intersection);
		}

		public virtual IClauseContext Union(ContextCatalogMap contextProjectMap)
		{
			ISet<ICatalogManufacturerContext> union = ToCatalogManufacturerContextSet(CombineContextMaps(AssetTypeContextsPerCatalog, contextProjectMap.AssetTypeContextsPerCatalog, CombineType.Union));
			return new ClauseContext(union);
		}

        private IDictionary<ICatalogContext, ISet<IManufacturerContext>> CombineContextMaps(IDictionary<ICatalogContext, ISet<IManufacturerContext>> map1, IDictionary<ICatalogContext, ISet<IManufacturerContext>> map2, CombineType combineType)
        {
            var projectContexts = new HashSet<ICatalogContext>(map1.Keys);
            foreach (var key in map2.Keys) projectContexts.Add(key);
			
            var results = new HashMap<ICatalogContext, ISet<IManufacturerContext>>();
			foreach (ICatalogContext catalogContext in projectContexts)
			{
                var assetTypesContextSet1 = map1[catalogContext] ?? new HashSet<IManufacturerContext>();
                var assetTypesContextSet2 = map2[catalogContext] ?? new HashSet<IManufacturerContext>();
				var resultSet = new List<IManufacturerContext>();
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

		/// <param name="contextProjectMap">The map containing sets of <seealso cref="ICatalogManufacturerContext"/>  keyed by <seealso cref="ICatalogContext"/></param>
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
		private IEnumerable<ICatalogManufacturerContext> HandleGlobals(ContextCatalogMap contextProjectMap)
		{
            var catalogAssetTypeContexts = new HashSet<ICatalogManufacturerContext>();
			if (ContainsGlobalContext)
			{
                foreach (var catalogAssetTypeContext in ToCatalogManufacturerContextSet(contextProjectMap.AssetTypeContextsPerCatalog))
                {
                    catalogAssetTypeContexts.Add(catalogAssetTypeContext);
                }
			}
			if (contextProjectMap.ContainsGlobalContext)
			{
                foreach (var catalogAssetTypeContext in ToCatalogManufacturerContextSet(AssetTypeContextsPerCatalog))
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

		private IEnumerable<ICatalogManufacturerContext> GetCatalogAssetTypeContextsForCatalogGlobals(IEnumerable<ICatalogContext> catalogsWithAllAssetTypes, IDictionary<ICatalogContext, ISet<IManufacturerContext>> assetTypeContextsMap)
		{
			var catalogAssetTypeContexts = new HashSet<ICatalogManufacturerContext>();
			foreach (ICatalogContext projectContext in catalogsWithAllAssetTypes)
			{
				var assetTypeContexts = assetTypeContextsMap[projectContext];
				var assetTypeContextsInAllCatalogContexts = assetTypeContextsMap[AllCatalogsContext.INSTANCE];
				if (assetTypeContexts != null)
				{
                    foreach (var catalogAssetTypeContext in ToCatalogManufacturerContextSet(projectContext, assetTypeContexts))
                    {
                        catalogAssetTypeContexts.Add(catalogAssetTypeContext);
                    }
				}
				if (assetTypeContextsInAllCatalogContexts != null)
				{
                    foreach (var catalogAssetTypeContext in ToCatalogManufacturerContextSet(projectContext, assetTypeContextsInAllCatalogContexts))
                    {
                        catalogAssetTypeContexts.Add(catalogAssetTypeContext);
                    }
				}
			}
			return catalogAssetTypeContexts;
		}

		private IEnumerable<ICatalogManufacturerContext> ToCatalogManufacturerContextSet(ICatalogContext catalogContext, ISet<IManufacturerContext> issueTypeContexts)
		{
			var catalogManufacturerContexts = new HashSet<ICatalogManufacturerContext>();
			foreach (IManufacturerContext manufacturerContext in issueTypeContexts)
			{
				if (manufacturerContext != AllManufacturersContext.Instance)
				{
					catalogManufacturerContexts.Add(new CatalogManufacturerContext(catalogContext, manufacturerContext));
				}
			}
			return catalogManufacturerContexts;
		}

		private IEnumerable<ICatalogManufacturerContext> GetCatalogAssetTypeContextsForAllAssetTypes(ISet<IManufacturerContext> issueTypeContextsInAllProjects, IDictionary<ICatalogContext, ISet<IManufacturerContext>> issueTypeContextsMap)
		{
			var catalogAssetTypeContexts = new HashSet<ICatalogManufacturerContext>();
			if (issueTypeContextsInAllProjects.Any())
			{
				foreach (ICatalogContext projectContext in issueTypeContextsMap.Keys)
				{
					if (projectContext != AllCatalogsContext.INSTANCE)
					{
						var issueTypeContexts = issueTypeContextsMap[projectContext];
						if (issueTypeContexts != null)
						{
							foreach (IManufacturerContext issueTypeContext in issueTypeContexts)
							{
								if (issueTypeContextsInAllProjects.Contains(issueTypeContext))
								{
									catalogAssetTypeContexts.Add(new CatalogManufacturerContext(projectContext, issueTypeContext));
								}
							}
						}
					}
				}
			}
			return catalogAssetTypeContexts;
		}

		private ISet<ICatalogManufacturerContext> ToCatalogManufacturerContextSet(IDictionary<ICatalogContext, ISet<IManufacturerContext>> projectIssueTypeContextMap)
		{
			var catalogAssetTypeContexts = new HashSet<ICatalogManufacturerContext>();
			if (projectIssueTypeContextMap.Count > 0)
			{
				foreach (KeyValuePair<ICatalogContext, ISet<IManufacturerContext>> entry in projectIssueTypeContextMap)
				{
					foreach (IManufacturerContext issueTypeContext in entry.Value)
					{
						catalogAssetTypeContexts.Add(new CatalogManufacturerContext(entry.Key, issueTypeContext));
					}
				}
			}

			return catalogAssetTypeContexts;
		}
    }
}