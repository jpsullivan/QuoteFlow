using QuoteFlow.Models.Search.Jql.Query;
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
        public static Set<Operator> NonRelationalOperators = new Set<Operator>()
        {
            Operator.NOT_EQUALS,
            Operator.EQUALS,
            Operator.NOT_LIKE,
            Operator.LIKE,
            Operator.NOT_IN,
            Operator.IN,
            Operator.IS_NOT,
            Operator.IS
        };

        /// <summary>
        /// A list of operators that work exclusively on the EMPTY clause.
        /// </summary>
        public static Set<Operator> EmptyOnlyOperators = new Set<Operator>()
        {
            Operator.IS_NOT,
            Operator.IS
        }; 

        /// <summary>
        /// A list of operators that can work on the EMPTY clause.
        /// </summary>
        public static Set<Operator> EmptyOperators = new Set<Operator>()
        {
            Operator.NOT_EQUALS,
            Operator.EQUALS,
            Operator.NOT_LIKE,
            Operator.LIKE,
            Operator.IS_NOT,
            Operator.IS,
            Operator.WAS_NOT,
            Operator.WAS
        }; 

        /// <summary>
        /// A list of operators that can work on text clauses.
        /// </summary>
        public static Set<Operator> TextOperators = new Set<Operator>()
        {
            Operator.NOT_LIKE,
            Operator.LIKE,
            Operator.IS_NOT,
            Operator.IS
        };

        /// <summary>
        /// 
        /// </summary>
        public static Set<Operator> PositiveEqualityOperators = new Set<Operator>()
        {
            Operator.EQUALS,
            Operator.IN,
            Operator.IS
        }; 

        /// <summary>
        /// 
        /// </summary>
        public static Set<Operator> NegativeEqualityOperators = new Set<Operator>()
        {
            Operator.NOT_EQUALS,
            Operator.NOT_IN,
            Operator.IS_NOT
        };

        /// <summary>
        /// A list of operators that a clause needs to support if it supports the EQUALS operator
        /// and the EMPTY operand.
        /// </summary>
        public static Set<Operator> EqualityOperatorsWithEmpty = new Set<Operator>()
        {
            Operator.EQUALS,
            Operator.NOT_EQUALS,
            Operator.NOT_IN,
            Operator.IN,
            Operator.IS_NOT,
            Operator.IS
        };

        /// <summary>
        /// The list of operators that a clause needs to support if it supports the EQUALS operator.
        /// </summary>
        public static Set<Operator> EqualityOperators = new Set<Operator>()
        {
            Operator.EQUALS,
            Operator.NOT_EQUALS,
            Operator.NOT_IN,
            Operator.IN
        };

        /// <summary>
        /// A set of operators that work with lists.
        /// </summary>
        public static Set<Operator> ListOnlyOperators = new Set<Operator>
        {
            Operator.NOT_IN,
            Operator.IN,
            Operator.WAS_NOT_IN,
            Operator.WAS_IN
        };

        /// <summary>
        /// 
        /// </summary>
        public static Set<Operator> RelationalOnlyOperators = new Set<Operator>
        {
            Operator.GREATER_THAN,
            Operator.GREATER_THAN_EQUALS,
            Operator.LESS_THAN,
            Operator.LESS_THAN_EQUALS
        };

        /// <summary>
        /// A set of equality operators and relational only operators
        /// </summary>
        public static Set<Operator> EqualityAndRelational = new Set<Operator>
        {
            Operator.EQUALS,
            Operator.NOT_EQUALS,
            Operator.NOT_IN,
            Operator.IN,
            Operator.GREATER_THAN,
            Operator.GREATER_THAN_EQUALS,
            Operator.LESS_THAN,
            Operator.LESS_THAN_EQUALS
        }; 

        /// <summary>
        /// A list of change history predicates.
        /// </summary>
        public static Set<Operator> ChangeHistoryPredicates = new Set<Operator>
        {
            Operator.AFTER,
            Operator.BEFORE,
            Operator.BY,
            Operator.DURING,
            Operator.ON,
            Operator.FROM,
            Operator.TO
        }; 

        /// <summary>
        /// A list of change history predicates that support date searching.
        /// </summary>
        public static Set<Operator> ChangeHistoryValuePredicates = new Set<Operator>()
        {
            Operator.FROM,
            Operator.TO
        }; 

        /// <summary>
        /// A set of change history predicates that support date searching.
        /// </summary>
        public static Set<Operator> ChangeHistoryDatePredicates = new Set<Operator>()
        {
            Operator.AFTER,
            Operator.BEFORE,
            Operator.DURING,
            Operator.ON
        }; 

        /// <summary>
        /// A list of change history operators.
        /// </summary>
        public static Set<Operator> ChangeHistoryOperators = new Set<Operator>()
        {
            Operator.WAS,
            Operator.WAS_NOT,
            Operator.WAS_IN,
            Operator.WAS_NOT_IN,
            Operator.CHANGED
        };

        /// <summary>
        /// A set of equality operators with empty and relational only operators.
        /// </summary>
        public static Set<Operator> EqualityAndRelationalWithEmpty = new Set<Operator>()
        {
            Operator.EQUALS,
            Operator.NOT_EQUALS,
            Operator.NOT_IN,
            Operator.IN,
            Operator.IS_NOT,
            Operator.IS,
            Operator.GREATER_THAN,
            Operator.GREATER_THAN_EQUALS,
            Operator.LESS_THAN,
            Operator.LESS_THAN_EQUALS
        };
    }
}