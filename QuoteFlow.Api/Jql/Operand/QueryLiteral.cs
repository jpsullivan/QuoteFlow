using System;
using QuoteFlow.Api.Jql.Query.Operand;

namespace QuoteFlow.Api.Jql.Operand
{
    /// <summary>
    /// Used to communicate literal values, Strings or Longs, as input into the Operands.
    /// 
    /// Long values are typically used to represent ids or raw numerical values. For example, 
    /// issue ids are represented using a Long.
    /// String values are typically used to represent raw string values or named values that 
    /// need to be resolved into ids. For example, issue keys or project names are represented 
    /// using a String.
    /// 
    /// When writing <seealso cref="JqlFunction"/>s that must return QueryLiterals, try to
    /// return the more specific QueryLiteral where possible, to avoid unnecessary resolving. 
    /// "More specific" here means the form that is used by the index (if applicable), as this 
    /// value can then be used directly when constructing index queries.
    /// 
    /// QueryLiterals contain an operand source, this is the <seealso cref="IOperand"/> of the JQL that
    /// produced the QueryLiteral. For instance in the JQL query {@code project = HSP} the "HSP" 
    /// QueryLiteral will have the operand source of a <seealso cref="SingleValueOperand"/> with value "HSP". 
    /// Notably QueryLiterals produced by <seealso cref="JqlFunction"/>s must set the <seealso cref="FunctionOperand"/> 
    /// as the operand source.
    /// </summary>
    public class QueryLiteral
    {
        public QueryLiteral()
        {
            SourceOperand = EmptyOperand.EMPTY;
            StringValue = null;
            IntValue = null;
        }

        public QueryLiteral(IOperand sourceOperand)
        {
            SourceOperand = sourceOperand;
            StringValue = null;
            IntValue = null;
        }

        public QueryLiteral(IOperand sourceOperand, int? intValue)
        {
            SourceOperand = sourceOperand;
            StringValue = null;
            IntValue = intValue;
        }

        public QueryLiteral(IOperand sourceOperand, string stringValue)
        {
            SourceOperand = sourceOperand;
            StringValue = stringValue;
            IntValue = null;
        }

        /// <summary>
        /// 
        /// </summary>
        public string StringValue { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int? IntValue { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public IOperand SourceOperand { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool IsEmpty
        {
            get { return StringValue == null && IntValue == null; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string AsString()
        {
            return IntValue != null ? IntValue.ToString() : StringValue;
        }

        public override string ToString()
        {
            return (IntValue != null) ? IntValue.ToString() : Convert.ToString(StringValue);
        }
    }
}