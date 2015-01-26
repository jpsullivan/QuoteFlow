using Wintellect.PowerCollections;

namespace QuoteFlow.Api.Util
{
    public interface IMessageSet
    {
        /// <summary>
        /// Message levels
        /// </summary>
        MessageSetLevel Level { get; set; }

        /// <summary>
        /// A unique set of error messages.
        /// </summary>
        Set<string> ErrorMessages { get; }

        /// <summary>
        /// A unique set of warning messages. These messages are seperate from
        /// the error messages.
        /// </summary>
        Set<string> WarningMessages { get; }

        /// <summary>
        /// Returns true if there are error messages, otherwise false.
        /// </summary>
        /// <returns></returns>
        bool HasAnyErrors();

        /// <summary>
        /// Returns true if there are warning messages, otherwise false.
        /// </summary>
        /// <returns></returns>
        bool HasAnyWarnings();

        /// <summary>
        /// Returns true if there are messages of any type, otherwise false.
        /// </summary>
        /// <returns></returns>
        bool HasAnyMessages();

        /// <summary>
        /// Will concatenate this message set with the provided message set.
        /// All new errors and warnings will be added to the existing messages.
        /// </summary>
        /// <param name="messageSet"></param>
        void AddMessageSet(IMessageSet messageSet);

        /// <summary>
        /// Adds a message with the given warning / error level.
        /// </summary>
        /// <param name="level">The message level.</param>
        /// <param name="errorMessage">The message to add.</param>
        void AddMessage(MessageSetLevel level, string errorMessage);

        /// <summary>
        /// Adds an error message.
        /// </summary>
        /// <param name="errorMessage">The error message to add.</param>
        void AddErrorMessage(string errorMessage);

        /// <summary>
        /// Adds a warning message.
        /// </summary>
        /// <param name="warningMessage">The message to add.</param>
        void AddWarningMessage(string warningMessage);
    }

    public enum MessageSetLevel
    {
        Warning,
        Error
    }
}