using System;
using System.Collections.Generic;
using System.IO;
using Lucene.Net.Analysis;
using QuoteFlow.Api.Asset.Index.Analyzer;
using QuoteFlow.Api.Infrastructure.Lucene;
using QuoteFlow.Api.Lucene.Index;

namespace QuoteFlow.Api.Asset.Index
{
    public class QuoteFlowAnalyzer : global::Lucene.Net.Analysis.Analyzer
    {
        private readonly bool _indexing;
		private readonly Stemming _stemming;
		private readonly StopWordRemoval _stopWordRemoval;

		public enum Stemming
		{
			On, Off
		}

		public enum StopWordRemoval
		{
			On, Off
		}

		public static readonly global::Lucene.Net.Analysis.Analyzer AnalyzerForIndexing = new PerFieldIndexingAnalyzer();

		public static readonly global::Lucene.Net.Analysis.Analyzer AnalyzerForSearching = new QuoteFlowAnalyzer(false, Stemming.On, StopWordRemoval.On);

		public static readonly global::Lucene.Net.Analysis.Analyzer AnalyzerForExactSearching = new QuoteFlowAnalyzer(false, Stemming.Off, StopWordRemoval.Off);

        public readonly IDictionary<string, global::Lucene.Net.Analysis.Analyzer> Analyzers = new Dictionary<string, global::Lucene.Net.Analysis.Analyzer>();

		private readonly global::Lucene.Net.Analysis.Analyzer _fallbackAnalyzer;

		public QuoteFlowAnalyzer(bool indexing, Stemming stemming, StopWordRemoval stopWordRemoval)
		{
			_indexing = indexing;
			_stemming = stemming;
			_stopWordRemoval = stopWordRemoval;
		    _fallbackAnalyzer = new SimpleAnalyzer();
            Analyzers.Add("english", MakeAnalyzer());
            
            // todo lucene 4.8
            //_fallbackAnalyzer = new SimpleAnalyzer(LuceneVersion.Get(), this.indexing);
		    _fallbackAnalyzer = new SimpleAnalyzer();
		}

        internal global::Lucene.Net.Analysis.Analyzer MakeAnalyzer()
        {
            return new EnglishAnalyzer(LuceneVersion.Get(),
                _indexing, _stemming == Stemming.On
                    ? TokenFilters.English.Stemming.Aggressive
                    : TokenFilters.General.Stemming.None,
                _stopWordRemoval == StopWordRemoval.On
                    ? TokenFilters.English.StopWordRemoval.DefaultSet
                    : TokenFilters.General.StopWordRemoval.None);

            // Deep fallback
            return _fallbackAnalyzer;
        }

        /// <summary>
        /// Create a token stream for this analyzer.
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="reader"></param>
        /// <returns></returns>
        public override TokenStream TokenStream(string fieldName, TextReader reader)
        {
            if (fieldName == null)
            {
                fieldName = "";
            }
            // end workaround
            return FindAnalyzer().TokenStream(fieldName, reader);
        }

        /// <summary>
        /// We do this because Lucene insists we subclass this and make it final.
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="reader"></param>
        /// <returns></returns>
        public override TokenStream ReusableTokenStream(string fieldName, TextReader reader)
        {
            return base.ReusableTokenStream(fieldName, reader);
        }

        private global::Lucene.Net.Analysis.Analyzer FindAnalyzer()
        {
            global::Lucene.Net.Analysis.Analyzer analyzer;
            
            try
            {
                analyzer = Analyzers["english"];
            }
            catch (Exception e)
            {
                //log.error("Invalid indexing language: '" + language + "', defaulting to '" + APKeys.Languages.OTHER + "'.");
                analyzer = _fallbackAnalyzer;
            }
            
            return analyzer ?? (analyzer = _fallbackAnalyzer);
        }
    }
}