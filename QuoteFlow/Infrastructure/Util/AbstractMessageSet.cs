using System;
using System.Linq;
using Wintellect.PowerCollections;

namespace QuoteFlow.Infrastructure.Util
{
    /// <summary>
    /// Base clase for the message set interface.
    /// </summary>
    [Serializable]
    public class AbstractMessageSet : IMessageSet
    {
        public MessageSetLevel Level { get; set; }
        public Set<string> ErrorMessages { get; private set; }
        public Set<string> WarningMessages { get; private set; }

        public AbstractMessageSet(Set<string> errorMessages, Set<string> warningMessages)
        {
            ErrorMessages = errorMessages;
            WarningMessages = warningMessages;
        }

        public bool HasAnyErrors()
        {
            return ErrorMessages.Any();
        }

        public bool HasAnyWarnings()
        {
            return WarningMessages.Any();
        }

        public bool HasAnyMessages()
        {
            return HasAnyErrors() || HasAnyWarnings();
        }

        public void AddMessageSet(IMessageSet messageSet)
        {
            if (messageSet == null) return;

            foreach (var errorMessage in messageSet.ErrorMessages)
            {
                ErrorMessages.Add(errorMessage);
            }
            foreach (var warningMessage in messageSet.WarningMessages)
            {
                WarningMessages.Add(warningMessage);
            }
        }

        public void AddMessage(MessageSetLevel level, string errorMessage)
        {
            switch (level)
            {
                case MessageSetLevel.Error:
                    AddErrorMessage(errorMessage);
                    break;
                case MessageSetLevel.Warning:
                    AddWarningMessage(errorMessage);
                    break;
                default:
                    throw new Exception(string.Format("Unrecognized MessageSet Level: {0}", level));
            }
        }

        public void AddErrorMessage(string errorMessage)
        {
            ErrorMessages.Add(errorMessage);
        }

        public void AddWarningMessage(string warningMessage)
        {
            WarningMessages.Add(warningMessage);
        }
    }
}