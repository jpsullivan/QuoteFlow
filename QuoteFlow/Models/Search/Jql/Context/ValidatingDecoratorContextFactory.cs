using System;
using QuoteFlow.Models.Search.Jql.Query.Clause;
using QuoteFlow.Models.Search.Jql.Validator;

namespace QuoteFlow.Models.Search.Jql.Context
{
    /// <summary>
    /// A <see cref="IClauseContextFactory"/> that wraps another ClauseContextFactory to ensure
    /// that the <see cref="GetClauseContext(User, ITerminalClause)"/> method
    /// on the wrapped object is only called when this passed TerminalClause passes usage validation.
    /// </summary>
    public sealed class ValidatingDecoratorContextFactory : IClauseContextFactory
    {
        private readonly IOperatorUsageValidator _usageValidator;
        private readonly IClauseContextFactory _delegatingContextFactory;

        public ValidatingDecoratorContextFactory(IOperatorUsageValidator usageValidator, IClauseContextFactory delegatingContextFactory)
        {
            if (usageValidator == null)
            {
                throw new ArgumentNullException("usageValidator");
            }

            if (delegatingContextFactory == null)
            {
                throw new ArgumentNullException("delegatingContextFactory");
            }

            _usageValidator = usageValidator;
            _delegatingContextFactory = delegatingContextFactory;
        }

        public IClauseContext GetClauseContext(User searcher, ITerminalClause terminalClause)
        {
            if (!_usageValidator.Check(searcher, terminalClause))
            {
                return ClauseContext.CreateGlobalClauseContext();
            }

            return _delegatingContextFactory.GetClauseContext(searcher, terminalClause);
        }
    }
}