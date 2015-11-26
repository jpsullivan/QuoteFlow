using System.Collections.Generic;
using QuoteFlow.Api.Asset.Index;
using QuoteFlow.Api.Asset.Search;
using QuoteFlow.Api.Asset.Search.Constants;
using QuoteFlow.Api.Asset.Search.Searchers.Renderer;
using QuoteFlow.Api.Models;
using QuoteFlow.Api.Services;

namespace QuoteFlow.Core.Asset.Search.Searchers.Renderer
{
    public class CreatorSearchRenderer : AbstractUserSearchRenderer, ISearchRenderer
    {
        public CreatorSearchRenderer(string nameKey, IUserService userService)
            : base(SystemSearchConstants.ForCreator(), nameKey, userService)
        {
        }

        public override bool IsShown(User user, ISearchContext searchContext)
        {
            return true;
        }

        protected override List<IDictionary<string, string>> SelectedListOptions(User searcher)
        {
            var creatorTypes = new List<IDictionary<string, string>>
            {
                new Dictionary<string, string>()
                {
                    {"value", "Any User"},
                    {"key", null},
                    {"related", "select.list.none"}
                },
                new Dictionary<string, string>()
                {
                    {"value", "Current User"},
                    {"key", DocumentConstants.AssetCurrentUser},
                    {"related", "select.list.none"}
                },
                new Dictionary<string, string>()
                {
                    {"value", "Specify User"},
                    {"key", DocumentConstants.SpecificUser},
                    {"related", "select.list.user"}
                }
            };

            return creatorTypes;
        }

        protected override string EmptyValueKey => "common.concepts.anonymous.creator";
    }
}