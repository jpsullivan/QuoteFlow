using QuoteFlow.Core.Jql.Util;

namespace QuoteFlow.Core.Jql.Builder
{
    public class JqlClauseBuilderFactory : IJqlClauseBuilderFactory
    {
        public JqlDateSupport DateSupport { get; protected set; }

        public JqlClauseBuilderFactory(JqlDateSupport dateSupport)
        {
            DateSupport = dateSupport;
        }

        public IJqlClauseBuilder NewJqlClauseBuilder(JqlQueryBuilder parent)
        {
            return new JqlClauseBuilder(parent, new PrecedenceSimpleClauseBuilder(), DateSupport);
        }
    }
}
