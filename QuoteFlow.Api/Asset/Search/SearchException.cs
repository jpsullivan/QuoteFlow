using System;
using System.IO;

namespace QuoteFlow.Api.Asset.Search
{
    [Serializable]
    public class SearchException : Exception
    {
        public SearchException(IOException ex)
            : base(ex.Message)
        {
        }
    }
}
