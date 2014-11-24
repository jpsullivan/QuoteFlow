using System;
using System.Collections.Generic;
using QuoteFlow.Models.Search.Jql.Operand;
using QuoteFlow.Models.Search.Jql.Query.Clause;
using QuoteFlow.Models.Search.Jql.Query.Operand.Registry;

namespace QuoteFlow.Models.Search.Jql.Query.Operand
{
    /// <summary>
    /// Has the standard handlers for dealing with history predicates.
    /// </summary>
    public class PredicateOperandHandlerRegistry
    {
        private readonly IJqlFunctionHandlerRegistry functionRegistry;

        public PredicateOperandHandlerRegistry(IJqlFunctionHandlerRegistry functionRegistry)
        {
            if (functionRegistry == null)
            {
                throw new ArgumentNullException("functionRegistry");
            }

            this.functionRegistry = functionRegistry;
        }

        public virtual IPredicateOperandHandler GetHandler(User searcher, string field, IOperand operand)
        {
            if (operand is SingleValueOperand)
            {
                return new SingleValuePredicateOperandHandler(searcher, (SingleValueOperand)operand);
            }
//            if (operand is EmptyOperand)
//            {
//                return new EmptyPredicateOperandHandler(changeHistoryFieldConfigurationManager, searcher, field, (EmptyOperand)operand);
//            }
            if (operand is MultiValueOperand)
            {
                return new MultiValuePredicateOperandHandler(searcher, this, field, (MultiValueOperand)operand);
            }
            if (operand is FunctionOperand)
            {
                return new FunctionPredicateOperandHandler(searcher, (FunctionOperand) operand, functionRegistry);
            }
            //log.debug(string.Format("Unknown operand type '{0}' with name '{1}'", operand.GetType(), operand.DisplayString));
            return null;
        }


        internal sealed class SingleValuePredicateOperandHandler : IPredicateOperandHandler
        {
            internal readonly SingleValueOperand singleValueOperand;
            internal readonly User searcher;

            internal SingleValuePredicateOperandHandler(User searcher, SingleValueOperand singleValueOperand)
            {
                this.singleValueOperand = singleValueOperand;
                this.searcher = searcher;
            }

            IEnumerable<QueryLiteral> IPredicateOperandHandler.Values
            {
                get
                {
                    if (singleValueOperand.IntValue == null)
                    {
                        return new List<QueryLiteral>()
                        {
                            new QueryLiteral(singleValueOperand, singleValueOperand.StringValue)
                        };
                    }

                    return new List<QueryLiteral>() {new QueryLiteral(singleValueOperand, singleValueOperand.IntValue)};
                }
            }

            bool IPredicateOperandHandler.Empty
            {
                get { return false; }
            }

            bool IPredicateOperandHandler.List
            {
                get { return false; }
            }

            bool IPredicateOperandHandler.Function
            {
                get { return false; }
            }
        }

//        internal sealed class EmptyPredicateOperandHandler : IPredicateOperandHandler
//        {
//
//            internal readonly EmptyOperand emptyOperand;
//            internal readonly User searcher;
//            internal readonly string field;
//            internal readonly ChangeHistoryFieldConfigurationManager changeHistoryFieldConfigurationManager;
//
//            internal EmptyPredicateOperandHandler(ChangeHistoryFieldConfigurationManager changeHistoryFieldConfigurationManager, User searcher, string field, EmptyOperand emptyOperand)
//            {
//                this.changeHistoryFieldConfigurationManager = changeHistoryFieldConfigurationManager;
//                this.searcher = searcher;
//                this.field = field;
//                this.emptyOperand = emptyOperand;
//            }
//
//            IEnumerable<QueryLiteral> IPredicateOperandHandler.Values
//            {
//                get
//                {
//                    var literals = new List<QueryLiteral>();
//                    if (emptyOperand != null)
//                    {
//                        literals.Add(new QueryLiteral(emptyOperand, GetStringValueForEmpty(field)));
//                    }
//                    return literals;
//                }
//            }
//
//            bool IPredicateOperandHandler.Empty
//            {
//                get { return true; }
//            }
//
//            bool IPredicateOperandHandler.List
//            {
//                get { return false; }
//            }
//
//            bool IPredicateOperandHandler.Function
//            {
//                get { return false; }
//            }
//
//            internal string GetStringValueForEmpty(string field)
//            {
//                return (field != null) ? changeHistoryFieldConfigurationManager.GetEmptyValue(field.ToLower()) : null;
//            }
//        }

        internal sealed class MultiValuePredicateOperandHandler : IPredicateOperandHandler
        {
            internal readonly PredicateOperandHandlerRegistry handlerRegistry;
            internal readonly MultiValueOperand operand;
            internal readonly string field;
            internal readonly User searcher;

            internal MultiValuePredicateOperandHandler(User searcher, PredicateOperandHandlerRegistry handlerRegistry, string field, MultiValueOperand operand)
            {
                this.searcher = searcher;
                this.handlerRegistry = handlerRegistry;
                this.field = field;
                this.operand = operand;
            }

            IEnumerable<QueryLiteral> IPredicateOperandHandler.Values
            {
                get
                {
                    var valuesList = new List<QueryLiteral>();
                    foreach (IOperand subOperand in operand.Values)
                    {
                        var vals = handlerRegistry.GetHandler(searcher, field, subOperand).Values;
                        if (vals != null)
                        {
                            valuesList.AddRange(vals);
                        }
                    }
                    return valuesList;
                }
            }

            bool IPredicateOperandHandler.Empty
            {
                get { return false; }
            }

            bool IPredicateOperandHandler.List
            {
                get { return true; }
            }

            bool IPredicateOperandHandler.Function
            {
                get { return false; }
            }
        }

        internal sealed class FunctionPredicateOperandHandler : IPredicateOperandHandler
        {
            internal readonly FunctionOperand operand;
            internal readonly User searcher;
            internal readonly IJqlFunctionHandlerRegistry functionRegistry;

            internal FunctionPredicateOperandHandler(User searcher, FunctionOperand operand, IJqlFunctionHandlerRegistry functionRegistry)
            {
                //this.searcher = ApplicationUsers.from(searcher);
                this.searcher = searcher;
                this.operand = operand;
                this.functionRegistry = functionRegistry;
            }

            IEnumerable<QueryLiteral> IPredicateOperandHandler.Values
            {
                get
                {
                    FunctionOperandHandler handler = functionRegistry.GetOperandHandler(operand);
                    return handler != null
                        ? handler.GetValues(new QueryCreationContext(searcher), operand,
                            new TerminalClause("PredicateOperandClause", Operator.EQUALS, operand))
                        : new List<QueryLiteral>();
                }
            }

            bool IPredicateOperandHandler.Empty
            {
                get { return false; }
            }

            bool IPredicateOperandHandler.List
            {
                get { return false; }
            }

            bool IPredicateOperandHandler.Function
            {
                get { return true; }
            }
        }
    }
}