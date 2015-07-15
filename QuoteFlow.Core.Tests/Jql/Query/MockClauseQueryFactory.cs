using System;
using QuoteFlow.Api.Jql.Query;
using QuoteFlow.Api.Jql.Query.Clause;
using QuoteFlow.Api.Util.Support;

namespace QuoteFlow.Core.Tests.Jql.Query
{
    public class MockClauseQueryFactory : IClauseQueryFactory
    {
        private static readonly AtomicInteger Count = new AtomicInteger();

        private int count;

        public MockClauseQueryFactory()
        {
            this.count = Count.GetAndIncrement();
        }


        public QueryFactoryResult GetQuery(IQueryCreationContext queryCreationContext, ITerminalClause terminalClause)
        {
            throw new InvalidOperationException();
        }

        protected bool Equals(MockClauseQueryFactory other)
        {
            return count == other.count;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((MockClauseQueryFactory) obj);
        }

        public override int GetHashCode()
        {
            return count;
        }

        public override string ToString()
        {
            return string.Format("Mock Query Factory {0}", count);
        }
    }
}