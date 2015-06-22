using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using QuoteFlow.Api.Asset.Search;
using QuoteFlow.Api.Asset.Search.Constants;
using QuoteFlow.Api.Asset.Search.Searchers.Renderer;
using QuoteFlow.Api.Asset.Transport;
using QuoteFlow.Api.Jql.Query;
using QuoteFlow.Api.Models;
using QuoteFlow.Api.Services;

namespace QuoteFlow.Core.Asset.Search.Searchers.Renderer
{
    public class CatalogSearchRenderer : AbstractSearchRenderer, ISearchRenderer
    {
        // if the number of catalogs a user can see is <= this amount, we don't show recently used
        public const int MaxCatalogsBeforeRecent = 10;

        // max number of recently used catalogs to show
        public const int MaxRecentCatalogsToShow = 5;

        public ICatalogService CatalogService { get; protected set; }

        public CatalogSearchRenderer(ICatalogService catalogService, string searcherNameKey) 
            : base(SystemSearchConstants.ForCatalog(), searcherNameKey)
        {
            CatalogService = catalogService;
        }

        public CatalogSearchRenderer(string searcherId, string searcherNameKey) 
            : base(searcherId, searcherNameKey)
        {
        }

        public override string GetEditHtml(User user, ISearchContext searchContext, IFieldValuesHolder fieldValuesHolder, IDictionary<string, object> displayParameters)
        {
            var templateParameters = GetDisplayParams(user, searchContext, fieldValuesHolder, displayParameters);
            AddParameters(user, fieldValuesHolder, false, templateParameters);
            return RenderEditTemplate("CatalogSearcherEdit", templateParameters);
        }

        public void AddParameters(User searcher, IFieldValuesHolder fieldValuesHolder, bool noCurrentSearchRequest, IDictionary<string, object> templateParams)
        {
            var allCatalogs = GetVisibleCatalogs(searcher).ToList();
            templateParams.Add("visibleCatalogs", allCatalogs);

            // if there is no search request in session and no catalog has been specified in the params,
            // add the single visible catalog to the list
            if (noCurrentSearchRequest && allCatalogs.Count() == 1 &&
                !fieldValuesHolder.ContainsKey(SystemSearchConstants.ForCatalog().UrlParameter))
            {
                var singleCatalogId = allCatalogs.First().Id;
                templateParams.Add("selectedCatalogs", new List<int>() {singleCatalogId});
            }
            else
            {
                object catalogs;
                if (!fieldValuesHolder.TryGetValue(SystemSearchConstants.ForCatalog().UrlParameter, out catalogs)) return;

                // attempt to cast the catalogs into a list
                var resolvedCatalogs = (IList) catalogs;

                if (resolvedCatalogs != null && resolvedCatalogs.Count == 1 && resolvedCatalogs[0].Equals("-1"))
                {
                    templateParams.Add("selectedCatalogs", new List<int>());
                }
                else
                {
                    templateParams.Add("selectedCatalogs", resolvedCatalogs ?? new List<int>());
                }
            }

            if (allCatalogs.Count > MaxCatalogsBeforeRecent)
            {
                templateParams.Add("recentCatalogs", null);
            }
        }

        public override bool IsShown(User user, ISearchContext searchContext)
        {
            return true;
        }

        public override string GetViewHtml(User user, ISearchContext searchContext, IFieldValuesHolder fieldValuesHolder, IDictionary<string, object> displayParameters)
        {
            var templateParameters = GetDisplayParams(user, searchContext, fieldValuesHolder, displayParameters);

            object fieldValue;
            if (fieldValuesHolder.TryGetValue(SystemSearchConstants.ForCatalog().UrlParameter, out fieldValue))
            {
                var resolvedCatalogIds = fieldValue as List<string>;
                if (resolvedCatalogIds != null)
                {
                    var catalogIds = resolvedCatalogIds.Select(int.Parse).ToList();
                    var catalogs = catalogIds.Select(id => CatalogService.GetCatalog(id)).ToList();
                    if (catalogs.Any())
                    {
                        templateParameters.Add("filteredOutCatalogs", catalogs);
                    }

                    templateParameters.Add("selectedCatalogs", catalogs);
                }
            }

            return RenderEditTemplate("CatalogSearcherView", templateParameters);
        }

        public override bool IsRelevantForQuery(User user, IQuery query)
        {
            return IsRelevantForQuery(SystemSearchConstants.ForCatalog().JqlClauseNames, query);
        }

        public IEnumerable<Catalog> GetVisibleCatalogs(User searcher)
        {
            // todo only load catalogs that are available to the specified user

            return CatalogService.GetCatalogs(1); // todo org fix
        }
    }
}