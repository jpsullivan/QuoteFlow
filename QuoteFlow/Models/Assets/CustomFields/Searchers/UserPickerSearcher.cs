using QuoteFlow.Models.Assets.CustomFields.Searchers.Information;
using QuoteFlow.Models.Assets.Search.Searchers.Transformer;
using QuoteFlow.Models.Search.Jql.Operand;
using QuoteFlow.Services.Interfaces;

namespace QuoteFlow.Models.Assets.CustomFields.Searchers
{
    public class UserPickerSearcher
    {
        protected IUserService UserService { get; set; }
        protected JqlOperandResolver OperandResolver { get; set; }
        protected CustomFieldSearcherInformation SearcherInformation { get; set; }
        protected ISearchInputTransformer SearchInputTransformer { get; set; }

        private volatile ICustomFieldSearcherClauseHandler customFieldSearcherClauseHandler;

        private readonly UserPickerSearcher userPickerSearcher;
        //private CustomFieldInputHelper customFieldInputHelper;

        public UserPickerSearcher(JqlOperandResolver operandResolver, UserPickerSearcher userPickerSearcher, IUserService userService)
        {
            this.userPickerSearcher = userPickerSearcher;
            UserService = userService;
            //this.context = context;
            OperandResolver = operandResolver;
            //this.customFieldInputHelper = customFieldInputHelper;
        }

    }
}