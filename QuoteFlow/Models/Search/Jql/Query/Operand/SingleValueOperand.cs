﻿using System;
using QuoteFlow.Models.Search.Jql.Operand;

namespace QuoteFlow.Models.Search.Jql.Query.Operand
{
    public sealed class SingleValueOperand : IOperand
    {
        public const string OPERAND_NAME = "SingleValueOperand";

        private readonly int? _intValue;
        private readonly string _stringValue;

        public SingleValueOperand(string stringValue)
        {
            _stringValue = stringValue;
            _intValue = null;
        }

        public SingleValueOperand(int? intValue)
        {
            _intValue = intValue;
            _stringValue = null;
        }

        /// <summary>
        /// Note: cannot accept an empty <seealso cref="QueryLiteral"/>.
        /// Use <seealso cref="EmptyOperand"/> instead.
        /// </summary>
        /// <param name="literal">The query literal to convert to an operand; must not be null or empty.</param>
        public SingleValueOperand(QueryLiteral literal)
        {
            if (literal.IntValue != null)
            {
                _intValue = literal.IntValue;
                _stringValue = null;
            }
            else if (literal.StringValue != null)
            {
                _stringValue = literal.StringValue;
                _intValue = null;
            }
            else
            {
                throw new ArgumentException("QueryLiteral '" + literal + "' must contain at least one non-null value");
            }
        }

        public string Name { get { return OPERAND_NAME; } }

        public string DisplayString
        {
            get
            {
                if (_intValue == null)
                {
                    return "\"" + _stringValue + "\"";
                }
                return _intValue.ToString();
            }
        }

        public T Accept<T>(IOperandVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }

        public int? IntValue { get { return _intValue; } }

        public string StringValue { get { return _stringValue; } }

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

            var that = (SingleValueOperand)o;

            if (_intValue != null ? !_intValue.Equals(that._intValue) : that._intValue != null)
            {
                return false;
            }
            if (_stringValue != null ? !_stringValue.Equals(that._stringValue) : that._stringValue != null)
            {
                return false;
            }

            return true;
        }

        public override int GetHashCode()
        {
            int result = (_intValue != null ? _intValue.GetHashCode() : 0);
            result = 31 * result + (_stringValue != null ? _stringValue.GetHashCode() : 0);
            return result;
        }

        public override string ToString()
        {
            return "Single Value Operand [" + DisplayString + "]";
        }
    }
}