using QuoteFlow.Models.Search.Jql.Query;

namespace QuoteFlow.Models.Search.Jql.Parser
{
    public interface IJqlQueryParser
    {
        /// <summary>
        /// Parse the passed JQL string into its SearchQuery representation.
        /// </summary>
        /// <param name="jqlQuery"> the JQL string to parse. Must not be <code>null</code> or blank. </param>
        /// <returns> the Query representation of the passed jql string. Never null. </returns>
        /// <exception cref="JqlParseException"> if an error occurs while parsing the query. </exception>
        /// <exception cref="IllegalArgumentException"> if jqlQuery  is <code>null</code> or blank. </exception>
        IQuery ParseQuery(string jqlQuery);

        /// <summary>
        /// Determines whether or not the passed string is a valid JQL field name.
        /// </summary>
        /// <param name="fieldName"> the field name to check. </param>
        /// <returns> true if the passed string is a valid field name or false otherwise.</returns>
        bool IsValidFieldName(string fieldName);

        /// <summary>
        /// Determines whether or not the passed string is a valid JQL function argument.
        /// </summary>
        /// <param name="argument"> the function argument to check. </param>
        /// <returns> true if the passed function argument is valid or false otherwise.</returns>
        bool IsValidFunctionArgument(string argument);

        /// <summary>
        /// Determines whether or not the passed string is a valid JQL function name.
        /// </summary>
        /// <param name="functionName"> the function name to check. </param>
        /// <returns> true if the passed function name is valid or false otherwise. </returns>
        bool IsValidFunctionName(string functionName);

        /// <summary>
        /// Determines whether or not the passed string is a valid JQL value.
        /// </summary>
        /// <param name="value"> the value to check. </param>
        /// <returns> true if the passed value is valid or false otherwise. </returns>
        bool IsValidValue(string value);
 
    }
}