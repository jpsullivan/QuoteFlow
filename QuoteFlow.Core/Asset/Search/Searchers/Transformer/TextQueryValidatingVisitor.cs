using System;
using QuoteFlow.Api.Asset.Search.Searchers.Transformer;
using QuoteFlow.Api.Jql.Operand;
using QuoteFlow.Api.Jql.Query;
using QuoteFlow.Api.Jql.Query.Clause;
using QuoteFlow.Api.Models;

namespace QuoteFlow.Core.Asset.Search.Searchers.Transformer
{
    /// <summary>
    /// Checks that the query fits the expectations of the Search UI
    /// </summary>
    internal class TextQueryValidatingVisitor : SimpleNavigatorCollectorVisitor, IClauseVisitor<object>
    {
        private bool _seenQueryClauses;
        private readonly string _clauseName;
        private ITerminalClause _terminal;

        public TextQueryValidatingVisitor(string clauseName)
            : base(clauseName)
        {
            if (clauseName == null)
            {
                throw new ArgumentNullException(nameof(clauseName));
            }

            _clauseName = clauseName;
        }

        public virtual void Visit(OrClause orClause)
        {
            validPath = false;
        }

        public virtual void Visit(ITerminalClause terminalClause)
        {
            base.Visit(terminalClause);
            if (!terminalClause.Name.Equals(_clauseName, StringComparison.InvariantCultureIgnoreCase)) return;

            if ((_seenQueryClauses) || terminalClause.Operator != Operator.LIKE)
            {
                valid = false;
            }
            else
            {
                _seenQueryClauses = true;
                _terminal = !valid ? null : terminalClause;
            }
        }

        public virtual string GetTextTerminalValue(IJqlOperandResolver operandResolver, User user)
        {
            if (_terminal != null)
            {
                QueryLiteral rawValue = operandResolver.GetSingleValue(user, _terminal.Operand, _terminal);
                if (rawValue != null && !rawValue.IsEmpty)
                {
                    return rawValue.AsString();
                }
            }
            return null;
        }
    }
}