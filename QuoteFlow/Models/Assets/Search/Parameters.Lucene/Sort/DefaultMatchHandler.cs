using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Lucene.Net.Index;

namespace QuoteFlow.Models.Assets.Search.Parameters.Lucene.Sort
{
    /// <summary>
    /// The default match handler used by <see cref="QuoteFlowLuceneFieldFinder.GetMatches(IndexReader, String)"/>.
    /// 
    /// Builds an array of collections of strings, where the array index is the document number 
    /// and the collection contains the values for that term.  In QuoteFlow's terms, the array 
    /// index is a key for looking up an issue and the collection contains the values assigned 
    /// to the field we are matching against. 
    /// 
    /// This array built here is a memory eating monster and we take special care to eat as little as possible.
    /// This matcher labours under the assumption that nearly all documents have single values for most terms,
    /// even in the case of multi-valued fields, such as component or fixVersion, most documents have only a single value,
    /// often the empty value, "-1".
    /// 
    /// We use a shared singleton for any single values and only build a mutable collection once we go past a single value.
    /// This has no performance cost even in the case where there are > 1 values, aside from the size() == 1 comparison.
    /// </summary>
    public class DefaultMatchHandler : IMatchHandler
    {
        private static readonly List<string> NULL_SINGLETON = new List<string>();
        private readonly List<string>[] docToTerms;

        // Note: The matcher will feed up the same term value over repeatedly
        // for multiple docs before moving on to the next term value.  These
        // two fields allow us to reuse singletons for as long
        private object previousTermValue = null;
        private List<string> currentSingleton = NULL_SINGLETON;

        public DefaultMatchHandler(int maxdoc)
        {
            docToTerms = new List<string>[maxdoc];
        }

        public void HandleMatchedDocument(int doc, string termValue)
        {
            if ((string) previousTermValue != termValue)
            {
                previousTermValue = termValue;
                currentSingleton = new List<string>() { termValue };
            }
            var currentValue = docToTerms[doc];
            if (currentValue == null)
            {
                docToTerms[doc] = currentSingleton;
            }
            else if (currentValue.Count() == 1)
            {
                string prevTermValue = currentValue.ElementAt(0);
                currentValue = new List<string>(2);
                currentValue.Add(prevTermValue);
                currentValue.Add(termValue);
                docToTerms[doc] = currentValue;
            }
            else
            {
                currentValue.Add(termValue);
            }
        }

        public List<string>[] Results { get { return docToTerms; } } 
    }
}