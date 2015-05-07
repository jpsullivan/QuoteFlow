using System.Collections.Generic;

namespace QuoteFlow.Api.Util
{
    public interface IErrorCollection
    {
        /// <summary>
        /// Add a field-specific error message. 
        /// </summary>
        /// <param name="field"> Field name, eg. "assignee" </param>
        /// <param name="message"> Error message. </param>
        void AddError(string field, string message);

        /// <summary>
        /// Add error message relating to system state (not field-specific). 
        /// </summary>
        /// <param name="message"> Error message. </param>
        void AddErrorMessage(string message);

        /// <summary>
        /// Get all non field-specific error messages. 
        /// </summary>
        /// <returns> Collection of error Strings. </returns>
        ICollection<string> ErrorMessages { get; set; }

        /// <summary>
        /// Get error messages, then get rid of them. 
        /// </summary>
        /// <returns> The (now cleared) error messages. </returns>
        ICollection<string> FlushedErrorMessages { get; }

        /// <summary>
        /// Get all field-specific errors. 
        /// </summary>
        /// <returns> Map of String: String pairs, eg. {"assignee": "Assignee is required"} </returns>
        IDictionary<string, string> Errors { get; }

        /// <summary>
        /// Populate this ErrorCollection with general and field-specific errors. 
        /// </summary>
        /// <param name="errors"> ErrorCollection whose errors/messages we obtain. </param>
        void AddErrorCollection(IErrorCollection errors);

        /// <summary>
        /// Append new error messages to those already collected. 
        /// </summary>
        /// <param name="errorMessages"> Collection of error strings. </param>
        void AddErrorMessages(ICollection<string> errorMessages);

        /// <summary>
        /// Append new field-specific errors to those already collected. 
        /// </summary>
        /// <param name="errors"> of String: String pairs, eg. {"assignee": "Assignee is required"} </param>
        void AddErrors(IDictionary<string, string> errors);

        /// <summary>
        /// Whether any errors (of any type - field-specific or otherwise) have been collected. 
        /// </summary>
        /// <returns> true if any errors (of any type - field-specific or otherwise) have been collected. </returns>
        bool HasAnyErrors(); 
    }
}