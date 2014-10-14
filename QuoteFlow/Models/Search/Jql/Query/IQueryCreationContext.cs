namespace QuoteFlow.Models.Search.Jql.Query
{
    /// <summary>
    /// A parameter object that encapsulates the context required when creating queries in the JQL way.
    /// </summary>
    public interface IQueryCreationContext
    {
        /// <summary>
        /// Gets the user in this context; null signifies the anonymous user.
        /// </summary>
        User User { get; set; }

        /// <summary>
        /// true if security should be overriden when creating the lucene query or evaluating JQL functions.
        /// </summary>
        bool SecurityOverriden { get;set; }
    }
}