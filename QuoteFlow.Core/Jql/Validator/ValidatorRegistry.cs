using System;
using System.Collections.Generic;
using System.Linq;
using QuoteFlow.Api.Asset.Search.Managers;
using QuoteFlow.Api.Jql.Query.Clause;
using QuoteFlow.Api.Jql.Validator;
using QuoteFlow.Api.Models;

namespace QuoteFlow.Core.Jql.Validator
{
    public class ValidatorRegistry : IValidatorRegistry
    {
        private readonly ISearchHandlerManager manager;
		private readonly IList<IClauseValidator> wasClauseValidators = new List<IClauseValidator>();
		private readonly ChangedClauseValidator changedClauseValidator;

        public ValidatorRegistry(ISearchHandlerManager manager, WasClauseValidator wasClauseValidator, ChangedClauseValidator changedClauseValidator)
		{
			this.manager = manager;
			this.wasClauseValidators.Add(wasClauseValidator);
			this.changedClauseValidator = changedClauseValidator;
		}

        public ICollection<IClauseValidator> GetClauseValidator(User searcher, ITerminalClause clause)
        {
            if (clause == null)
            {
                throw new ArgumentNullException("clause");
            }
            
            var clauseHandlers = manager.GetClauseHandler(searcher, clause.Name).ToList();

            // Collect the factories.
            // JRA-23141 : We avoid using a lazy transformed collection here because it gets accessed multiple times
            // and size() in particular is slow.
            var clauseValidators = new List<IClauseValidator>(clauseHandlers.Count());
            clauseValidators.AddRange(clauseHandlers.Select(clauseHandler => clauseHandler.Validator));
            return clauseValidators;
        }

        public ICollection<IClauseValidator> GetClauseValidator(User searcher, IWasClause clause)
        {
            return new List<IClauseValidator>(wasClauseValidators);
        }

        public ChangedClauseValidator GetClauseValidator(User searcher, IChangedClause clause)
        {
            return changedClauseValidator;
        }
    }
}