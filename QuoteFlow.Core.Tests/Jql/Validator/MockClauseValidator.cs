using System;
using QuoteFlow.Api.Jql.Query.Clause;
using QuoteFlow.Api.Jql.Validator;
using QuoteFlow.Api.Models;
using QuoteFlow.Api.Util;
using QuoteFlow.Api.Util.Support;

namespace QuoteFlow.Core.Tests.Jql.Validator
{
    public class MockClauseValidator : IClauseValidator
    {
        private static readonly AtomicInteger Count = new AtomicInteger();

        private int _count;

        public MockClauseValidator()
        {
            _count = Count.GetAndIncrement();
        }

        public IMessageSet Validate(User searcher, ITerminalClause terminalClause)
        {
            throw new InvalidOperationException();
        }

        protected bool Equals(MockClauseValidator other)
        {
            return _count == other._count;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((MockClauseValidator) obj);
        }

        public override int GetHashCode()
        {
            return _count;
        }

        public override string ToString()
        {
            return string.Format("Mock Query Validator {0}", _count);
        }
    }
}