using System;
using System.IO;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;
using Version = Lucene.Net.Util.Version;

namespace QuoteFlow.Models.Assets.Index.Analyzer
{
    /*
	 * Note: checked for Lucene 2.9 compatibility.
	 */
    public class EnglishAnalyzer : TextAnalyzer
    {
        private readonly Version version;
        private readonly Func<TokenStream, TokenStream> stemmingAlgorithm;
        private readonly Func<TokenStream, TokenStream> stopWordFilter;

        public EnglishAnalyzer(Version version, bool indexing, Func<TokenStream, TokenStream> stemmingStrategy, Func<TokenStream, TokenStream> stopWordFilter)
            : base(indexing)
        {
            this.stemmingAlgorithm = stemmingStrategy;
            this.stopWordFilter = stopWordFilter;
            this.version = version;
        }

        /// <summary>
        /// Create a token stream for this analyzer.
        /// </summary>
        /// <param name="fieldname"></param>
        /// <param name="reader"></param>
        /// <returns></returns>
        public override TokenStream TokenStream(string fieldname, TextReader reader)
        {
            TokenStream result = new StandardTokenizer(version, reader);
            result = new StandardFilter(result);
            result = WrapStreamForIndexing(result);

            result = new LowerCaseFilter(result);
            result = stopWordFilter.Invoke(result);

            result = WrapStreamForWilcardSearchSupport(result);
            result = stemmingAlgorithm.Invoke(result);

            return result;
        }
    }
}