using System.Collections;
using System.Collections.Generic;
using Ninject;
using QuoteFlow.Api.Asset.Search;
using QuoteFlow.Api.Asset.Search.Constants;
using QuoteFlow.Api.Asset.Search.Searchers.Renderer;
using QuoteFlow.Api.Asset.Transport;
using QuoteFlow.Api.Jql.Query;
using QuoteFlow.Api.Models;
using QuoteFlow.Api.Services;
using QuoteFlow.Core.Asset.Search.Searchers.Transformer;
using QuoteFlow.Core.DependencyResolution;

namespace QuoteFlow.Core.Asset.Search.Searchers.Renderer
{
    /// <summary>
    /// A search renderer for the user fields.
    /// </summary>
    public abstract class AbstractUserSearchRenderer : AbstractSearchRenderer, ISearchRenderer
    {
        public const string SELECT_LIST_NONE = "select.list.none";
        public const string SELECT_LIST_USER = "select.list.user";
        public const string SELECT_LIST_GROUP = "select.list.group";

        private readonly UserFieldSearchConstants _searchConstants;
        private readonly string _emptySelectFlag;
        private readonly string _nameKey;
        private readonly IUserService _userService;
        private readonly IUserSearcherHelperService _userSearcherHelperService;

        public AbstractUserSearchRenderer(UserFieldSearchConstantsWithEmpty searchConstants, string nameKey, IUserService userManager) 
            : this(searchConstants, searchConstants.EmptySelectFlag, nameKey, userManager, Container.Kernel.TryGet<IUserSearcherHelperService>())
        {
        }

        private AbstractUserSearchRenderer(UserFieldSearchConstants searchConstants, string emptySelectFlag, 
            string nameKey, IUserService userService, IUserSearcherHelperService searchHelperService) 
            : base(searchConstants.SearcherId, nameKey)
        {
            _emptySelectFlag = emptySelectFlag;
            _searchConstants = searchConstants;
            _nameKey = nameKey;
            _userService = userService;
            _userSearcherHelperService = searchHelperService;
        }

        public override string GetEditHtml(User user, ISearchContext searchContext, IFieldValuesHolder fieldValuesHolder, IDictionary<string, object> displayParameters)
        {
            var velocityParams = GetDisplayParams(user, searchContext, fieldValuesHolder, displayParameters);
            velocityParams["selectListOptions"] = SelectedListOptions(user);
            return RenderEditTemplate("UserSearcherEdit", velocityParams);
        }

        public override string GetViewHtml(User user, ISearchContext searchContext, IFieldValuesHolder fieldValuesHolder, IDictionary<string, object> displayParameters)
        {
            IDictionary<string, object> velocityParams = GetDisplayParams(user, searchContext, fieldValuesHolder, displayParameters);
            return RenderViewTemplate("UserSearcherView", velocityParams);
        }

        public override bool IsRelevantForQuery(User user, IQuery query)
        {
            return IsRelevantForQuery(_searchConstants.JqlClauseNames, query);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="searcher">The user performing the action</param>
        /// <returns>The select list options that are displayed for this user searcher (e.g. SpecificUser, CurrentUser...)</returns>
        protected abstract List<IDictionary<string, string>> SelectedListOptions(User searcher); 

        /// <summary>
        /// Gets the key for the text that describes an empty value for this searcher.
        /// </summary>
        protected abstract string EmptyValueKey { get; }

        protected override IDictionary<string, object> GetDisplayParams(User searcher, ISearchContext searchContext, IFieldValuesHolder fieldValuesHolder,
            IDictionary<string, object> displayParameters)
        {
            var templateParams = base.GetDisplayParams(searcher, searchContext, fieldValuesHolder, displayParameters);

            templateParams.Add("emptyValueKey", EmptyValueKey);
            templateParams.Add("name", _nameKey);
            templateParams.Add("userField", _searchConstants.FieldUrlParameter);
            templateParams.Add("userSelect", _searchConstants.SelectUrlParameter);
            templateParams.Add("hasPermissionToPickUsers", true);

            var specialParams = GetSpecialDisplayParams(fieldValuesHolder, searcher, searchContext);
            foreach (var specialParam in specialParams)
            {
                if (templateParams.ContainsKey(specialParam.Key))
                {
                    // update it
                    templateParams[specialParam.Key] = specialParam.Value;
                }
                else
                {
                    templateParams.Add(specialParam.Key, specialParam.Value);
                }
            }

            return templateParams;
        }

        private IDictionary<string, object> GetSpecialDisplayParams(IFieldValuesHolder fieldValuesHolder, User user,
            ISearchContext searchContext)
        {
            string key = _searchConstants.FieldUrlParameter;
            var values = new List<UserSearchInput>();

            object fieldValue;
            if (fieldValuesHolder.TryGetValue(key, out fieldValue))
            {
                values = fieldValue as List<UserSearchInput>;
            }

            var templateParams = new Dictionary<string, object>();
            templateParams.Add("avatarSize", 16);
            templateParams.Add("hasCurrentUser", false);
            templateParams.Add("hasEmpty", false);

            foreach (var suggestedUser in AddUserSuggestionParams(fieldValuesHolder, user, searchContext, ExtractUserNames(values)))
            {
                templateParams.Add(suggestedUser.Key, suggestedUser.Value);
            }

            if (values != null)
            {
                foreach (var userSearchInput in values)
                {
                    if (userSearchInput.CurrentUser)
                    {
                        templateParams["hasCurrentUser"] = true;
                    }
                    if (userSearchInput.Empty)
                    {
                        templateParams["hasEmpty"] = true;
                    }
                    if (userSearchInput.Group)
                    {
                        // todo when groups are a thing
                    }
                    if (userSearchInput.User)
                    {
                        userSearchInput.Object = _userService.GetUser(userSearchInput.Value);
                    }
                }

                values.Sort();
            }

            templateParams.Add("values", values);
            return templateParams;
        }

        protected IDictionary<string, object> AddUserSuggestionParams(IFieldValuesHolder fieldValuesHolder, User user,
            ISearchContext searchContext, List<string> selectedUsers)
        {
            var templateParams = new Dictionary<string, object>();
            _userSearcherHelperService.AddUserSuggestionParams(user, selectedUsers, templateParams);
            return templateParams;
        }

        private List<string> ExtractUserNames(IReadOnlyCollection<UserSearchInput> values)
        {
            var result = new List<string>();
            if (values != null)
            {
                foreach (var userSearchInput in values)
                {
                    if (userSearchInput.User)
                    {
                        result.Add(userSearchInput.Value);
                    }
                }
            }

            return result;
        } 
    }
}