using System.Collections;
using QuoteFlow.Api.Asset.Transport;
using QuoteFlow.Api.Jql.Query;
using QuoteFlow.Api.Models;

namespace QuoteFlow.Api.Asset.Search.Searchers.Renderer
{
    /// <summary>
    /// Abstract class for SearchRenderers that provides some common methods.
    /// </summary>
    public class AbstractSearchRenderer : ISearchRenderer
    {
        public string GetEditHtml(User user, ISearchContext searchContext, IFieldValuesHolder fieldValuesHolder, IDictionary displayParameters)
        {
            throw new System.NotImplementedException();
        }

        public bool IsShown(User user, ISearchContext searchContext)
        {
            throw new System.NotImplementedException();
        }

        public string GetViewHtml(User user, ISearchContext searchContext, IFieldValuesHolder fieldValuesHolder, IDictionary displayParameters)
        {
            throw new System.NotImplementedException();
        }

        public bool IsRelevantForQuery(User user, IQuery query)
        {
            throw new System.NotImplementedException();
        }
    }
}