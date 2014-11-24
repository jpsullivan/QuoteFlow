﻿using System;
using System.Collections.Generic;
using QuoteFlow.Models.Search.Jql.Operand;

namespace QuoteFlow.Models.Search.Jql.Query.Operand
{
    /// <summary>
    /// Default implementation of the <see cref="IPredicateOperandResolver"/>
    /// </summary>
    public class PredicateOperandResolver : IPredicateOperandResolver
    {
        private readonly PredicateOperandHandlerRegistry predicateOperandHandlerRegistry;

        public PredicateOperandResolver(PredicateOperandHandlerRegistry handlerRegistry)
		{
			  predicateOperandHandlerRegistry = handlerRegistry;
		}

        public IEnumerable<QueryLiteral> GetValues(User searcher, string field, IOperand operand)
        {
            if (operand == null)
            {
                throw new ArgumentNullException("operand");
            }

            return predicateOperandHandlerRegistry.GetHandler(searcher, field, operand).Values;
        }

        public bool IsEmptyOperand(User searcher, string field, IOperand operand)
        {
            return predicateOperandHandlerRegistry.GetHandler(searcher, field, operand).Empty;

        }

        public bool IsFunctionOperand(User searcher, string field, IOperand operand)
        {
            return predicateOperandHandlerRegistry.GetHandler(searcher, field, operand).Function;
        }

        public bool IsListOperand(User searcher, string field, IOperand operand)
        {
            return predicateOperandHandlerRegistry.GetHandler(searcher, field, operand).List;
        }
    }
}