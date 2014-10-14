using System.Collections.Generic;
using QuoteFlow.Models.Search.Jql.Query.Operand;

namespace QuoteFlow.Models.Search.Jql.Query.Clause
{
    /// <summary>
    /// Used to represent a terminal node in the query tree.
    /// </summary>
    public class TerminalClause : ITerminalClause
    {
        private readonly string _name;
		private readonly Operator _operator;
		private readonly IOperand _operand;
		private readonly Property _property;

		/// <summary>
		/// Creates a terminal clause with the specified name, operator and turns the string value into a
		/// <seealso cref="SingleValueOperand"/> populated with a string value.
		/// </summary>
		/// <param name="name"> the name for the clause. </param>
        /// <param name="oprator"> the operator for the clause. </param>
		/// <param name="operand"> the string value that will be wrapped in a SingleValueOperand. </param>
		public TerminalClause(string name, Operator oprator, string operand) : this(name, oprator, new SingleValueOperand(operand), null)
		{
		}

		/// <summary>
		/// Creates a terminal clause with the specified name, operator and turns the long value into a
		/// <seealso cref="com.atlassian.query.operand.SingleValueOperand"/> populated with a long value.
		/// </summary>
		/// <param name="name"> the name for the clause. </param>
        /// <param name="oprator"> the operator for the clause. </param>
		/// <param name="operand"> the long value that will be wrapped in a SingleValueOperand. </param>
		public TerminalClause(string name, Operator oprator, long operand) : this(name, oprator, new SingleValueOperand(operand), null)
		{
		}

		/// <summary>
		/// Creates a terminal clause with the specified name, operator and operand.
		/// </summary>
		/// <param name="name"> the name for the clause. </param>
        /// <param name="oprator"> the operator for the clause. </param>
		/// <param name="operand"> the right-hand-side value of the clause. </param>
		public TerminalClause(string name, Operator oprator, IOperand operand) : this(name, oprator, operand, null)
		{
		}

		/// <summary>
		/// Creates a terminal clause with the specified name, operator, operand and property.
		/// </summary>
		/// <param name="name"> the name for the clause. </param>
        /// <param name="oprator"> the operator for the clause. </param>
		/// <param name="operand"> the right-hand-side value of the clause. </param>
		/// <param name="property"> the name of the property. </param>
		public TerminalClause(string name, Operator oprator, IOperand operand, Property property)
		{
		    _operator = oprator;
			_operand = operand;
			_name = name;
			_property = property;
		}

		/// <summary>
		/// A convienience constructor that will create a clause with the <seealso cref="Operator#EQUALS"/>
		/// operator if there is only one value in the array and with the <seealso cref="Operator#IN"/>
		/// operator if there are more than one value in the array.
		/// </summary>
		/// <param name="name"> the name for the clause. </param>
		/// <param name="values"> the string values that will be turned into <seealso cref="SingleValueOperand"/>'s
		/// containing a string value. </param>
        public TerminalClause(string name, params string[] values)
		{
			_name = name;
			if (values.Length == 1)
			{
				_operator = Operator.EQUALS;
				_operand = new SingleValueOperand(values[0]);
			}
			else
			{
				_operator = Operator.IN;
				_operand = new MultiValueOperand(values);
			}
		    _property = null;
		}

		/// <summary>
		/// A convienience constructor that will create a clause with the <seealso cref="Operator#EQUALS"/>
		/// operator if there is only one value in the array and with the <seealso cref="Operator#IN"/>
		/// operator if there are more than one value in the array.
		/// </summary>
		/// <param name="name">The name for the clause.</param>
		/// <param name="values">The long values that will be turned into <seealso cref="SingleValueOperand"/>'s
		/// containing a long value.</param>
        public TerminalClause(string name, params long?[] values)
		{
			_name = name;
			if (values.Length == 1)
			{
				_operator = Operator.EQUALS;
				_operand = new SingleValueOperand(values[0]);
			}
			else
			{
				_operator = Operator.IN;
				_operand = new MultiValueOperand(values);
			}
		    _property = null;
		}

		public virtual IOperand Operand { get { return _operand; } }

		public virtual Operator Operator { get { return _operator; } }

		public virtual Property Property { get { return _property; } }

		public virtual string Name { get { return _name; } }

		public virtual IEnumerable<IClause> Clauses { get { return new List<IClause>(); } }

		public virtual T Accept<T>(IClauseVisitor<T> visitor)
		{
			return visitor.Visit(this);
		}

		public override bool Equals(object o)
		{
			if (this == o)
			{
				return true;
			}
			if (o == null || GetType() != o.GetType())
			{
				return false;
			}

			var that = (TerminalClause) o;

			if (!_name.Equals(that.Name))
			{
				return false;
			}
			if (!_operand.Equals(that.Operand))
			{
				return false;
			}
			if (_operator != that._operator)
			{
				return false;
			}
			
			if (!_property.Equals(that.Property))
			{
				return false;
			}

			return true;
		}

		public override int GetHashCode()
		{
			int result = _operand.GetHashCode();
			result = 31 * result + _operator.GetHashCode();
			result = 31 * result + _name.GetHashCode();
			result = 31 * result + _property.GetHashCode();
			return result;
		}
    }
}