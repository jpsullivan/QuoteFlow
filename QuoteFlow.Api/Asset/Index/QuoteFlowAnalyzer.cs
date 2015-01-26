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
        private readonly bool indexing;
		private readonly Stemming stemming;
		private readonly StopWordRemoval stopWordRemoval;

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

		private readonly global::Lucene.Net.Analysis.Analyzer fallbackAnalyzer;

		public QuoteFlowAnalyzer(bool indexing, Stemming stemming, StopWordRemoval stopWordRemoval)
		{
			this.indexing = indexing;
			this.stemming = stemming;
			this.stopWordRemoval = stopWordRemoval;
		    fallbackAnalyzer = new SimpleAnalyzer();
            Analyzers.Add("english", MakeAnalyzer());
		    //fallbackAnalyzer = new SimpleAnalyzer(LuceneVersion.Get(), this.indexing);
		}

        internal global::Lucene.Net.Analysis.Analyzer MakeAnalyzer()
        {
            return new EnglishAnalyzer(LuceneVersion.Get(),
                indexing, stemming == Stemming.On
                    ? TokenFilters.English.Stemming.Aggressive
                    : TokenFilters.General.Stemming.None,
                stopWordRemoval == StopWordRemoval.On
                    ? TokenFilters.English.StopWordRemoval.DefaultSet
                    : TokenFilters.General.StopWordRemoval.None);

            // Deep fallback
            return fallbackAnalyzer;
        }

        /// <summary>
        /// Create a token stream for this analyzer.
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="reader"></param>
        /// <returns></returns>
        public override TokenStream TokenStream(string fieldName, TextReader reader)
        {
            // workaround for https://issues.apache.org/jira/browse/LUCENE-1359
            // reported here: http://jira.atlassian.com/browse/JRA-16239
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
                analyzer = fallbackAnalyzer;
            }
            if (analyzer == null)
            {
                //log.error("Invalid indexing language: '" + language + "', defaulting to '" + APKeys.Languages.OTHER + "'.");
                analyzer = fallbackAnalyzer;
            }
            return analyzer;
        }
    }
}