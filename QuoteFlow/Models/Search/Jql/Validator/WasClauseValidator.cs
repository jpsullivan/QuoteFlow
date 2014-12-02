using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using QuoteFlow.Infrastructure.Util;
using QuoteFlow.Models.Assets.Search.Managers;
using QuoteFlow.Models.Search.Jql.Operand;
using QuoteFlow.Models.Search.Jql.Query.Clause;
using QuoteFlow.Models.Search.Jql.Query.Operand;

namespace QuoteFlow.Models.Search.Jql.Validator
{
    /// <summary>
    /// Validate the was clause against any field.
    /// </summary>
    public class WasClauseValidator : IClauseValidator
    {
//        private readonly SupportedOperatorsValidator supportedOperatorsValidator;
//        private readonly IJqlOperandResolver operandResolver;
//        private readonly IndexedChangeHistoryFieldManager indexedChangeHistoryFieldManager;
//        private readonly ISearchHandlerManager searchHandlerManager;
//        private readonly ChangeHistoryManager changeHistoryManager;
//        private readonly HistoryPredicateValidator historyPredicateValidator;
//        private readonly JqlChangeItemMapping jqlChangeItemMapping;
//        private readonly ChangeHistoryFieldConfigurationManager configurationManager;
//        private readonly HistoryFieldValueValidator historyFieldValueValidator;
//
//        public WasClauseValidator(IJqlOperandResolver operandResolver, ISearchHandlerManager searchHandlerManager, IndexedChangeHistoryFieldManager indexedChangeHistoryFieldManager, ChangeHistoryManager changeHistoryManager, PredicateOperandResolver predicateOperandResolver, JqlDateSupport jqlDateSupport, JiraAuthenticationContext authContext, JqlChangeItemMapping jqlChangeItemMapping, ChangeHistoryFieldConfigurationManager configurationManager, HistoryFieldValueValidator historyFieldValueValidator, UserManager userManager, JqlFunctionHandlerRegistry registry)
//        {
//            this.searchHandlerManager = searchHandlerManager;
//            this.operandResolver = operandResolver;
//            this.indexedChangeHistoryFieldManager = indexedChangeHistoryFieldManager;
//            this.jqlChangeItemMapping = jqlChangeItemMapping;
//            this.configurationManager = configurationManager;
//            this.historyFieldValueValidator = historyFieldValueValidator;
//            this.supportedOperatorsValidator = SupportedOperatorsValidator;
//            this.changeHistoryManager = changeHistoryManager;
//            this.historyPredicateValidator = new HistoryPredicateValidator(authContext, predicateOperandResolver, jqlDateSupport, historyFieldValueValidator, registry, userManager);
//        }


        public IMessageSet Validate(User searcher, ITerminalClause terminalClause)
        {
            throw new NotImplementedException();
        }
    }
}