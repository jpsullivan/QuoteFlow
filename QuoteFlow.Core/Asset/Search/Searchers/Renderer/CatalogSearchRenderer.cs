using System.Collections;
using System.Collections.Generic;
using QuoteFlow.Api.Asset.Search;
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

        public CatalogSearchRenderer(string searcherId, string searcherNameKey, ICatalogService catalogService) : base(searcherId, searcherNameKey)
        {
            CatalogService = catalogService;
        }

        public CatalogSearchRenderer(string searcherId, string searcherNameKey) : base(searcherId, searcherNameKey)
        {
        }

        public override string GetEditHtml(User user, ISearchContext searchContext, IFieldValuesHolder fieldValuesHolder, IDictionary<string, object> displayParameters)
        {
            throw new System.NotImplementedException();
        }

        public override bool IsShown(User user, ISearchContext searchContext)
        {
            return true;
        }

        public override string GetViewHtml(User user, ISearchContext searchContext, IFieldValuesHolder fieldValuesHolder, IDictionary displayParameters)
        {
            throw new System.NotImplementedException();
        }

        public override bool IsRelevantForQuery(User user, IQuery query)
        {
            throw new System.NotImplementedException();
        }
    }
}