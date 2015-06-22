using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Web.Hosting;
using QuoteFlow.Api.Asset.Search.Constants;
using QuoteFlow.Api.Asset.Search.Searchers.Transformer;
using QuoteFlow.Api.Asset.Transport;
using QuoteFlow.Api.Jql.Query;
using QuoteFlow.Api.Models;
using RazorEngine;
using RazorEngine.Configuration;
using RazorEngine.Templating;
using StackExchange.Profiling;

namespace QuoteFlow.Api.Asset.Search.Searchers.Renderer
{
    /// <summary>
    /// Abstract class for SearchRenderers that provides some common methods.
    /// </summary>
    public abstract class AbstractSearchRenderer : ISearchRenderer
    {
        private readonly string _searcherId;
        private readonly string _searcherNameKey;

        public AbstractSearchRenderer(SimpleFieldSearchConstants searchConstants, string searcherNameKey)
            : this(searchConstants.SearcherId, searcherNameKey)
        {
        }

        public AbstractSearchRenderer(string searcherId, string searcherNameKey)
        {
            _searcherId = searcherId;
            _searcherNameKey = searcherNameKey;
        }

        protected IDictionary<string, object> GetDisplayParams(User searcher, ISearchContext searchContext,
            IFieldValuesHolder fieldValuesHolder, IDictionary<string, object> displayParameters)
        {
            var templateParams = new Dictionary<string, object>();
            templateParams.Add("searchContext", searchContext);
            templateParams.Add("fieldValuesHolder", fieldValuesHolder);
            templateParams.Add("displayParameters", displayParameters);

            // static shit
            templateParams.Add("searcherId", _searcherId);
            templateParams.Add("searcherNameKey", _searcherNameKey);

            // required for custom/system field templates
            templateParams.Add("auiparams", new Dictionary<string, object>());

            return templateParams;
        }

        protected string RenderEditTemplate(string templateName, IDictionary<string, object> templateParams)
        {
            // measure how long this takes to render
            var profiler = MiniProfiler.Current; // it's ok if this is null
            using (profiler.Step(string.Format("Rendering the {0} template", templateName))) 
            {
                var result = Engine.Razor.RunCompile(templateName, null, templateParams);
                return result;
            }
        }

        protected string RenderViewTemplate(string templateName, object templateParams)
        {
            return string.Empty;
        }

        protected bool IsRelevantForQuery(ClauseNames clauseNames, IQuery query)
        {
            if ((query == null) || (query.WhereClause == null))
            {
                return false;
            }

            var clauseVisitor = new NamedTerminalClauseCollectingVisitor(clauseNames.JqlFieldNames);
            query.WhereClause.Accept(clauseVisitor);
            return clauseVisitor.ContainsNamedClause();
        }

        public abstract string GetEditHtml(User user, ISearchContext searchContext, IFieldValuesHolder fieldValuesHolder, IDictionary<string, object> displayParameters);

        public abstract bool IsShown(User user, ISearchContext searchContext);

        public abstract string GetViewHtml(User user, ISearchContext searchContext, IFieldValuesHolder fieldValuesHolder, IDictionary<string, object> displayParameters);

        public abstract bool IsRelevantForQuery(User user, IQuery query);
    }
}