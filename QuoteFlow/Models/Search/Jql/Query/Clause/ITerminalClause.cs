﻿using System.Collections.Generic;
using QuoteFlow.Models.Search.Jql.Query.Operand;

namespace QuoteFlow.Models.Search.Jql.Query.Clause
{
    public interface ITerminalClause : IClause
    {
        /// <summary>
        /// Returns the right hand side value of the expression. This can be a composite of 
        /// more Operands or it can be SingleValueOperands that resolve to constant values.
        /// </summary>
        IOperand Operand { get; }

        /// <summary>
        /// Returns the operator used by the clause <seealso cref="Operator"/>.
        /// </summary>
        Operator Operator { get; }

        /// <summary>
        /// Returns the name of the property or absent.
        /// </summary>
        IEnumerable<Property> Property { get; }
    }
}