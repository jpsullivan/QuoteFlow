using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.BR;
using Lucene.Net.Analysis.CJK;
using Lucene.Net.Analysis.Cz;
using Lucene.Net.Analysis.De;
using Lucene.Net.Analysis.El;
using Lucene.Net.Analysis.Fr;
using Lucene.Net.Analysis.Th;
using QuoteFlow.Configuration;
using QuoteFlow.Infrastructure.Lucene;
using QuoteFlow.Models.Assets.Index.Analyzer;

namespace QuoteFlow.Models.Assets.Index
{
    public class QuoteFlowAnalyzer : Lucene.Net.Analysis.Analyzer
    {
        private readonly bool indexing;
		private readonly Stemming stemming;
		private readonly StopWordRemoval stopWordRemoval;
        public IAppConfiguration Config { get; protected set; }

		public enum Stemming
		{
			ON,
			OFF
		}

		public enum StopWordRemoval
		{
			ON,
			OFF
		}

		public static readonly Lucene.Net.Analysis.Analyzer ANALYZER_FOR_INDEXING = new PerFieldIndexingAnalyzer();

		public static readonly Lucene.Net.Analysis.Analyzer ANALYZER_FOR_SEARCHING = new QuoteFlowAnalyzer(false, Stemming.ON, StopWordRemoval.ON);

		public static readonly Lucene.Net.Analysis.Analyzer ANALYZER_FOR_EXACT_SEARCHING = new QuoteFlowAnalyzer(false, Stemming.OFF, StopWordRemoval.OFF);

		private readonly Cache<string, Lucene.Net.Analysis.Analyzer> analyzers = CacheBuilder.newBuilder().build(new CacheLoaderAnonymousInnerClassHelper());

		private class CacheLoaderAnonymousInnerClassHelper : CacheLoader<string, Analyzer>
		{
			public CacheLoaderAnonymousInnerClassHelper()
			{
			}

			public override Analyzer load(string key)
			{
				return outerInstance.makeAnalyzer(key);
			}
		}

		private readonly Lucene.Net.Analysis.Analyzer fallbackAnalyzer;


		public QuoteFlowAnalyzer(IAppConfiguration config, bool indexing, Stemming stemming, StopWordRemoval stopWordRemoval)
		{
		    Config = config;
			this.indexing = indexing;
			this.stemming = stemming;
			this.stopWordRemoval = stopWordRemoval;
			fallbackAnalyzer = new SimpleAnalyzer(LuceneVersion.Get(), this.indexing);
		}

        internal virtual Lucene.Net.Analysis.Analyzer makeAnalyzer(string language)
        {
            return new EnglishAnalyzer(LuceneVersion.Get(),
                indexing, stemming == Stemming.ON
                    ? TokenFilters.English.Stemming.Aggressive
                    : TokenFilters.General.Stemming.none(),
                stopWordRemoval == StopWordRemoval.ON
                    ? TokenFilters.English.StopWordRemoval.DefaultSet
                    : TokenFilters.General.StopWordRemoval.none());

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
            return findAnalyzer().TokenStream(fieldName, reader);
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

        private Lucene.Net.Analysis.Analyzer findAnalyzer()
        {
            string language = Language;
            if (language == null)
            {
                return fallbackAnalyzer;
            }
            Analyzer analyzer = null;
            try
            {
                analyzer = analyzers.get(language);
            }
            catch (ExecutionException e)
            {
                log.error("Invalid indexing language: '" + language + "', defaulting to '" + APKeys.Languages.OTHER + "'.");
                analyzer = fallbackAnalyzer;
            }
            if (analyzer == null)
            {
                log.error("Invalid indexing language: '" + language + "', defaulting to '" + APKeys.Languages.OTHER + "'.");
                analyzer = fallbackAnalyzer;
            }
            return analyzer;
        }

        private string GetLanguage()
        {
            return Config.
        }
    }
}