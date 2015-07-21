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
        private bool seenQueryClauses;
        private string clauseName;
        private ITerminalClause terminal;

        public TextQueryValidatingVisitor(string clauseName)
            : base(clauseName)
        {
            if (clauseName == null)
            {
                throw new ArgumentNullException(nameof(clauseName));
            }

            this.clauseName = clauseName;
        }

        public virtual void Visit(OrClause orClause)
        {
            validPath = false;
        }

        public virtual void Visit(ITerminalClause terminalClause)
        {
            base.Visit(terminalClause);
            if (!terminalClause.Name.Equals(clauseName, StringComparison.InvariantCultureIgnoreCase)) return;

            if ((seenQueryClauses) || terminalClause.Operator != Operator.LIKE)
            {
                valid = false;
            }
            else
            {
                seenQueryClauses = true;
                terminal = !valid ? null : terminalClause;
            }
        }

        public virtual string GetTextTerminalValue(IJqlOperandResolver operandResolver, User user)
        {
            if (terminal != null)
            {
                QueryLiteral rawValue = operandResolver.GetSingleValue(user, terminal.Operand, terminal);
                if (rawValue != null && !rawValue.IsEmpty)
                {
                    return rawValue.AsString();
                }
            }
            return null;
        }
    }
}