using System;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Snowball;
using SF.Snowball.Ext;

namespace QuoteFlow.Models.Assets.Index.Analyzer
{
    public class TokenFilters
    {
        public class General
        {
            public class Stemming
            {
                public static readonly Func<TokenStream, TokenStream> None = stream => stream;
            }

            public class StopWordRemoval
            {
                public static readonly Func<TokenStream, TokenStream> None = stream => stream;
            }
        }

        public class English
        {
            public class Stemming
            {
                public static readonly Func<TokenStream, SnowballFilter> Aggressive = stream => new SnowballFilter(stream, new EnglishStemmer());

//                public static Func<TokenStream, TokenStream> Moderate = stream => new KStemFilter(stream);
//
//                public static Func<TokenStream, TokenStream> Minimal = stream => new EnglishMinimalStemFilter(stream); 

            }

            public class StopWordRemoval
            {
                public static readonly Func<TokenStream, TokenStream> DefaultSet = stream => new StopFilter(true, stream, StopAnalyzer.ENGLISH_STOP_WORDS_SET);
            }
        }
    }
}