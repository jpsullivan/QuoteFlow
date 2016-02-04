using System;
using System.Collections.Generic;
using System.Linq;
using QuoteFlow.Api.Asset.Search.Managers;
using QuoteFlow.Api.Jql.Operand;
using QuoteFlow.Api.Jql.Permission;
using QuoteFlow.Api.Jql.Query.Clause;
using QuoteFlow.Api.Jql.Query.Operand;
using QuoteFlow.Api.Models;

namespace QuoteFlow.Core.Jql.Permission
{
    /// <summary>
    /// A visitor for converting a clause into its sanitized form.
    /// </summary>
    public class ClauseSanitizingVisitor : IClauseVisitor<IClause>
    {
        private readonly ISearchHandlerManager _searchHandlerManager;
        private readonly IJqlOperandResolver _jqlOperandResolver;
        private readonly User _user;

        public ClauseSanitizingVisitor(ISearchHandlerManager searchHandlerManager, IJqlOperandResolver jqlOperandResolver, User user)
        {
            if (searchHandlerManager == null) throw new ArgumentNullException(nameof(searchHandlerManager));
            if (jqlOperandResolver == null) throw new ArgumentNullException(nameof(jqlOperandResolver));

            _searchHandlerManager = searchHandlerManager;
            _jqlOperandResolver = jqlOperandResolver;
            _user = user;
        }

        public IClause Visit(AndClause andClause)
        {
            return new AndClause(SanitizeChildren(andClause));
        }

        public IClause Visit(NotClause notClause)
        {
            return new NotClause(notClause.SubClause.Accept(this));
        }

        public IClause Visit(OrClause orClause)
        {
            return new OrClause(SanitizeChildren(orClause));
        }

        public IClause Visit(ITerminalClause clause)
        {
            // if we don't get any handlers back, this means the user does not have permission
            // to use the clause, so just return the input
            var handlers = _searchHandlerManager.GetClauseHandler(_user, clause.Name);
            if (!handlers.Any())
            {
                return clause;
            }

            // first we want to sanitize all operands with the DefaultOperandSanitizingVisitor
            // as it uses a strategy that should be applied across all fields.
            clause = SanitizeOperands(clause);

            // we only care about unique sanitized clauses, so use a set
            var newClauses = new HashSet<IClause>();
            foreach (var clauseHandler in handlers)
            {
                newClauses.Add(clauseHandler.PermissionHandler.Sanitize(_user, clause));
            }

            return newClauses.Count == 1 ? newClauses.First() : new OrClause(newClauses);
        }

        public IClause Visit(IWasClause clause)
        {
            throw new System.NotImplementedException();
        }

        public IClause Visit(IChangedClause clause)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Important note: we are making a big assumption here that the <see cref="DefaultOperandSanitizingVisitor"/>
        /// will always return the same kind of operand back after sanitizing. This is because it only 
        /// mutates function arguments and multi-value operands that contains function operands. In either
        /// case, the multiplicity of the operand does not change after sanitizing. Because of this, we 
        /// blindly reuse the original operator from the input clause.
        /// 
        /// If this assumption ever changes, I'm gonna have to revisit this shit.
        /// </summary>
        /// <param name="clause">The clause to sanitize.</param>
        /// <returns>The sanitized clause; never null.</returns>
        private ITerminalClause SanitizeOperands(ITerminalClause clause)
        {
            var originalOperand = clause.Operand;
            var sanitizedOperand = SanitizedOperand(originalOperand);
            if (originalOperand.Equals(sanitizedOperand))
            {
                return clause;
            }

            return new TerminalClause(clause.Name, clause.Operator, sanitizedOperand);
        }

        private IOperand SanitizedOperand(IOperand original)
        {
            var visitor = CreateOperandVisitor(_user);
            return original.Accept(visitor);
        }

        private DefaultOperandSanitizingVisitor CreateOperandVisitor(User user)
        {
            return new DefaultOperandSanitizingVisitor(_jqlOperandResolver, user);
        }

        private IList<IClause> SanitizeChildren(IClause parentClause)
        {
            var newClauses = new List<IClause>(parentClause.Clauses.Count());
            newClauses.AddRange(parentClause.Clauses.Select(clause => clause.Accept(this)));
            return newClauses;
        } 
    }
}