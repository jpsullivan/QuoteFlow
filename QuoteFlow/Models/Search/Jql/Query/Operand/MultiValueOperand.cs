using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using QuoteFlow.Models.Search.Jql.Operand;

namespace QuoteFlow.Models.Search.Jql.Query.Operand
{
    public sealed class MultiValueOperand : IOperand
    {
        public const string OPERAND_NAME = "MultiValueOperand";

        private IList<IOperand> Values { get; set; } 

        private const string LEFT_PAREN = "(";
		private const string COMMA_SPACE = ", ";
		private const string RIGHT_PAREN = ")";
		private readonly int hashcode;

		public static MultiValueOperand ofQueryLiterals(ICollection<QueryLiteral> literals)
		{
			return new MultiValueOperand(literals.toArray(new QueryLiteral[literals.Count]));
		}

		public MultiValueOperand(params string[] stringValues)
		{
			notNull("stringValues", stringValues);
			not("stringValues is empty", stringValues.Length == 0);
			List<Operand> tmpValues = new List<Operand>(stringValues.Length);
			foreach (string stringValue in stringValues)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final SingleValueOperand singleValueOperand = new SingleValueOperand(stringValue);
				SingleValueOperand singleValueOperand = new SingleValueOperand(stringValue);
				tmpValues.Add(singleValueOperand);
			}
			values = Collections.unmodifiableList(tmpValues);
			this.hashcode = calculateHashCode(this.values);
		}

		public MultiValueOperand(IList<long?> longs)
		{
			notNull("longs", longs);
			not("longs not empty", longs.Count == 0);
			values = Collections.unmodifiableList(getLongOperands(longs));
			this.hashcode = calculateHashCode(this.values);
		}

		public MultiValueOperand(params long?[] longs)
		{
			notNull("longs", longs);
			not("longs not empty", longs.Length == 0);
			values = Collections.unmodifiableList(getLongOperands(Arrays.asList(longs)));
			this.hashcode = calculateHashCode(this.values);
		}

		public MultiValueOperand(params IOperand[] operands) : this(Arrays.asList(notNull("operands", operands)))
		{
		}

		public MultiValueOperand(params QueryLiteral[] literals)
		{
			notNull("literals", literals);
			not("literals not empty", literals.Length == 0);
			List<Operand> tmpValues = new List<Operand>(literals.Length);
			foreach (QueryLiteral literal in literals)
			{
				if (literal.Empty)
				{
					tmpValues.Add(EmptyOperand.EMPTY);
				}
				else
				{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final SingleValueOperand singleValueOperand = new SingleValueOperand(literal);
					SingleValueOperand singleValueOperand = new SingleValueOperand(literal);
					tmpValues.Add(singleValueOperand);
				}
			}

			values = Collections.unmodifiableList(tmpValues);
			this.hashcode = calculateHashCode(this.values);
		}

		public MultiValueOperand<T1>(ICollection<T1> values) where T1 : IOperand
		{
			containsNoNulls("values", values);
			not("values is empty", values.Count == 0);
			this.values = CollectionUtil.copyAsImmutableList(values);
			this.hashcode = calculateHashCode(this.values);
		}
    }
}