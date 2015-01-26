using QuoteFlow.Api.Asset.CustomFields.Searchers.Information;
using QuoteFlow.Api.Asset.Search.Searchers.Transformer;
using QuoteFlow.Api.Jql.Operand;
using QuoteFlow.Api.Services;

namespace QuoteFlow.Api.Asset.CustomFields.Searchers
{
    public class UserPickerSearcher
    {
        protected IUserService UserService { get; set; }
        protected IJqlOperandResolver OperandResolver { get; set; }
        protected CustomFieldSearcherInformation SearcherInformation { get; set; }
        protected ISearchInputTransformer SearchInputTransformer { get; set; }

        private volatile ICustomFieldSearcherClauseHandler customFieldSearcherClauseHandler;

        private readonly UserPickerSearcher userPickerSearcher;
        //private CustomFieldInputHelper customFieldInputHelper;

        public UserPickerSearcher(IJqlOperandResolver operandResolver, UserPickerSearcher userPickerSearcher, IUserService userService)
        {
            this.userPickerSearcher = userPickerSearcher;
            UserService = userService;
            //this.context = context;
            OperandResolver = operandResolver;
            //this.customFieldInputHelper = customFieldInputHelper;
        }
    }
}