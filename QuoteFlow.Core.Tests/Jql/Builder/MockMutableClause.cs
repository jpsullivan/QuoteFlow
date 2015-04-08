using QuoteFlow.Api.Jql.Query.Clause;
using QuoteFlow.Core.Jql.Builder;

namespace QuoteFlow.Core.Tests.Jql.Builder
{
    public sealed class MockMutableClause : IMutableClause
    {
        private IClause Clause { set; get; }
        private string Message { get; set; }

        public MockMutableClause(IClause clause, string message = null)
        {
            Clause = clause;
            Message = message;
        }

        public IMutableClause Combine(BuilderOperator logicalOperator, IMutableClause otherClause)
        {
            return this;
        }

        public IClause AsClause()
        {
            return Clause;
        }

        public IMutableClause Copy()
        {
            return this;
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

            MockMutableClause that = (MockMutableClause)o;

            if (Clause != null ? !Clause.Equals(that.Clause) : that.Clause != null)
            {
                return false;
            }

            return true;
        }

        public override int GetHashCode()
        {
            return Clause != null ? Clause.GetHashCode() : 0;
        }

        public override string ToString()
        {
            return Message;
        }

    }
}