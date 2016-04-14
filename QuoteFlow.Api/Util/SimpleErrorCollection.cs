using System;
using System.Collections.Generic;

namespace QuoteFlow.Api.Util
{
    [Serializable]
    public class SimpleErrorCollection : IErrorCollection
    {
        private readonly IDictionary<string, string> _errors;
        private IList<string> _errorMessages;

        public SimpleErrorCollection()
        {
            _errors = new Dictionary<string, string>(2);
            _errorMessages = new List<string>();
        }

        public SimpleErrorCollection(IErrorCollection errorCollection)
        {
            _errors = new Dictionary<string, string>(errorCollection.Errors);
            _errorMessages = new List<string>(errorCollection.ErrorMessages);
        }

        public virtual void AddError(string field, string message)
        {
            _errors[field] = message;
        }

        public virtual void AddErrorMessage(string message)
        {
            _errorMessages.Add(message);
        }

        public virtual ICollection<string> ErrorMessages
        {
            get { return _errorMessages; }
            set { _errorMessages = new List<string>(value); }
        }

        public virtual ICollection<string> FlushedErrorMessages
        {
            get
            {
                ICollection<string> errors = ErrorMessages;
                _errorMessages = new List<string>();
                return errors;
            }
        }

        public virtual IDictionary<string, string> Errors
        {
            get { return _errors; }
        }

        public virtual void AddErrorCollection(IErrorCollection errors)
        {
            AddErrorMessages(errors.ErrorMessages);
            AddErrors(errors.Errors);
        }

        public virtual void AddErrorMessages(ICollection<string> incomingMessages)
        {
            if (incomingMessages != null && incomingMessages.Count > 0)
            {
                foreach (var incomingMessage in incomingMessages)
                {
                    AddErrorMessage(incomingMessage);
                }
            }
        }

        public virtual void AddErrors(IDictionary<string, string> incomingErrors)
        {
            if (incomingErrors == null)
            {
                return;
            }
            foreach (var mapEntry in incomingErrors)
            {
                AddError(mapEntry.Key, mapEntry.Value);
            }
        }

        public virtual bool HasAnyErrors()
        {
            return (_errors != null && _errors.Count > 0) || (_errorMessages != null && _errorMessages.Count > 0);
        }

        public override string ToString()
        {
            return string.Format("Errors: {0} \n Error Messages: {1}", Errors, ErrorMessages);
        }


        public override bool Equals(object o)
        {
            if (this == o)
            {
                return true;
            }
            if (o == null || GetType() != o.GetType())
            {
                return false;
            }

            SimpleErrorCollection that = (SimpleErrorCollection)o;

            if (!_errorMessages.Equals(that._errorMessages))
            {
                return false;
            }
            if (!_errors.Equals(that._errors))
            {
                return false;
            }

            return true;
        }

        public override int GetHashCode()
        {
            int result = _errors.GetHashCode();
            result = 31 * result + _errorMessages.GetHashCode();
            return result;
        }
    }
}