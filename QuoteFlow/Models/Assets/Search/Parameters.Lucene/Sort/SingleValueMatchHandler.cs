using System.Collections.Generic;

namespace QuoteFlow.Models.Assets.Search.Parameters.Lucene.Sort
{
    /// <summary>
    /// A match handler that assumes all values it will only ever see one
    /// value per document.  This allows it to optimise the storage somewhat
    /// by avoiding the use of more complicated data structures to contain
    /// the values.
    /// </summary>
    public class SingleValueMatchHandler : IMatchHandler
    {
        private static readonly List<string> NullSingleton = new List<string>();

        private readonly List<string>[] _docToTerms;

        // Note: The matcher will feed up the same term value over repeatedly
        // for multiple docs before moving on to the next term value.  These
        // two fields allow us to reuse singletons for as long
        private object _previousTermValue;
        private List<string> _currentSingleton = NullSingleton;

        public SingleValueMatchHandler(int maxdoc)
        {
            _docToTerms = new List<string>[maxdoc];
        }

        /// <summary>
        /// Get the results from the match handler. Every element is guaranteed
        /// to be either <tt>null</tt> (meaning that document had no value for the
        /// field) or an immutable <tt>List</tt> containing exactly one value.
        /// </summary>
        /// <returns> the resulting matches </returns>
        public virtual List<string>[] Results
        {
            get { return _docToTerms; }
        }

        public void HandleMatchedDocument(int doc, string termValue)
        {
            if ((string) _previousTermValue != termValue)
            {
                _previousTermValue = termValue;
                _currentSingleton = new List<string> { termValue };
            }
            _docToTerms[doc] = _currentSingleton;
        }
    }

}