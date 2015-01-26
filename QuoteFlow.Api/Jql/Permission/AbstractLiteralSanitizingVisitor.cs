using System.Collections.Generic;
using QuoteFlow.Api.Jql.Operand;
using QuoteFlow.Api.Jql.Query.Clause;
using QuoteFlow.Api.Jql.Query.Operand;
using QuoteFlow.Api.Models;

namespace QuoteFlow.Api.Jql.Permission
{
    /// <summary>
    /// An abstract implementation of a <see cref="OperandSanitizingVisitor"/> that utilises
    /// a <see cref="ILiteralSanitizer"/> to convert a <see cref="SingleValueOperand"/>
    /// into its sanitised form.
    /// </summary>
    public abstract class AbstractLiteralSanitizingVisitor : DefaultOperandSanitizingVisitor
    {
        private readonly IJqlOperandResolver jqlOperandResolver;
        private readonly User user;
        private readonly ITerminalClause terminalClause;

        public AbstractLiteralSanitizingVisitor(IJqlOperandResolver jqlOperandResolver, User user,
            ITerminalClause terminalClause) : base(jqlOperandResolver, user)
        {
            this.jqlOperandResolver = jqlOperandResolver;
            this.user = user;
            this.terminalClause = terminalClause;
        }

        /// <summary>
        /// Sanitise the values stored in <see cref="SingleValueOperand"/>s.
        /// Utilise the <see cref="ILiteralSanitizer"/> to do the hard work.
        /// </summary>
        /// <param name="singleValueOperand"> the operand being visited. </param>
        /// <returns> the sanitised operand; never null. </returns>
        public virtual IOperand Visit(SingleValueOperand singleValueOperand)
        {
            // short circuit if we can't get any values back
            var literals = jqlOperandResolver.GetValues(user, singleValueOperand, terminalClause);
            if (literals == null)
            {
                return singleValueOperand;
            }

            LiteralSanitizerResult result = CreateLiteralSanitizer().SanitiseLiterals(literals);

            if (!result.Modified)
            {
                return singleValueOperand;
            }
            IList<QueryLiteral> resultantLiterals = result.Literals;
            if (resultantLiterals.Count == 1)
            {
                return new SingleValueOperand(resultantLiterals[0]);
            }
            return MultiValueOperand.OfQueryLiterals(resultantLiterals);
        }

        /// <summary>
        /// Creates an instance of a <see cref="ILiteralSanitizer"/> that knows how to sanitise the
        /// literals expected of this <see cref="DefaultOperandSanitizingVisitor"/>.
        /// </summary>
        /// <returns> a new instance of the sanitiser; never null. </returns>
        protected abstract ILiteralSanitizer CreateLiteralSanitizer();
    }
}