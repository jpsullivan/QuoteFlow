using System.Collections.Generic;
using QuoteFlow.Models.Assets.CustomFields.Searchers.Transformer;
using QuoteFlow.Models.Assets.Fields;
using QuoteFlow.Models.Assets.Index.Indexers;
using QuoteFlow.Models.Assets.Search.Constants;
using QuoteFlow.Models.Assets.Search.Searchers.Information;
using QuoteFlow.Models.Assets.Search.Searchers.Transformer;
using QuoteFlow.Models.Assets.Search.Searchers.Util;
using QuoteFlow.Models.Search.Jql.Operand;
using QuoteFlow.Models.Search.Jql.Util;

namespace QuoteFlow.Models.Assets.Search.Searchers
{
    /// <summary>
    /// A simple class that most date searchers will be able to extends to implement searching. 
    /// </summary>
    public class AbstractDateSearcher : AbstractInitializationSearcher, IAssetSearcher<ISearchableField>
    {
        private readonly ISearcherInformation<ISearchableField> searcherInformation;
        private readonly ISearchInputTransformer searchInputTransformer;

        public AbstractDateSearcher(
            SimpleFieldSearchConstants constants, 
            string nameKey, BaseFieldIndexer indexer, 
            IJqlDateSupport dateSupport, 
            JqlOperandResolver operandResolver,
            CustomFieldInputHelper customFieldInputHelper)
        {
            var config = new DateSearcherConfig(constants.UrlParameter, constants.JqlClauseNames, constants.JqlClauseNames.PrimaryName);
            this.searcherInformation = new GenericSearcherInformation<ISearchableField>(constants.SearcherId, nameKey, new List<IFieldIndexer>() { indexer }, FieldReference, SearcherGroupType.Date);
            this.searchInputTransformer = new DateSearchInputTransformer(false, config, operandResolver, dateSupport, customFieldInputHelper);
        }

        public ISearcherInformation<ISearchableField> SearchInformation
        {
            get { return searcherInformation; }
        }

        public ISearchInputTransformer SearchInputTransformer
        {
            get { return searchInputTransformer; }
        }

    }

}