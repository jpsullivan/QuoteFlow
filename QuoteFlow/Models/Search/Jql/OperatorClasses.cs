using System.Collections.Generic;

namespace QuoteFlow.Models.Search.Jql
{
    /// <summary>
    /// Contains classes for operators.
    /// </summary>
    public static class OperatorClasses
    {
        /// <summary>
        /// A set of all non-relational operators.
        /// </summary>
        public static List<Query.Operator> NonRelationalOperators = new List<Query.Operator>()
        {
            Query.Operator.NOT_EQUALS,
            Query.Operator.EQUALS,
            Query.Operator.NOT_LIKE,
            Query.Operator.LIKE,
            Query.Operator.NOT_IN,
            Query.Operator.IN,
            Query.Operator.IS_NOT,
            Query.Operator.IS
        };

        /// <summary>
        /// A list of operators that work exclusively on the EMPTY clause.
        /// </summary>
        public static List<Query.Operator> EmptyOnlyOperators = new List<Query.Operator>()
        {
            Query.Operator.IS_NOT,
            Query.Operator.IS
        }; 

        /// <summary>
        /// A list of operators that can work on the EMPTY clause.
        /// </summary>
        public static List<Query.Operator> EmptyOperators = new List<Query.Operator>()
        {
            Query.Operator.NOT_EQUALS,
            Query.Operator.EQUALS,
            Query.Operator.NOT_LIKE,
            Query.Operator.LIKE,
            Query.Operator.IS_NOT,
            Query.Operator.IS,
            Query.Operator.WAS_NOT,
            Query.Operator.WAS
        }; 

        /// <summary>
        /// A list of operators that can work on text clauses.
        /// </summary>
        public static List<Query.Operator> TextOperators = new List<Query.Operator>()
        {
            Query.Operator.NOT_LIKE,
            Query.Operator.LIKE,
            Query.Operator.IS_NOT,
            Query.Operator.IS
        };

        /// <summary>
        /// 
        /// </summary>
        public static List<Query.Operator> PositiveEqualityOperators = new List<Query.Operator>()
        {
            Query.Operator.EQUALS,
            Query.Operator.IN,
            Query.Operator.IS
        }; 

        /// <summary>
        /// 
        /// </summary>
        public static List<Query.Operator>  NegativeEqualityOperators = new List<Query.Operator>()
        {
            Query.Operator.NOT_EQUALS,
            Query.Operator.NOT_IN,
            Query.Operator.IS_NOT
        };

        /// <summary>
        /// A list of operators that a clause needs to support if it supports the EQUALS operator
        /// and the EMPTY operand.
        /// </summary>
        public static List<Query.Operator> EqualityOperatorsWithEmpty = new List<Query.Operator>()
        {
            Query.Operator.EQUALS,
            Query.Operator.NOT_EQUALS,
            Query.Operator.NOT_IN,
            Query.Operator.IN,
            Query.Operator.IS_NOT,
            Query.Operator.IS
        };

        /// <summary>
        /// The list of operators that a clause needs to support if it supports the EQUALS operator.
        /// </summary>
        public static List<Query.Operator> EqualityOperators = new List<Query.Operator>()
        {
            Query.Operator.EQUALS,
            Query.Operator.NOT_EQUALS,
            Query.Operator.NOT_IN,
            Query.Operator.IN
        };

        /// <summary>
        /// A set of operators that work with lists.
        /// </summary>
        public static List<Query.Operator> ListOnlyOperators = new List<Query.Operator>()
        {
            Query.Operator.NOT_IN,
            Query.Operator.IN,
            Query.Operator.WAS_NOT_IN,
            Query.Operator.WAS_IN
        };

        /// <summary>
        /// 
        /// </summary>
        public static List<Query.Operator> RelationalOnlyOperators = new List<Query.Operator>()
        {
            Query.Operator.GREATER_THAN,
            Query.Operator.GREATER_THAN_EQUALS,
            Query.Operator.LESS_THAN,
            Query.Operator.LESS_THAN_EQUALS
        };

        /// <summary>
        /// A set of equality operators and relational only operators
        /// </summary>
        public static List<Query.Operator> EqualityAndRelational = new List<Query.Operator>()
        {
            Query.Operator.EQUALS,
            Query.Operator.NOT_EQUALS,
            Query.Operator.NOT_IN,
            Query.Operator.IN,
            Query.Operator.GREATER_THAN,
            Query.Operator.GREATER_THAN_EQUALS,
            Query.Operator.LESS_THAN,
            Query.Operator.LESS_THAN_EQUALS
        }; 

        /// <summary>
        /// A list of change history predicates.
        /// </summary>
        public static List<Query.Operator> ChangeHistoryPredicates = new List<Query.Operator>()
        {
            Query.Operator.AFTER,
            Query.Operator.BEFORE,
            Query.Operator.BY,
            Query.Operator.DURING,
            Query.Operator.ON,
            Query.Operator.FROM,
            Query.Operator.TO
        }; 

        /// <summary>
        /// A list of change history predicates that support date searching.
        /// </summary>
        public static List<Query.Operator> ChangeHistoryValuePredicates = new List<Query.Operator>()
        {
            Query.Operator.FROM,
            Query.Operator.TO
        }; 

        /// <summary>
        /// A list of change history operators.
        /// </summary>
        public static List<Query.Operator> ChangeHistoryOperators = new List<Query.Operator>()
        {
            Query.Operator.WAS,
            Query.Operator.WAS_NOT,
            Query.Operator.WAS_IN,
            Query.Operator.WAS_NOT_IN,
            Query.Operator.CHANGED
        };

        /// <summary>
        /// A set of equality operators with empty and relational only operators.
        /// </summary>
        public static List<Query.Operator> EqualityAndRelationalWithEmpty = new List<Query.Operator>()
        {
            Query.Operator.EQUALS,
            Query.Operator.NOT_EQUALS,
            Query.Operator.NOT_IN,
            Query.Operator.IN,
            Query.Operator.IS_NOT,
            Query.Operator.IS,
            Query.Operator.GREATER_THAN,
            Query.Operator.GREATER_THAN_EQUALS,
            Query.Operator.LESS_THAN,
            Query.Operator.LESS_THAN_EQUALS
        };
    }
}