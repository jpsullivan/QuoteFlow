using QuoteFlow.Api.Jql.Util;

namespace QuoteFlow.Core.Jql.Builder
{
    public class JqlClauseBuilderFactory : IJqlClauseBuilderFactory
    {
        public IJqlDateSupport DateSupport { get; protected set; }

        public JqlClauseBuilderFactory(IJqlDateSupport dateSupport)
        {
            DateSupport = dateSupport;
        }

        public IJqlClauseBuilder NewJqlClauseBuilder(JqlQueryBuilder parent)
        {
            return new JqlClauseBuilder(parent, new PrecedenceSimpleClauseBuilder(), DateSupport);
        }
    }
}
