using System;
using System.Collections.Generic;
using System.Linq;
using QuoteFlow.Models.Assets.Search.Constants;
using QuoteFlow.Models.Search.Jql.Operand;
using QuoteFlow.Models.Search.Jql.Query.Clause;
using QuoteFlow.Models.Search.Jql.Query.Operand;

namespace QuoteFlow.Models.Search.Jql.Query
{
    /// <summary>
    /// Can generate queries for issue constant clauses.
    /// </summary>
    public class GenericClauseQueryFactory : ClauseQueryFactory
    {
        private readonly IJqlOperandResolver operandResolver;
        private readonly IList<IOperatorSpecificQueryFactory> operatorQueryFactories;
        private readonly string documentFieldName;

        public GenericClauseQueryFactory(string documentFieldName, IEnumerable<IOperatorSpecificQueryFactory> operatorQueryFactories, IJqlOperandResolver operandResolver)
        {
            if (documentFieldName == null)
            {
                throw new ArgumentNullException("documentFieldName");
            }

            if (operandResolver == null)
            {
                throw new ArgumentNullException("operandResolver");
            }

            if (operatorQueryFactories == null)
            {
                throw new ArgumentNullException("operatorQueryFactories");
            }

            this.documentFieldName = documentFieldName;
            this.operandResolver = operandResolver;
            this.operatorQueryFactories = new List<IOperatorSpecificQueryFactory>(operatorQueryFactories);
        }

        public GenericClauseQueryFactory(SimpleFieldSearchConstants constants, IEnumerable<IOperatorSpecificQueryFactory> operatorQueryFactories, IJqlOperandResolver operandResolver)
            : this(constants.IndexField, operatorQueryFactories, operandResolver)
        {
        }

        public virtual QueryFactoryResult GetQuery(IQueryCreationContext queryCreationContext, ITerminalClause terminalClause)
        {
            IOperand operand = terminalClause.Operand;
            Operator @operator = terminalClause.Operator;

            if (operandResolver.IsValidOperand(operand))
            {
                // Run through all the registered operatorQueryFactories giving each one a chance to handle the current
                // Operator.
                foreach (IOperatorSpecificQueryFactory operatorQueryFactory in operatorQueryFactories)
                {
                    if (!operatorQueryFactory.HandlesOperator(@operator)) continue;

                    if (operandResolver.IsEmptyOperand(operand))
                    {
                        return operatorQueryFactory.CreateQueryForEmptyOperand(documentFieldName, @operator);
                    }
                        
                    // Turn the raw input values from the Operand into values that we can query in the index.
                    var rawValues = GetRawValues(queryCreationContext, terminalClause);

                    // We want to indicate to the OperatorQueryFactory whether these index values come from a
                    // single inputted value or a list of inputted values.
                    if (operandResolver.IsListOperand(operand))
                    {
                        return operatorQueryFactory.CreateQueryForMultipleValues(documentFieldName, @operator, rawValues);
                    }
                    return operatorQueryFactory.CreateQueryForSingleValue(documentFieldName, @operator, rawValues);
                }

                // If no one handled the operator then lets log it and return false
                //log.debug(string.Format("The '{0}' clause does not support the {1} operator.", terminalClause.Name, @operator.DisplayString));
                return QueryFactoryResult.CreateFalseResult();
            }
            
            // If there is no registered OperandHandler then lets log it and return false
            //log.debug(string.Format("There is no OperandHandler registered to handle the operand '{0}' for operand '{1}'.", @operator.DisplayString, terminalClause.Operand.DisplayString));
            return QueryFactoryResult.CreateFalseResult();
        }

        internal virtual List<QueryLiteral> GetRawValues(IQueryCreationContext queryCreationContext, ITerminalClause clause)
        {
            return operandResolver.GetValues(queryCreationContext, clause.Operand, clause).ToList();
        }
    }
}