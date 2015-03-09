using System;

namespace QuoteFlow.Api.Asset.Fields
{
    [Serializable]
    public class FieldException : Exception
    {
        public FieldException(string message) : base(message)
        {
        }
    }
}
