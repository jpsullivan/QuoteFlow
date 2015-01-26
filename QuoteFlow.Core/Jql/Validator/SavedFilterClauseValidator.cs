//using System.Collections.Generic;
//using QuoteFlow.Infrastructure.Util;
//using QuoteFlow.Models.Search.Jql.Operand;
//using QuoteFlow.Models.Search.Jql.Query.Clause;
//using QuoteFlow.Models.Search.Jql.Query.Operand;
//
//namespace QuoteFlow.Models.Search.Jql.Validator
//{
//    public class SavedFilterClauseValidator : IClauseValidator
//    {
//        private readonly SupportedOperatorsValidator supportedOperatorsValidator;
//        private readonly SavedFilterResolver savedFilterResolver;
//        private readonly IJqlOperandResolver jqlOperandResolver;
//        private readonly SavedFilterCycleDetector savedFilterCycleDetector;
//
//        public virtual IMessageSet Validate(User searcher, ITerminalClause terminalClause, long? filterId)
//        {
//            var messageSet = supportedOperatorsValidator.Validate(searcher, terminalClause);
//            
//            if (messageSet.HasAnyErrors()) return messageSet;
//            IOperand operand = terminalClause.Operand;
//            IEnumerable<QueryLiteral> rawValues = jqlOperandResolver.GetValues(searcher, operand, terminalClause);
//            if (rawValues == null) return messageSet;
//
//            // Now lets look up the filter and see if it exists
//            //I18nHelper i18n = getI18n(searcher);
//
//            foreach (QueryLiteral rawValue in rawValues)
//            {
//                if (rawValue.IsEmpty)
//                {
//                    // we got an empty operand inside a multi value operand or function
//                    messageSet.AddErrorMessage(jqlOperandResolver.IsFunctionOperand(rawValue.SourceOperand)
//                        ? string.Format("quoteflow.jql.clause.field.does.not.support.empty.from.func: {0}, {1}", terminalClause.Name, rawValue.SourceOperand.Name)
//                        : string.Format("quoteflow.jql.clause.field.does.not.support.empty: {0}", terminalClause.Name));
//                    continue;
//                }
//
//                // Get a filter for each individual raw value and see if it resolves
//                IList<SearchRequest> matchingFilters = savedFilterResolver.getSearchRequest(searcher, Collections.singletonList(rawValue));
//                if (matchingFilters.Count == 0)
//                {
//                    if (rawValue.StringValue != null)
//                    {
//                        messageSet.AddErrorMessage(jqlOperandResolver.IsFunctionOperand(rawValue.SourceOperand)
//                            ? string.Format("quoteflow.jql.clause.no.value.for.name.from.function: {0}, {1}", rawValue.SourceOperand.Name, terminalClause.Name)
//                            : string.Format("quoteflow.jql.clause.no.value.for.name: {0}, {1}", terminalClause.Name, rawValue));
//                    }
//                    else if (rawValue.IntValue != null)
//                    {
//                        messageSet.AddErrorMessage(jqlOperandResolver.IsFunctionOperand(rawValue.SourceOperand)
//                            ? string.Format("quoteflow.jql.clause.no.value.for.name.from.function: {0}, {1}", rawValue.SourceOperand.Name, terminalClause.Name)
//                            : string.Format("quoteflow.jql.clause.no.value.for.id: {0}, {1}", terminalClause.Name, rawValue));
//                    }
//                }
//                else
//                {
//                    // Run through and make sure the filters don't contain any cycles
//                    foreach (SearchRequest matchingFilter in matchingFilters)
//                    {
//                        if (savedFilterCycleDetector.ContainsSavedFilterReference(searcher, false, matchingFilter, filterId))
//                        {
//                            messageSet.AddErrorMessage(string.Format("quoteflow.jql.saved.filter.detected.cycle: {0}, {1}, {2}", terminalClause.Name, rawValue, matchingFilter.Name));
//                        }
//                    }
//                }
//            }
//            return messageSet;
//        }
//        
//        public IMessageSet Validate(User searcher, ITerminalClause terminalClause)
//        {
//            return Validate(searcher, terminalClause, null);
//        }
//    }
//}