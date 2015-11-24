using System.Collections.Generic;
using System.Web;
using QuoteFlow.Api.Asset.Search;
using QuoteFlow.Api.Asset.Search.Searchers.Renderer;
using QuoteFlow.Api.Asset.Search.Searchers.Transformer;
using QuoteFlow.Api.Asset.Transport;
using QuoteFlow.Api.Jql.Query;
using QuoteFlow.Api.Models;

namespace QuoteFlow.Core.Asset.Search.Searchers.Renderer
{
    public class TextQuerySearchRenderer : AbstractSearchRenderer, ISearchRenderer
    {
        private readonly string _name;
        private readonly ISearchInputTransformer _searchInputTransformer;

        public TextQuerySearchRenderer(string id, string name, string labelKey, ISearchInputTransformer searchInputTransformer)
            : base(id, labelKey)
        {
            _name = name;
            _searchInputTransformer = searchInputTransformer;
        }

        public override string GetEditHtml(User user, ISearchContext searchContext, IFieldValuesHolder fieldValuesHolder,
            IDictionary<string, object> displayParameters)
        {
            var value = string.Empty;
            object fieldValue;
            if (fieldValuesHolder.TryGetValue(_name, out fieldValue))
            {
                value = (string) fieldValue;
            }

            // this searcher html is only used as a transport protocol, the client
            // only needs the value out and renders it client-side
            return HttpUtility.HtmlEncode(value);
        }

        public override bool IsShown(User user, ISearchContext searchContext)
        {
            return true;
        }

        public override string GetViewHtml(User user, ISearchContext searchContext, IFieldValuesHolder fieldValuesHolder,
            IDictionary<string, object> displayParameters)
        {
            var value = string.Empty;
            object fieldValue;
            if (fieldValuesHolder.TryGetValue(_name, out fieldValue))
            {
                value = (string)fieldValue;
            }

            // this searcher html is only used as a transport protocol, the client
            // only needs the value out and renders it client-side
            return HttpUtility.HtmlEncode(value);
        }

        public override bool IsRelevantForQuery(User user, IQuery query)
        {
            return _searchInputTransformer.DoRelevantClausesFitFilterForm(user, query, null);
        }
    }
}