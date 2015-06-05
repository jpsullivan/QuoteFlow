using System.Collections.Generic;
using QuoteFlow.Api.Jql.Operand;
using QuoteFlow.Api.Jql.Query;
using QuoteFlow.Api.Jql.Query.Clause;
using QuoteFlow.Api.Jql.Query.Operand;
using QuoteFlow.Api.Models;
using QuoteFlow.Api.Util;

namespace QuoteFlow.Core.Tests.Jql.Query.Operand
{
    public class MockOperandHandler<T> : IOperandHandler<T> where T : IOperand
    {
        private bool _listOperand;
        private readonly bool _emptyOperand;
        private readonly bool _functionOperand;
        private readonly List<QueryLiteral> _literals = new List<QueryLiteral>();

        public MockOperandHandler()
            : this(false, false, false)
        {
        }

        public MockOperandHandler(bool listOperand, bool emptyOperand, bool functionOperand)
        {
            _listOperand = listOperand;
            _emptyOperand = emptyOperand;
            _functionOperand = functionOperand;
        }

        public IMessageSet Validate(User searcher, T operand, ITerminalClause terminalClause)
        {
            throw new System.NotSupportedException("Not implemented in the mock");
        }

        public IEnumerable<QueryLiteral> GetValues(IQueryCreationContext queryCreationContext, T operand, ITerminalClause terminalClause)
        {
            return new List<QueryLiteral>(_literals);
        }

        public MockOperandHandler<T> Add(params string[] strings)
        {
            foreach (string @string in strings)
            {
                _literals.Add(new QueryLiteral(new SingleValueOperand(@string), @string));
            }
            return this;
        }

        public MockOperandHandler<T> Add(params int[] ints)
        {
            foreach (int i in ints)
            {
                _literals.Add(new QueryLiteral(new SingleValueOperand(i), i));
            }

            return this;
        }

        public MockOperandHandler<T> Add(params QueryLiteral[] lits)
        {
            _literals.AddRange(lits);
            return this;
        }

        public MockOperandHandler<T> Clear()
        {
            _literals.Clear();
            return this;
        }


        public bool IsList()
        {
            return _listOperand;
        }

        public bool IsEmpty()
        {
            return _emptyOperand;
        }

        public bool IsFunction()
        {
            return _functionOperand;
        }

        public MockOperandHandler<T> SetList(bool listOperand)
        {
            _listOperand = listOperand;
            return this;
        }   

        public override string ToString()
        {
            return string.Format("Mock literals {0}", _literals);
        }
    }
}