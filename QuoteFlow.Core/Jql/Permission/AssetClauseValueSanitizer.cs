using System;
using System.Collections.Generic;
using System.Linq;
using QuoteFlow.Api.Jql.Operand;
using QuoteFlow.Api.Jql.Operator;
using QuoteFlow.Api.Jql.Permission;
using QuoteFlow.Api.Jql.Query;
using QuoteFlow.Api.Jql.Query.Clause;
using QuoteFlow.Api.Jql.Query.Operand;
using QuoteFlow.Api.Jql.Util;
using QuoteFlow.Api.Models;

namespace QuoteFlow.Core.Jql.Permission
{
    /// <summary>
    /// Sanitizes clauses which has asset SKU's or ID's as their names.
    /// </summary>
    public class AssetClauseValueSanitizer : IClauseSanitizer
    {
        private readonly IJqlOperandResolver _jqlOperandResolver;
        private readonly IJqlAssetSupport _jqlAssetSupport;

        public AssetClauseValueSanitizer(IJqlOperandResolver jqlOperandResolver, IJqlAssetSupport jqlAssetSupport)
        {
            if (jqlOperandResolver == null) throw new ArgumentNullException(nameof(jqlOperandResolver));
            if (jqlAssetSupport == null) throw new ArgumentNullException(nameof(jqlAssetSupport));

            _jqlOperandResolver = jqlOperandResolver;
            _jqlAssetSupport = jqlAssetSupport;
        }

        /// <summary>
        /// Note: we cannot assume that the <see cref="AssetOperandSanitizingVisitor"/> returns the 
        /// same type of operand that went in, because assets can expand to more than one literal. 
        /// </summary>
        /// <param name="user"></param>
        /// <param name="clause">The clause to sanitize.</param>
        /// <returns>The sanitized clause; never null</returns>
        public IClause Sanitize(User user, ITerminalClause clause)
        {
            var clauseName = clause.Name;
            var @operator = clause.Operator;

            var visitor = CreateOperandVisitor(user, clause);
            var originalOperand = clause.Operand;
            var sanitizedOperand = originalOperand.Accept(visitor);

            if (originalOperand.Equals(sanitizedOperand))
            {
                return clause;
            }

            // if the have the same type of operand, we can reuse the operator
            if (originalOperand.GetType() == sanitizedOperand.GetType())
            {
                return new TerminalClause(clauseName, @operator, sanitizedOperand);
            }

            // if we had a SingleValueOperand and now a MultiValueOperand, we need to massage the operator
            if (originalOperand is SingleValueOperand && sanitizedOperand is MultiValueOperand)
            {
                // if the operator was positive equality, we just need to change it to "IN"
                if (OperatorClasses.PositiveEqualityOperators.Contains(@operator))
                {
                    return new TerminalClause(clauseName, Operator.IN, sanitizedOperand);
                }

                // if the operator was negative equality, we just need to change it into "NOT IN"
                if (OperatorClasses.NegativeEqualityOperators.Contains(@operator))
                {
                    return new TerminalClause(clauseName, Operator.NOT_IN, sanitizedOperand);
                }

                // if ther roperator was relational, we need to build up multiple clauses and OR them together.
                // Note: there is a known issue with sanitizing clauses with relational operators, since
                // the asset id cannot be used in a relational clause.
                if (OperatorClasses.RelationalOnlyOperators.Contains(@operator))
                {
                    var multiOperand = sanitizedOperand as MultiValueOperand;
                    var clauses = new List<IClause>(multiOperand.Values.Count());
                    clauses.AddRange(multiOperand.Values.Select(operand => new TerminalClause(clauseName, @operator, operand)));
                    return new OrClause(clauses);
                }
            }

            // if we got here, we have no idea how to sanitize this properly - just return the original
            Console.WriteLine(
                "Could not figure out how to reconcile the original operand '{0}' and sanitized operand {1}",
                originalOperand, sanitizedOperand);
            return clause;
        }

        protected bool Equals(AssetClauseValueSanitizer other)
        {
            return Equals(_jqlOperandResolver, other._jqlOperandResolver) && Equals(_jqlAssetSupport, other._jqlAssetSupport);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((AssetClauseValueSanitizer) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((_jqlOperandResolver != null ? _jqlOperandResolver.GetHashCode() : 0)*397) ^ (_jqlAssetSupport != null ? _jqlAssetSupport.GetHashCode() : 0);
            }
        }

        private class AssetOperandSanitizingVisitor : AbstractLiteralSanitizingVisitor
        {
            private readonly User _user;
            private readonly IJqlAssetSupport _jqlAssetSupport;

            internal AssetOperandSanitizingVisitor(IJqlOperandResolver jqlOperandResolver, User user, ITerminalClause terminalClause, IJqlAssetSupport jqlAssetSupport) 
                : base(jqlOperandResolver, user, terminalClause)
            {
                _user = user;
                _jqlAssetSupport = jqlAssetSupport;
            }

            protected override ILiteralSanitizer CreateLiteralSanitizer()
            {
                return new AssetLiteralSanitizer(_jqlAssetSupport, _user);
            }
        }

        private AssetOperandSanitizingVisitor CreateOperandVisitor(User user, ITerminalClause terminalClause)
        {
            return new AssetOperandSanitizingVisitor(_jqlOperandResolver, user, terminalClause, _jqlAssetSupport);
        }
    }
}