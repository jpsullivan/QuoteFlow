using System;
using System.IO;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;
using Version = Lucene.Net.Util.Version;

namespace QuoteFlow.Api.Asset.Index.Analyzer
{
    /// <summary>
    /// Note: checked for Lucene 2.9 compatibility.
    /// </summary>
    public class EnglishAnalyzer : TextAnalyzer
    {
        private readonly Version _version;
        private readonly Func<TokenStream, TokenStream> _stemmingAlgorithm;
        private readonly Func<TokenStream, TokenStream> _stopWordFilter;

        public EnglishAnalyzer(Version version, bool indexing, Func<TokenStream, TokenStream> stemmingStrategy, Func<TokenStream, TokenStream> stopWordFilter)
            : base(indexing)
        {
            _stemmingAlgorithm = stemmingStrategy;
            _stopWordFilter = stopWordFilter;
            _version = version;
        }

        /// <summary>
        /// Create a token stream for this analyzer.
        /// </summary>
        /// <param name="fieldname"></param>
        /// <param name="reader"></param>
        /// <returns></returns>
        public override TokenStream TokenStream(string fieldname, TextReader reader)
        {
            TokenStream result = new StandardTokenizer(_version, reader);
            result = new StandardFilter(result);
            result = WrapStreamForIndexing(result);

            result = new LowerCaseFilter(result);
            result = _stopWordFilter.Invoke(result);

            result = WrapStreamForWilcardSearchSupport(result);
            result = _stemmingAlgorithm.Invoke(result);

            return result;
        }
    }
}