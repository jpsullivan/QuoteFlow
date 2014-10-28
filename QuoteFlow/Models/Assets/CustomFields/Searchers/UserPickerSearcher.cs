using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using QuoteFlow.Models.Assets.CustomFields.Searchers.Information;
using QuoteFlow.Models.Assets.Search.Searchers.Transformer;
using QuoteFlow.Models.Search.Jql.Operand;
using QuoteFlow.Services.Interfaces;

namespace QuoteFlow.Models.Assets.CustomFields.Searchers
{
    public class UserPickerSearcher
    {
        #region IoC

        protected IUserService UserService { get; set; }

        #endregion

        protected JqlOperandResolver OperandResolver { get; set; }
        protected CustomFieldSearcherInformation SearcherInformation { get; set; }
        protected ISearchInputTransformer SearchInputTransformer { get; set; }

        private volatile ICustomFieldSearcherClauseHandler customFieldSearcherClauseHandler;

        private readonly UserPickerSearchService userPickerSearchService;
        private CustomFieldInputHelper customFieldInputHelper;

        public UserPickerSearcher(UserResolver userResolver, JqlOperandResolver operandResolver, JiraAuthenticationContext context, UserConverter userConverter, UserPickerSearchService userPickerSearchService, CustomFieldInputHelper customFieldInputHelper, IUserService userService, FieldVisibilityManager fieldVisibilityManager)
        {
            this.userPickerSearchService = userPickerSearchService;
            UserService = userService;
            this.context = notNull("context", context);
            OperandResolver = operandResolver;
            this.customFieldInputHelper = customFieldInputHelper;
        }

    }
}