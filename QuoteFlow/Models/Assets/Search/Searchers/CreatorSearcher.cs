using System.Collections.Generic;
using QuoteFlow.Models.Assets.Fields;
using QuoteFlow.Models.Assets.Index.Indexers;
using QuoteFlow.Models.Assets.Search.Constants;
using QuoteFlow.Models.Assets.Search.Searchers.Information;
using QuoteFlow.Models.Assets.Search.Searchers.Transformer;
using QuoteFlow.Services.Interfaces;

namespace QuoteFlow.Models.Assets.Search.Searchers
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