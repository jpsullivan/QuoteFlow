﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QuoteFlow.Api.Jql.Operand;

namespace QuoteFlow.Api.Jql.Query.Operand
{
    public sealed class MultiValueOperand : IOperand
    {
        public const string OperandName = "MultiValueOperand";

        public IEnumerable<IOperand> Values { get; private set; } 
        public int HashCode { get; set; }

        private const string LeftParen = "(";
		private const string CommaSpace = ", ";
		private const string RightParen = ")";
		private readonly int _hashcode;

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

		public MultiValueOperand(IEnumerable<int?> ints)
		{
			Values = new List<IOperand>(GetLongOperands(ints));
			HashCode = CalculateHashCode(Values);
		}

		public MultiValueOperand(params int?[] ints)
		{
			Values = new List<IOperand>(GetLongOperands(ints.ToList()));
			HashCode = CalculateHashCode(Values);
		}

        public MultiValueOperand(IEnumerable<IOperand> operands) : this(operands.ToList())
        {
        }

        public MultiValueOperand(params IOperand[] operands) : this(operands.ToList())
        {
        }


		public MultiValueOperand(params QueryLiteral[] literals)
		{
			var tmpValues = new List<IOperand>(literals.Length);
			foreach (QueryLiteral literal in literals)
			{
				if (literal.IsEmpty)
				{
					tmpValues.Add(EmptyOperand.Empty);
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

        public MultiValueOperand(List<IOperand> values)
		{
            if (values.Any(value => value == null))
            {
                throw new ArgumentNullException();
            }

			Values = values;
			_hashcode = CalculateHashCode(Values);
		}

        private static int CalculateHashCode(IEnumerable<IOperand> values)
        {
            return values != null ? values.GetHashCode() : 0;
        }

        private static IEnumerable<IOperand> GetLongOperands(IEnumerable<int?> operands)
        {
            var tmpValues = new List<IOperand>(operands.Count());
            tmpValues.AddRange(operands.Select(intValue => new SingleValueOperand(intValue)));
            return tmpValues;
        }

        public string Name
        {
            get { return OperandName; }
        }

        public string DisplayString
        {
            get
            {
                var sb = new StringBuilder();
                sb.Append(LeftParen);

                for (int i = 0; i < Values.Count(); i++)
                {
                    IOperand value = Values.ElementAt(i);
                    sb.Append(value.DisplayString);

                    if (i != Values.Count() - 1)
                    {
                        sb.Append(CommaSpace);
                    }
                }
                
                sb.Append(RightParen);
                return sb.ToString();
            }
        }

        public T Accept<T>(IOperandVisitor<T> visitor)
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

            MultiValueOperand that = (MultiValueOperand) obj;

            if (Values != null ? !Values.SequenceEqual(that.Values) : that.Values != null)
            {
                return false;
            }

            return true;
        }

        public override int GetHashCode()
        {
            // Since the enclosed list is immutable, we can pre-calculate the hashCode.
            return _hashcode;
        }
    }
}