﻿namespace QuoteFlow.Core.Jql.Validator
{
    /// <summary>
    /// Validate the changed clause against any field.
    /// </summary>
    public class ChangedClauseValidator
    {
//        private readonly IndexedChangeHistoryFieldManager indexedChangeHistoryFieldManager;
//        private readonly HistoryPredicateValidator historyPredicateValidator;
//
//        public ChangedClauseValidator(IndexedChangeHistoryFieldManager indexedChangeHistoryFieldManager, PredicateOperandResolver predicateOperandResolver, JqlDateSupport jqlDateSupport, JiraAuthenticationContext authContext, HistoryFieldValueValidator historyFieldValueValidator, JqlFunctionHandlerRegistry registry, UserManager userService)
//        {
//            this.indexedChangeHistoryFieldManager = indexedChangeHistoryFieldManager;
//            this.historyPredicateValidator = new HistoryPredicateValidator(authContext, predicateOperandResolver, jqlDateSupport, historyFieldValueValidator, registry, userService);
//        }
//
//        public virtual MessageSet Validate(User searcher, IChangedClause clause)
//        {
//            var messageSet = new MessageSet();
//            ValidateField(searcher, clause.Field, messageSet);
//            if (clause.Predicate != null)
//            {
//                messageSet.AddMessageSet(historyPredicateValidator.Validate(searcher, clause, clause.Predicate));
//            }
//            return messageSet;
//        }
//
//        private void ValidateField(User searcher, string fieldName, IMessageSet messages)
//        {
//            if (!indexedChangeHistoryFieldManager.IndexedChangeHistoryFieldNames.contains(fieldName.ToLower()))
//            {
//                messages.AddErrorMessage(string.Format("jira.jql.history.field.not.supported: {0}", fieldName));
//            }
//        }
    }
}