using System.Collections.Generic;

namespace QuoteFlow.Models.Search.Jql.Query
{
    public sealed class Operator
    {
        public static readonly Operator LIKE = new Operator("LIKE", OperatorEnum.LIKE, "~");
        public static readonly Operator NOT_LIKE = new Operator("NOT_LIKE", OperatorEnum.NOT_LIKE, "!~");
        public static readonly Operator EQUALS = new Operator("EQUALS", OperatorEnum.EQUALS, "=");
        public static readonly Operator NOT_EQUALS = new Operator("NOT_EQUALS", OperatorEnum.NOT_EQUALS, "!=");
        public static readonly Operator IN = new Operator("IN", OperatorEnum.IN, "in");
        public static readonly Operator NOT_IN = new Operator("NOT_IN", OperatorEnum.NOT_IN, "not in");
        public static readonly Operator IS = new Operator("IS", OperatorEnum.IS, "is");
        public static readonly Operator IS_NOT = new Operator("IS_NOT", OperatorEnum.IS_NOT, "is not");
        public static readonly Operator LESS_THAN = new Operator("LESS_THAN", OperatorEnum.LESS_THAN, "<");
        public static readonly Operator LESS_THAN_EQUALS = new Operator("LESS_THAN_EQUALS", OperatorEnum.LESS_THAN_EQUALS, "<=");
        public static readonly Operator GREATER_THAN = new Operator("GREATER_THAN", OperatorEnum.GREATER_THAN, ">");
        public static readonly Operator GREATER_THAN_EQUALS = new Operator("GREATER_THAN_EQUALS", OperatorEnum.GREATER_THAN_EQUALS, ">=");
        public static readonly Operator WAS = new Operator("WAS", OperatorEnum.WAS, "was");
        public static readonly Operator WAS_NOT = new Operator("WAS_NOT", OperatorEnum.WAS_NOT, "was not");
        public static readonly Operator WAS_IN = new Operator("WAS_IN", OperatorEnum.WAS_IN, "was in");
        public static readonly Operator WAS_NOT_IN = new Operator("WAS_NOT_IN", OperatorEnum.WAS_NOT_IN, "was not in");
        public static readonly Operator CHANGED = new Operator("CHANGED", OperatorEnum.CHANGED, "changed");
        public static readonly Operator NOT_CHANGED = new Operator("NOT_CHANGED", OperatorEnum.NOT_CHANGED, "not changed");
        public static readonly Operator BEFORE = new Operator("BEFORE", OperatorEnum.BEFORE, "before");
        public static readonly Operator AFTER = new Operator("AFTER", OperatorEnum.AFTER, "after");
        public static readonly Operator FROM = new Operator("FROM", OperatorEnum.FROM, "from");
        public static readonly Operator TO = new Operator("TO", OperatorEnum.TO, "to");
        public static readonly Operator ON = new Operator("ON", OperatorEnum.ON, "on");
        public static readonly Operator DURING = new Operator("DURING", OperatorEnum.DURING, "during");
        public static readonly Operator BY = new Operator("BY", OperatorEnum.BY, "by");

        private static readonly IList<Operator> valueList = new List<Operator>();
        static Operator()
        {
            valueList.Add(LIKE);
            valueList.Add(NOT_LIKE);
            valueList.Add(EQUALS);
            valueList.Add(NOT_EQUALS);
            valueList.Add(IN);
            valueList.Add(NOT_IN);
            valueList.Add(IS);
            valueList.Add(IS_NOT);
            valueList.Add(LESS_THAN);
            valueList.Add(LESS_THAN_EQUALS);
            valueList.Add(GREATER_THAN);
            valueList.Add(GREATER_THAN_EQUALS);
            valueList.Add(WAS);
            valueList.Add(WAS_NOT);
            valueList.Add(WAS_IN);
            valueList.Add(WAS_NOT_IN);
            valueList.Add(CHANGED);
            valueList.Add(NOT_CHANGED);
            valueList.Add(BEFORE);
            valueList.Add(AFTER);
            valueList.Add(FROM);
            valueList.Add(TO);
            valueList.Add(ON);
            valueList.Add(DURING);
            valueList.Add(BY);
        }

        private readonly string _nameValue;
        private readonly int _ordinalValue;
        private readonly OperatorEnum _operatorEnumValue;
        private static int _nextOrdinal;

        private readonly string _value;

        private Operator(string name, OperatorEnum operatorEnum, string value)
        {
            _value = value;
            _nameValue = name;
            _ordinalValue = _nextOrdinal++;
            _operatorEnumValue = operatorEnum;
        }

        public string Value { get { return _value; } }

        public static IEnumerable<Operator> Values()
        {
            return valueList;
        }

        public override string ToString()
        {
            return _nameValue;
        }
    }

    public enum OperatorEnum
    {
        LIKE,
        NOT_LIKE,
        EQUALS,
        NOT_EQUALS,
        IN,
        NOT_IN,
        IS,
        IS_NOT,
        LESS_THAN,
        LESS_THAN_EQUALS,
        GREATER_THAN,
        GREATER_THAN_EQUALS,
        WAS,
        WAS_NOT,
        WAS_IN,
        WAS_NOT_IN,
        CHANGED,
        NOT_CHANGED,
        BEFORE,
        AFTER,
        FROM,
        TO,
        ON,
        DURING,
        BY
    }
}