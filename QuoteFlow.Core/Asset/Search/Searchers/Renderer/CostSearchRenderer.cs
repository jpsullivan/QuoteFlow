using System;
using System.Collections.Generic;
using QuoteFlow.Api.Asset.Search;
using QuoteFlow.Api.Asset.Search.Constants;
using QuoteFlow.Api.Asset.Search.Searchers.Renderer;
using QuoteFlow.Api.Asset.Transport;
using QuoteFlow.Api.Jql.Query;
using QuoteFlow.Api.Models;
using QuoteFlow.Core.Asset.Search.Searchers.Util;

namespace QuoteFlow.Core.Asset.Search.Searchers.Renderer
{
    /// <summary>
    /// Searcher renderer for the <see cref="CostSearcher"/>.
    /// </summary>
    public class CostSearchRenderer : AbstractSearchRenderer, ISearchRenderer
    {
        private readonly SimpleFieldSearchConstants _constants;
        private readonly CostSearcherConfig _config;

        public CostSearchRenderer(SimpleFieldSearchConstants searchConstants, string searcherNameKey, CostSearcherConfig config) 
            : base(searchConstants, searcherNameKey)
        {
            _constants = searchConstants;
            _config = config;

        }

        public override string GetEditHtml(User user, ISearchContext searchContext, IFieldValuesHolder fieldValuesHolder,
            IDictionary<string, object> displayParameters)
        {
            var templateParameters = GetDisplayParams(user, searchContext, fieldValuesHolder, displayParameters);
            return RenderEditTemplate("CostSearcherEdit", templateParameters);
        }

        public override bool IsShown(User user, ISearchContext searchContext)
        {
            return true;
        }

        public override string GetViewHtml(User user, ISearchContext searchContext, IFieldValuesHolder fieldValuesHolder,
            IDictionary<string, object> displayParameters)
        {
            var templateParameters = GetDisplayParams(user, searchContext, fieldValuesHolder, displayParameters);
            return RenderEditTemplate("CostSearcherView", templateParameters);
        }

        public override bool IsRelevantForQuery(User user, IQuery query)
        {
            return IsRelevantForQuery(_constants.JqlClauseNames, query);
        }

        protected override IDictionary<string, object> GetDisplayParams(User searcher, ISearchContext searchContext,
            IFieldValuesHolder fieldValuesHolder, IDictionary<string, object> displayParameters)
        {
            var templateParameters = base.GetDisplayParams(searcher, searchContext, fieldValuesHolder, displayParameters);
            templateParameters.Add("minField", _config.Min);
            templateParameters.Add("maxField", _config.Max);
            templateParameters.Add("minGreaterThanMax", false);

            // special case for when min > max to highlight both values as red
            try
            {
                int min = Convert.ToInt32((string) fieldValuesHolder[_config.Min]);
                int max = Convert.ToInt32((string) fieldValuesHolder[_config.Max]);

                if (min > max)
                {
                    templateParameters["minGreaterThanMax"] =  true;
                }
            }
            catch (Exception)
            {
                // swallow the exception
            }

            return templateParameters;
        }
    }
}