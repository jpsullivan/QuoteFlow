namespace QuoteFlow.Core.Jql.Builder
{
    /// <summary>
    /// Factory for creating new instances of JqlClauseBuilder.
    /// You normally wouldn't call this directly, it is used inside <seealso cref="JqlQueryBuilder"/>.
    /// </summary>
    public interface IJqlClauseBuilderFactory
    {
        IJqlClauseBuilder NewJqlClauseBuilder(JqlQueryBuilder parent);
    }
}