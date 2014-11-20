using System;
using System.Collections.Generic;
using System.Linq;
using QuoteFlow.Infrastructure.Extensions;
using QuoteFlow.Models.Search.Jql.Operand;
using QuoteFlow.Models.Search.Jql.Query;
using QuoteFlow.Models.Search.Jql.Query.Clause;
using QuoteFlow.Models.Search.Jql.Query.Operand;
using QuoteFlow.Models.Search.Jql.Validator;

namespace QuoteFlow.Models.Search.Jql.Context
{
    /// <summary>
    /// Decorates a <see cref="IClauseContextFactory"/> to ensure that:
    /// 
    /// <pre>
    /// context(k in (a, b, c)) &lt;=&gt; context(k = a or k = b or k = c)
    /// context(k not in (a, b, c)) &lt;=&gt; context(k != a and k != b and k != c)
    /// </pre>
    /// 
    /// It does this by intercepting calls to <see cref="GetClauseContext(User, ITerminalClause)"/>
    /// with a terminal clause that contains the <see cref="Operator.IN"/> or <see cref="Operator.NOT_IN"/>
    /// op and converts it into equivalent multiple calls to the contextFactory factory.
    /// </summary>
    public class MultiClauseDecoratorContextFactory : IClauseContextFactory
    {
        private readonly IClauseContextFactory contextFactory;
        private readonly IJqlOperandResolver jqlOperandResolver;

        public MultiClauseDecoratorContextFactory(IJqlOperandResolver jqlOperandResolver, IClauseContextFactory contextFactory)
        {
            if (jqlOperandResolver == null)
            {
                throw new ArgumentNullException("jqlOperandResolver");
            }

            if (contextFactory == null)
            {
                throw new ArgumentNullException("contextFactory");
            }

            this.jqlOperandResolver = jqlOperandResolver;
            this.contextFactory = contextFactory;
        }


        public IClauseContext GetClauseContext(User searcher, ITerminalClause terminalClause)
        {
            try
            {
                var operand = terminalClause.Operand;
                var literals = jqlOperandResolver.GetValues(searcher, operand, terminalClause).ToList();
                if (literals.AnySafe())
                {
                    return ClauseContext.CreateGlobalClauseContext();
                }

                var convertedOperator = ConvertListOperator(terminalClause.Operator);
                if (convertedOperator == null || !jqlOperandResolver.IsListOperand(operand))
                {
                    return contextFactory.GetClauseContext(searcher, terminalClause);
                }
                else
                {
                    ISet<IClauseContext> ctxs = new HashSet<IClauseContext>();
                    var operands = new MultiValueOperand(literals.ToArray());
                    var listClause = new TerminalClause(terminalClause.Name, (Operator) convertedOperator, operands);
                    ctxs.Add(contextFactory.GetClauseContext(searcher, listClause));

                    if (convertedOperator == Operator.EQUALS)
                    {
                        return ctxs.Union();
                    }
                    else
                    {
                        return ctxs.Intersect();
                    }
                }
            }
            finally
            {
            }
        }

        private static Operator? ConvertListOperator(Operator op)
        {
            if (op == Operator.IN)
            {
                return Operator.EQUALS;
            }
            if (op == Operator.NOT_IN)
            {
                return Operator.NOT_EQUALS;
            }
            return null;
        }

        /// <summary>
        /// Factory to create a <seealso cref="MultiClauseDecoratorContextFactory"/> given a
        /// <seealso cref="com.atlassian.jira.jql.context.ClauseContextFactory"/> to wrap.
        /// 
        /// @since 4.0
        /// </summary>
        public class Factory
        {
            internal readonly IOperatorUsageValidator validator;
            internal readonly IJqlOperandResolver resolver;

            public Factory(IOperatorUsageValidator validator, IJqlOperandResolver resolver)
            {
                if (validator == null)
                {
                    throw new ArgumentNullException("validator");
                }

                if (resolver == null)
                {
                    throw new ArgumentNullException("resolver");
                }

                this.validator = validator;
                this.resolver = resolver;
            }

            /// <summary>
            /// Same as calling {@code create(contextFactory, true)}.
            /// </summary>
            /// <param name="contextFactory">The ClauseContextFactory to wrap.</param>
            /// <returns>The wrapped clause context factory.</returns>
            public virtual IClauseContextFactory Create(IClauseContextFactory contextFactory)
            {
                return Create(contextFactory, true);
            }

            /// <summary>
            /// Wrap the passed <see cref="IClauseContextFactory"/> in a <see cref="MultiClauseDecoratorContextFactory"/>.
            /// When validating is set to true, the returned ClauseContextFactory will also perform a validation step on the passed clause when
            /// generating the clause context.
            /// </summary>
            /// <param name="contextFactory">The factory to wrap. Cannot be null.</param>
            /// <param name="validating">True if the returned factory should perform a validation step, or false otherwise.</param>
            /// <returns>The wrapped clause context factory.</returns>
            public virtual IClauseContextFactory Create(IClauseContextFactory contextFactory, bool validating)
            {
                if (contextFactory == null)
                {
                    throw new ArgumentNullException("contextFactory");
                }

                IClauseContextFactory factory = new MultiClauseDecoratorContextFactory(resolver, contextFactory);
                if (validating)
                {
                    factory = new ValidatingDecoratorContextFactory(validator, factory);
                }
                return factory;
            }
        }
    }
}