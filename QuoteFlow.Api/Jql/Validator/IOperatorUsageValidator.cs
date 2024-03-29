﻿using QuoteFlow.Api.Jql.Query.Clause;
using QuoteFlow.Api.Models;
using QuoteFlow.Api.Util;

namespace QuoteFlow.Api.Jql.Validator
{
    /// <summary>
    /// Performs global validation about where it is appropriate to use an <see cref="Operator"/>. 
    /// This does checks to see that list operators are not used with non-lists and vice-versa.
    /// </summary>
    public interface IOperatorUsageValidator
    {
        /// <summary>
        /// Validate the usage of the Operator and Operand that are held in the clause. The clause specific validation,
        /// as to whether the clause has any specific issues with the configuration occurs elsewhere. This is just
        /// performing global Operator/Operand checks.
        /// </summary>
        /// <param name="searcher">The user performing the validation.</param>
        /// <param name="clause">The clause that contains the Operator and Operand.</param>
        /// <returns>A MessageSet that will contain any errors or warnings that may have been generated from the validation.</returns>
        IMessageSet Validate(User searcher, ITerminalClause clause);

        /// <summary>
        /// Check the usage of the Operator and Operand that are held in the clause. The clause specific validation,
        /// as to whether the clause has any specific issues with the configuration occurs elsewhere. This is just
        /// performing global Operator/Operand checks.
        /// </summary>
        /// <param name="searcher">The user performing the validation.</param>
        /// <param name="clause">The clause that contains the Operator and Operand.</param>
        /// <returns>True if the passed clause is valid, false otherwise.</returns>
        bool Check(User searcher, ITerminalClause clause);

    }
}