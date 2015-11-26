using System.Collections.Generic;
using QuoteFlow.Api.Asset.Fields;
using QuoteFlow.Api.Asset.Index.Indexers;
using QuoteFlow.Api.Asset.Search.Constants;
using QuoteFlow.Api.Asset.Search.Searchers;
using QuoteFlow.Api.Asset.Search.Searchers.Information;
using QuoteFlow.Api.Asset.Search.Searchers.Renderer;
using QuoteFlow.Api.Asset.Search.Searchers.Transformer;
using QuoteFlow.Api.Asset.Search.Searchers.Util;
using QuoteFlow.Api.Services;
using QuoteFlow.Core.Asset.CustomFields.Searchers.Information;
using QuoteFlow.Core.Asset.Index.Indexers;
using QuoteFlow.Core.Asset.Search.Searchers.Renderer;
using QuoteFlow.Core.Asset.Search.Searchers.Transformer;

namespace QuoteFlow.Core.Asset.Search.Searchers
{
    /// <summary>
    /// Responsible for searching for Creators
    /// </summary>
    public sealed class CreatorSearcher : AbstractInitializationSearcher
    {
        private const string NameKey = "Creator";

        public override ISearcherInformation<ISearchableField> SearchInformation { get; set; }
        public override ISearchInputTransformer SearchInputTransformer { get; set; }
        public override ISearchRenderer SearchRenderer { get; set; }
        
        public CreatorSearcher(IUserService userService)
        {
            var searchConstants = SystemSearchConstants.ForCreator();
            var userFitsNavigatorHelper = new UserFitsNavigatorHelper(userService);

            SearchInformation = new GenericSearcherInformation<ISearchableField>(searchConstants.SearcherId, NameKey, new List<IFieldIndexer> { new CreatorIndexer() }, FieldReference, SearcherGroupType.Asset);
            SearchRenderer = new CreatorSearchRenderer(NameKey, userService);
            SearchInputTransformer = new EnhancedUserSearchInputTransformer(searchConstants, userFitsNavigatorHelper, userService);
        }
    }
}