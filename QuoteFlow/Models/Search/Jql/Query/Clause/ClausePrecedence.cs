using System;
using System.Collections.Generic;
using System.Linq;

namespace QuoteFlow.Models.Search.Jql.Query.Clause
{
    /// <summary>
    /// Used to determine the logical precedence of the clauses that can be contained in a SearchQuery.
    /// </summary>
    public sealed class ClausePrecedence
    {
        // Has the lowest logical precedence
        public static readonly ClausePrecedence OR = new ClausePrecedence("OR", InnerEnum.OR, 700);
        // Has the second highest logical precedence
        public static readonly ClausePrecedence AND = new ClausePrecedence("AND", InnerEnum.AND, 1000);
        // Has the highest logical precedence
        public static readonly ClausePrecedence NOT = new ClausePrecedence("NOT", InnerEnum.NOT, 2000);
        // This really has not precedence, but give it a large value to make things easuer.
        public static readonly ClausePrecedence TERMINAL = new ClausePrecedence("TERMINAL", InnerEnum.TERMINAL, Int32.MaxValue);

        private static readonly IList<ClausePrecedence> valueList = new List<ClausePrecedence>();

        static ClausePrecedence()
        {
            valueList.Add(OR);
            valueList.Add(AND);
            valueList.Add(NOT);
            valueList.Add(TERMINAL);
        }

        public enum InnerEnum
        {
            OR,
            AND,
            NOT,
            TERMINAL
        }

        private readonly string _nameValue;
        private readonly int _ordinalValue;
        private readonly InnerEnum _innerEnumValue;
        private static int _nextOrdinal = 0;

        private readonly int value;

        public static ClausePrecedence GetPrecedence(IClause clause)
        {
            if (clause is AndClause)
            {
                return AND;
            }
            else if (clause is OrClause)
            {
                return OR;
            }
            else if (clause is NotClause)
            {
                return NOT;
            }
            else if (clause is ITerminalClause || clause is IChangedClause)
            {
                return TERMINAL;
            }

            throw new ArgumentException("Attempt to get precedence for an unsupported clause.");
        }

        private ClausePrecedence(string name, InnerEnum innerEnum, int value)
        {
            this.value = value;

            _nameValue = name;
            _ordinalValue = _nextOrdinal++;
            _innerEnumValue = innerEnum;
        }

        public int Value { get { return value; } }

        public static IEnumerable<ClausePrecedence> Values()
        {
            return valueList;
        }

        public InnerEnum InnerEnumValue()
        {
            return _innerEnumValue;
        }

        public int Ordinal()
        {
            return _ordinalValue;
        }

        public override string ToString()
        {
            return _nameValue;
        }

        public static ClausePrecedence ValueOf(string name)
        {
            foreach (var enumInstance in Values().Where(enumInstance => enumInstance._nameValue == name))
            {
                return enumInstance;
            }
            throw new ArgumentException(name);
        }
    }

}