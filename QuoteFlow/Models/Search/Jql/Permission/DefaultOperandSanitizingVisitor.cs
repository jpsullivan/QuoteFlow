using System;
using System.Collections.Generic;
using QuoteFlow.Models.Search.Jql.Operand;
using QuoteFlow.Models.Search.Jql.Query.Operand;

namespace QuoteFlow.Models.Search.Jql.Permission
{
    /// <summary>
    /// The default strategy for sanitizing an arbitrary <see cref="IOperand"/> is:
    /// 
    /// - <see cref="EmptyOperand"/>s do not need sanitising;
    /// - <see cref="FunctionOperand"/>s have their arguments sanitized by <see cref="IJqlOperandResolver"/>;
    /// - <see cref="MultiValueOperand"/>s must have their children sanitized and potentially recombined into a new MultiValueOperand instance.
    /// - <see cref="SingleValueOperand"/>s should be sanitized depending on the context (what field's values we are dealing with). But we don't know about that here, so we just return the operand back.
    /// 
    /// In general, if no sanitization is required, the input <see cref="IOperand"/> should be returned.
    /// </summary>
    public class DefaultOperandSanitizingVisitor : IOperandVisitor<IOperand>
    {
        private readonly IJqlOperandResolver jqlOperandResolver;
        private readonly User searcher;

        public DefaultOperandSanitizingVisitor(IJqlOperandResolver jqlOperandResolver, User searcher)
        {
            if (jqlOperandResolver == null)
            {
                throw new ArgumentNullException("jqlOperandResolver");;
            }

            this.jqlOperandResolver = jqlOperandResolver;
            this.searcher = searcher;
        }

        public virtual IOperand Visit(EmptyOperand empty)
        {
            return empty;
        }

        public virtual IOperand Visit(FunctionOperand function)
        {
            return jqlOperandResolver.SanitizeFunctionOperand(searcher, function);
        }

        public virtual IOperand Visit(MultiValueOperand originalMulti)
        {
            bool isModified = false;

            // keep a set of operands: if we're going to sanitise the operand, we may as well optimise and remove duplicates.
            ISet<IOperand> sanitisedOperands = new HashSet<IOperand>();
            foreach (IOperand childOperand in originalMulti.Values)
            {
                var sanitisedChild = childOperand.Accept(this);
                if (!sanitisedChild.Equals(childOperand))
                {
                    isModified = true;
                }
                sanitisedOperands.Add(sanitisedChild);
            }

            return isModified ? new MultiValueOperand(sanitisedOperands) : originalMulti;
        }

        public virtual IOperand Visit(SingleValueOperand singleValueOperand)
        {
            // we are too dumb to know how to sanitize SingleValueOperands
            return singleValueOperand;
        }
    }
}