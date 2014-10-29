using System;
using System.Collections.Generic;
using QuoteFlow.Infrastructure.Util;
using QuoteFlow.Models.Search.Jql.Operand;
using QuoteFlow.Models.Search.Jql.Query.Clause;
using QuoteFlow.Models.Search.Jql.Query.Operand;

namespace QuoteFlow.Models.Search.Jql.Validator
{
    /// <summary>
    /// Used to perform validation over a <seealso cref="Query"/>. Uses
    /// <seealso cref="IClauseValidator"/>'s to validate the individual clauses and
    /// <seealso cref="OperandHandler.Validate(User, IOperand, TerminalClause)"/>
    /// to validate the operands.
    /// 
    /// @since v4.0
    /// </summary>
    public class ValidatorVisitor : IClauseVisitor<IMessageSet>
    {
        private readonly IValidatorRegistry validatorRegistry;
        private readonly JqlOperandResolver operandResolver;
        private readonly IOperatorUsageValidator operatorUsageValidator;
        private readonly User searcher;
        private readonly long? filterId;

        public ValidatorVisitor(IValidatorRegistry validatorRegistry, JqlOperandResolver operandResolver, OperatorUsageValidator operatorUsageValidator, User searcher, long? filterId)
        {
            this.validatorRegistry = validatorRegistry;
            this.operandResolver = operandResolver;
            this.operatorUsageValidator = operatorUsageValidator;
            this.searcher = searcher;
            this.filterId = filterId;
        }

        public IMessageSet Visit(AndClause andClause)
        {
            return GetMessagesFromSubClauses(andClause.Clauses);
        }

        public IMessageSet Visit(NotClause notClause)
        {
            return GetMessagesFromSubClauses(new List<IClause> {notClause.SubClause});
        }

        public IMessageSet Visit(OrClause orClause)
        {
            return GetMessagesFromSubClauses(orClause.Clauses);
        }

        public IMessageSet Visit(ITerminalClause clause)
        {
            ICollection<IClauseValidator> validators = validatorRegistry.GetClauseValidator(searcher, clause);
            IMessageSet messages = ValidateOperatorAndOperand(clause, validators);
            if (messages.HasAnyErrors())
            {
                return messages;
            }
            // Now validate the clause itself
            ValidateClause(clause, validators, messages);

            return messages;
        }

        public IMessageSet Visit(IWasClause clause)
        {
            ICollection<IClauseValidator> wasClauseValidators = validatorRegistry.GetClauseValidator(searcher, clause);
            IMessageSet messages = ValidateOperatorAndOperand(clause, wasClauseValidators);
            if (messages.HasAnyMessages())
            {
                return messages;
            }
            // Now validate the clause itself
            ValidateClause(clause, wasClauseValidators, messages);
            return messages;

        }

        public IMessageSet Visit(IChangedClause clause)
        {
//            ChangedClauseValidator changedClauseValidator = validatorRegistry.GetClauseValidator(searcher, clause);
//            return changedClauseValidator.Validate(searcher, clause);
            throw new NotImplementedException();
        }

        private IMessageSet ValidateOperatorAndOperand(ITerminalClause clause, ICollection<IClauseValidator> validators)
        {
            IMessageSet messages = new ListOrderedMessageSet();
            // It does not make sense to validate the operands or operators if the clause does not exist
            if (validators.Count <= 0) return messages;

            // Validate the operator makes sense from a global perspective
            if (ValidateOperator(clause, messages))
            {
                // Short-circuit if there are any errors, we are cool if there are warnings
                return messages;
            }

            // Validate the terminal clauses operands
            if (ValidateOperands(clause, messages))
            {
                // Short-circuit if there are any errors, we are cool if there are warnings
                return messages;
            }
            return messages;
        }

        private bool ValidateOperator(ITerminalClause clause, IMessageSet messages)
        {
            messages.AddMessageSet(operatorUsageValidator.Validate(searcher, clause));
            return messages.HasAnyErrors();
        }

        private bool ValidateOperands(ITerminalClause clause, IMessageSet messages)
        {
            var operand = clause.Operand;
            messages.AddMessageSet(operandResolver.Validate(searcher, operand, clause));
            return messages.HasAnyErrors();
        }

        private void ValidateClause(ITerminalClause clause, ICollection<IClauseValidator> validators, IMessageSet messages)
        {
            if (validators.Count > 0)
            {
                foreach (IClauseValidator validator in validators)
                {
                    IMessageSet messageSet;
//                    if (validator is SavedFilterClauseValidator)
//                    {
//                        messageSet = ((SavedFilterClauseValidator) validator).Validate(searcher, clause, filterId);
//                    }
//                    else
//                    {
//                        messageSet = validator.Validate(searcher, clause);
//                    }

                    messageSet = validator.Validate(searcher, clause);

                    messages.AddMessageSet(messageSet);
                }
            }
            else
            {
                messages.AddErrorMessage(searcher != null
                    ? string.Format("quoteflow.jql.validation.no.such.field: {0}", clause.Name)
                    : string.Format("quoteflow.jql.validation.no.such.field.anonymous: {0}", clause.Name));
            }
        }

        private IMessageSet GetMessagesFromSubClauses(IEnumerable<IClause> subClauses)
        {
            var messages = new ListOrderedMessageSet();

            foreach (IClause subClause in subClauses)
            {
                IMessageSet subMessages = subClause.Accept(this);
                if (subMessages != null)
                {
                    messages.AddMessageSet(subMessages);
                }
            }

            return messages;
        }

    }
}