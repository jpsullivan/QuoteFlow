using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QuoteFlow.Api.Infrastructure.Extensions;
using QuoteFlow.Api.Jql.Query.Operand;

namespace QuoteFlow.Api.Jql.Query.Clause
{
    /// <summary>
    /// Used to represent a terminal node in the query tree.
    /// </summary>
    public class TerminalClause : ITerminalClause
    {
        public IOperand Operand { get; set; }
        public Operator Operator { get; set; }
        public IEnumerable<Property> Property { get; set; }
        public string Name { get; set; }
        public IEnumerable<IClause> Clauses { get { return new List<IClause>(); } }

		/// <summary>
		/// Creates a terminal clause with the specified name, operator and turns the string value into a
		/// <see cref="SingleValueOperand"/> populated with a string value.
		/// </summary>
		/// <param name="name"> the name for the clause. </param>
        /// <param name="oprator"> the operator for the clause. </param>
		/// <param name="operand"> the string value that will be wrapped in a SingleValueOperand. </param>
		public TerminalClause(string name, Operator oprator, string operand) : this(name, oprator, new SingleValueOperand(operand), null)
		{
		}

		/// <summary>
		/// Creates a terminal clause with the specified name, operator and turns the long value into a
		/// <see cref="SingleValueOperand"/> populated with a long value.
		/// </summary>
		/// <param name="name"> the name for the clause. </param>
        /// <param name="oprator"> the operator for the clause. </param>
		/// <param name="operand"> the long value that will be wrapped in a SingleValueOperand. </param>
		public TerminalClause(string name, Operator oprator, int operand) : this(name, oprator, new SingleValueOperand(operand), null)
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
		public TerminalClause(string name, Operator oprator, IOperand operand, IEnumerable<Property> property)
		{
		    if (name == null) throw new ArgumentNullException(nameof(name));

		    Operator = oprator;
			Operand = operand;
			Name = name;
			Property = property;
		}

		/// <summary>
		/// A convienience constructor that will create a clause with the <see cref="Operator#EQUALS"/>
		/// operator if there is only one value in the array and with the <see cref="Operator#IN"/>
		/// operator if there are more than one value in the array.
		/// </summary>
		/// <param name="name"> the name for the clause. </param>
		/// <param name="values"> the string values that will be turned into <see cref="SingleValueOperand"/>'s
		/// containing a string value. </param>
        public TerminalClause(string name, params string[] values)
		{
			Name = name;
			if (values.Length == 1)
			{
				Operator = Operator.EQUALS;
				Operand = new SingleValueOperand(values[0]);
			}
			else
			{
				Operator = Operator.IN;
				Operand = new MultiValueOperand(values);
			}
		    Property = null;
		}

		/// <summary>
		/// A convienience constructor that will create a clause with the <see cref="Operator#EQUALS"/>
		/// operator if there is only one value in the array and with the <see cref="Operator#IN"/>
		/// operator if there are more than one value in the array.
		/// </summary>
		/// <param name="name">The name for the clause.</param>
		/// <param name="values">The long values that will be turned into <see cref="SingleValueOperand"/>'s
		/// containing a long value.</param>
        public TerminalClause(string name, params int?[] values)
		{
			Name = name;
			if (values.Length == 1)
			{
				Operator = Operator.EQUALS;
				Operand = new SingleValueOperand(values[0]);
			}
			else
			{
				Operator = Operator.IN;
				Operand = new MultiValueOperand(values);
			}
		    Property = null;
		}

		public virtual T Accept<T>(IClauseVisitor<T> visitor)
		{
			return visitor.Visit(this);
		}

        public override bool Equals(object obj)
        {
            if (this == obj)
            {
                return true;
            }
            if (obj == null || this.GetType() != obj.GetType())
            {
                return false;
            }

            TerminalClause that = (TerminalClause) obj;

            if (!Name.Equals(that.Name))
            {
                return false;
            }
            if (!Operand.Equals(that.Operand))
            {
                return false;
            }
            if (Operator != that.Operator)
            {
                return false;
            }
//            if (!Property.Equals(that.Property))
//            {
//                return false;
//            }

            return true;
        }

        public override int GetHashCode()
        {
            int result = Operand.GetHashCode();
            result = 31 * result + Operator.GetHashCode();
            result = 31 * result + Name.GetHashCode();
            //result = 31 * result + Property.GetHashCode();
            return result;
        }

        public override string ToString()
        {
            //The '{' brackets in this method are designed to make this method return invalid JQL so that we know when
            //we call this method. This method is only here for debugging and should not be used in production.
            var sb = new StringBuilder("{").Append(Name);

            if (Property != null)
            {
                foreach (var property in Property)
                {
                    sb.Append(property);
                }
            }

            sb.Append(" ");
            sb.Append(Operator.GetDisplayAttributeFrom(typeof(Operator)));
            sb.Append(" ");
            sb.Append(Operand.DisplayString).Append("}");
            return sb.ToString();
        }
    }
}