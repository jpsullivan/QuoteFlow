using System.Collections.Generic;
using QuoteFlow.Models.Search.Jql.Query.Clause;

namespace QuoteFlow.Models.Search.Jql.Util
{
    /// <summary>
    /// A utility code to help dealing with JQL strings. 
    /// </summary>
    public interface IJqlStringSupport
    {
        /// <summary>
        /// Encode the passed string value into a safe JQL value if necessary. The value will 
        /// not be encoded if it is already safe.
        /// </summary>
        /// <param name="value">The value to encode.</param>
        /// <returns>the encoded string.</returns>
        string EncodeStringValue(string value);

        /// <summary>
        /// Encode the passed string into a safe JQL function argument. The value will not be encoded if it is already safe.
        /// </summary>
        /// <param name="argument">the string to encode.</param>
        /// <returns>the encoded string.</returns>
        string EncodeFunctionArgument(string argument);

        /// <summary>
        /// Encode the passed string into a safe JQL function name. This value will not be encoded if it is not already safe.
        /// </summary>
        /// <param name="functionName"></param>
        /// <returns></returns>
        string EncodeFunctionName(string functionName);

        /// <summary>
        /// Encode the passed string into a safe JQL field name. This value will not be encoded if it is not already safe.
        /// </summary>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        string EncodeFieldName(string fieldName);

        /// <summary>
        ///  Generates a JQL string representation for the passed query. The JQL string is always 
        /// generated, that is, <see cref="Query#getQueryString()"/> is completely ignored if it exists. 
        /// The returned JQL is automatically escaped as necessary.
        /// </summary>
        /// <param name="query">the query. Cannot be null.</param>
        /// <returns>the generated JQL string representation of the passed query.</returns>
        string GenerateJqlString(Query.Query query);

        /// <summary>
        /// Generates a JQL string representation for the passed clause. The returned JQL is 
        /// automatically escaped as necessary.
        /// </summary>
        /// <param name="clause">the clause. Cannot be null.</param>
        /// <returns>the generated JQL string representation of the passed clause.</returns>
        string GenerateJqlString(IClause clause);

        /// <summary>
        /// Returns all the reserved words for the JQL language.
        /// </summary>
        /// <returns></returns>
        HashSet<string> GetJqlReservedWords();
    }
}