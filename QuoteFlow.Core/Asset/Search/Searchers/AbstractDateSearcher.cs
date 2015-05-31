using System.Collections.Generic;
using QuoteFlow.Api.Asset.CustomFields.Searchers.Transformer;
using QuoteFlow.Api.Asset.Fields;
using QuoteFlow.Api.Asset.Index.Indexers;
using QuoteFlow.Api.Asset.Search.Constants;
using QuoteFlow.Api.Asset.Search.Searchers;
using QuoteFlow.Api.Asset.Search.Searchers.Information;
using QuoteFlow.Api.Asset.Search.Searchers.Renderer;
using QuoteFlow.Api.Asset.Search.Searchers.Transformer;
using QuoteFlow.Api.Asset.Search.Searchers.Util;
using QuoteFlow.Api.Jql.Operand;
using QuoteFlow.Api.Jql.Util;
using QuoteFlow.Core.Asset.CustomFields.Searchers.Information;
using QuoteFlow.Core.Asset.Index.Indexers;

namespace QuoteFlow.Core.Asset.Search.Searchers
{
    /// <summary>
    /// A simple class that most date searchers will be able to extends to implement searching. 
    /// </summary>
    public sealed class AbstractDateSearcher : AbstractInitializationSearcher
    {
        public override ISearcherInformation<ISearchableField> SearchInformation { get; set; }
        public override ISearchInputTransformer SearchInputTransformer { get; set; }
        public override ISearchRenderer SearchRenderer { get; set; }

        public AbstractDateSearcher(
            SimpleFieldSearchConstants constants, 
            string nameKey, BaseFieldIndexer indexer, 
            IJqlDateSupport dateSupport, 
            IJqlOperandResolver operandResolver,
            ICustomFieldInputHelper customFieldInputHelper)
        {
            var config = new DateSearcherConfig(constants.UrlParameter, constants.JqlClauseNames, constants.JqlClauseNames.PrimaryName);
            SearchInformation = new GenericSearcherInformation<ISearchableField>(constants.SearcherId, nameKey, new List<IFieldIndexer>() { indexer }, FieldReference, SearcherGroupType.Date);
            SearchInputTransformer = new DateSearchInputTransformer(false, config, operandResolver, dateSupport, customFieldInputHelper);
        }
    }
}