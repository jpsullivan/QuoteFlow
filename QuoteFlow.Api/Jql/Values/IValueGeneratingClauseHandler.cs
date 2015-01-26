namespace QuoteFlow.Api.Jql.Values
{
    /// <summary>
    /// Implement this if you want to participate in the JQL autocomplete functionality.
    /// </summary>
    public interface IValueGeneratingClauseHandler
    {
        IClauseValuesGenerator GetClauseValuesGenerator();
    }
}