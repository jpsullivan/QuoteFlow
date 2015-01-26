using System.Collections.Generic;
using QuoteFlow.Api.Asset.Fields;
using QuoteFlow.Api.Asset.Index.Indexers;
using QuoteFlow.Api.Asset.Search.Constants;
using QuoteFlow.Api.Asset.Search.Searchers;
using QuoteFlow.Api.Asset.Search.Searchers.Information;
using QuoteFlow.Api.Asset.Search.Searchers.Transformer;
using QuoteFlow.Api.Services;

namespace QuoteFlow.Core.Asset.Search.Searchers
{
    /// <summary>
    /// Responsible for searching for Creators
    /// </summary>
    public class CreatorSearcher : AbstractInitializationSearcher, IAssetSearcher<ISearchableField>
    {
        public const string NAME_KEY = "navigator.filter.createdby";
        
        public CreatorSearcher(IUserService userService)
        {
            UserFieldSearchConstantsWithEmpty searchConstants = SystemSearchConstants.ForCreator();

            SearchInformation = new GenericSearcherInformation<ISearchableField>(searchConstants.SearcherId, NAME_KEY, new List<IFieldIndexer>(), FieldReference, SearcherGroupType.Asset);
            SearchInputTransformer = new EnhancedUserSearchInputTransformer(searchConstants, userService);
        }

        public virtual ISearcherInformation<ISearchableField> SearchInformation { get; set; }

        public virtual ISearchInputTransformer SearchInputTransformer { get; set; }
    }

}