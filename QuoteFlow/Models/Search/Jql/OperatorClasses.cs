using Wintellect.PowerCollections;

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
        public static Set<Query.Operator> NonRelationalOperators = new Set<Query.Operator>()
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
        public static Set<Query.Operator> EmptyOnlyOperators = new Set<Query.Operator>()
        {
            Query.Operator.IS_NOT,
            Query.Operator.IS
        }; 

        /// <summary>
        /// A list of operators that can work on the EMPTY clause.
        /// </summary>
        public static Set<Query.Operator> EmptyOperators = new Set<Query.Operator>()
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
        public static Set<Query.Operator> TextOperators = new Set<Query.Operator>()
        {
            Query.Operator.NOT_LIKE,
            Query.Operator.LIKE,
            Query.Operator.IS_NOT,
            Query.Operator.IS
        };

        /// <summary>
        /// 
        /// </summary>
        public static Set<Query.Operator> PositiveEqualityOperators = new Set<Query.Operator>()
        {
            Query.Operator.EQUALS,
            Query.Operator.IN,
            Query.Operator.IS
        }; 

        /// <summary>
        /// 
        /// </summary>
        public static Set<Query.Operator> NegativeEqualityOperators = new Set<Query.Operator>()
        {
            Query.Operator.NOT_EQUALS,
            Query.Operator.NOT_IN,
            Query.Operator.IS_NOT
        };

        /// <summary>
        /// A list of operators that a clause needs to support if it supports the EQUALS operator
        /// and the EMPTY operand.
        /// </summary>
        public static Set<Query.Operator> EqualityOperatorsWithEmpty = new Set<Query.Operator>()
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
        public static Set<Query.Operator> EqualityOperators = new Set<Query.Operator>()
        {
            Query.Operator.EQUALS,
            Query.Operator.NOT_EQUALS,
            Query.Operator.NOT_IN,
            Query.Operator.IN
        };

        /// <summary>
        /// A set of operators that work with lists.
        /// </summary>
        public static Set<Query.Operator> ListOnlyOperators = new Set<Query.Operator>
        {
            Query.Operator.NOT_IN,
            Query.Operator.IN,
            Query.Operator.WAS_NOT_IN,
            Query.Operator.WAS_IN
        };

        /// <summary>
        /// 
        /// </summary>
        public static Set<Query.Operator> RelationalOnlyOperators = new Set<Query.Operator>
        {
            Query.Operator.GREATER_THAN,
            Query.Operator.GREATER_THAN_EQUALS,
            Query.Operator.LESS_THAN,
            Query.Operator.LESS_THAN_EQUALS
        };

        /// <summary>
        /// A set of equality operators and relational only operators
        /// </summary>
        public static Set<Query.Operator> EqualityAndRelational = new Set<Query.Operator>
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
        public static Set<Query.Operator> ChangeHistoryPredicates = new Set<Query.Operator>
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
        public static Set<Query.Operator> ChangeHistoryValuePredicates = new Set<Query.Operator>()
        {
            Query.Operator.FROM,
            Query.Operator.TO
        }; 

        /// <summary>
        /// A list of change history operators.
        /// </summary>
        public static Set<Query.Operator> ChangeHistoryOperators = new Set<Query.Operator>()
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
        public static Set<Query.Operator> EqualityAndRelationalWithEmpty = new Set<Query.Operator>()
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