using System.Collections.Generic;
using System.Linq;
using System.Text;
using QuoteFlow.Models.Search.Jql.Operand;

namespace QuoteFlow.Models.Search.Jql.Query.Operand
{
    public sealed class MultiValueOperand : IOperand
    {
        public const string OPERAND_NAME = "MultiValueOperand";

        public IEnumerable<IOperand> Values { get; private set; } 
        public int HashCode { get; set; }

        private const string LEFT_PAREN = "(";
		private const string COMMA_SPACE = ", ";
		private const string RIGHT_PAREN = ")";
		private readonly int hashcode;

		public static MultiValueOperand OfQueryLiterals(IEnumerable<QueryLiteral> literals)
		{
			return new MultiValueOperand(literals.ToArray());
		}

		public MultiValueOperand(params string[] stringValues)
		{
			var tmpValues = new List<IOperand>(stringValues.Length);
		    tmpValues.AddRange(stringValues.Select(stringValue => new SingleValueOperand(stringValue)));
		    Values = new List<IOperand>(tmpValues);
			HashCode = CalculateHashCode(Values);
		}

		public MultiValueOperand(ICollection<long?> longs)
		{
			Values = new List<IOperand>(GetLongOperands(longs));
			HashCode = CalculateHashCode(Values);
		}

		public MultiValueOperand(params long?[] longs)
		{
			Values = new List<IOperand>(GetLongOperands(longs.ToList()));
			HashCode = CalculateHashCode(Values);
		}

		public MultiValueOperand(params QueryLiteral[] literals)
		{
			var tmpValues = new List<IOperand>(literals.Length);
			foreach (QueryLiteral literal in literals)
			{
				if (literal.IsEmpty)
				{
					tmpValues.Add(EmptyOperand.EMPTY);
				}
				else
				{
					var singleValueOperand = new SingleValueOperand(literal);
					tmpValues.Add(singleValueOperand);
				}
			}

		    Values = new List<IOperand>(tmpValues);
			HashCode = CalculateHashCode(Values);
		}

//		public MultiValueOperand<T>(ICollection<T> values) where T : IOperand
//		{
//			this.values = CollectionUtil.copyAsImmutableList(values);
//			this.hashcode = calculateHashCode(this.values);
//		}

        private static int CalculateHashCode(IEnumerable<IOperand> values)
        {
            return values != null ? values.GetHashCode() : 0;
        }

        private static IEnumerable<IOperand> GetLongOperands(ICollection<long?> operands)
        {
            var tmpValues = new List<IOperand>(operands.Count);
            tmpValues.AddRange(operands.Select(longValue => new SingleValueOperand(longValue)));
            return tmpValues;
        }

        public string Name
        {
            get { return OPERAND_NAME; }
        }

        public string DisplayString
        {
            get
            {
                var sb = new StringBuilder();
                sb.Append(LEFT_PAREN);

                for (int i = 0; i < Values.Count(); i++)
                {
                    IOperand value = Values.ElementAt(i);
                    sb.Append(value.DisplayString);

                    if (i != Values.Count() - 1)
                    {
                        sb.Append(COMMA_SPACE);
                    }
                }
                
                sb.Append(RIGHT_PAREN);
                return sb.ToString();
            }
        }

        public T Accept<T>(IOperandVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }
    }
}