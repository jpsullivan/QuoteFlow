using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using Lucene.Net.Store;
using QuoteFlow.Api.Asset.Index;
using QuoteFlow.Api.Lucene.Index;
using Xunit;

namespace QuoteFlow.Core.Tests.Asset.Index
{
    public class QuoteFlowAnalyzerTests
    {
        /// <summary>
        /// Ensures that tokens are created for each number present in a comma-separated string. 
        /// This is a regression test for JRA-7774.
        /// </summary>
        [Fact]
        public void ShouldTokenizeNumbers_GivenTheyAreInA_CommaSeperatedString()
        {
            AssertSearchFound("1602,1712,0000", "1602");
            AssertSearchFound("1602,1712,0000", "1712");
            AssertSearchFound("1602,1712,0000", "0000");
            AssertSearchFound("1602,1712,0000", "1602,1712,0000");
            AssertSearchFound("abc,def,ghi", "def");
        }

        [Fact]
        public virtual void TestHandlingStopWords()
        {
            AssertSearchFound("This is a bug", "This is a bug");
        }

        [Fact]
        public virtual void TestStopWords()
        {
            AssertSearchNotFound("The quick brown fox.", "the");
            AssertSearchNotFound("The quick brown fox.", "the");
            AssertSearchNotFound("The quick brown fox.", "the");
//            AssertSearchFound(APKeys.Languages.FRENCH, "The quick brown fox.", "the");

            AssertSearchFound("Le quick brown fox.", "le");
            AssertSearchFound("Le quick brown fox.", "le");
            AssertSearchFound("Le quick brown fox.", "le");
//            AssertSearchNotFound(APKeys.Languages.FRENCH, "Le quick brown fox.", "le");

            AssertSearchFound("Der quick brown fox.", "der");
            AssertSearchFound("Der quick brown fox.", "der");
            AssertSearchFound("Der quick brown fox.", "der");
//            AssertSearchNotFound(APKeys.Languages.GERMAN, "Der quick brown fox.", "der");
        }

        [Fact]
        public virtual void TestStemming()
        {
            // This stemming should work in English using the agressive stemmer
            AssertSearchFound("The child walked.", "walk");
            AssertSearchFound("The child will walk.", "walked");

            // This stemming should also work in English using the moderate stemmer
            AssertSearchFound("The child walked.", "walk");
            AssertSearchFound("The child will walk.", "walked");

//            // but not in French
//            AssertSearchNotFound(APKeys.Languages.FRENCH, "The child walked.", "walk");
//            AssertSearchNotFound(APKeys.Languages.FRENCH, "The child will walk.", "walked");
//
//            // This stemming should work in French
//            AssertSearchFound(APKeys.Languages.FRENCH, "L'enfant a march\u00e9.", "marchera");
//            AssertSearchFound(APKeys.Languages.FRENCH, "l'enfant marchera.", "march\u00e9");
//
//            // but not in English using the agressive stemmer
//            AssertSearchNotFound(APKeys.Languages.ENGLISH, "L'enfant a march\u00e9.", "marchera");
//            AssertSearchNotFound(APKeys.Languages.ENGLISH, "l'enfant marchera.", "march\u00e9");
//
//            // and not in English using the moderate stemmer
//            AssertSearchNotFound(APKeys.Languages.ENGLISH_MODERATE_STEMMING, "L'enfant a march\u00e9.", "marchera");
//            AssertSearchNotFound(APKeys.Languages.ENGLISH_MODERATE_STEMMING, "l'enfant marchera.", "march\u00e9");
        }

        private void AssertSearchFound(string textToSearch, string searchTerm)
        {
            var hits = GetHitsForSearch(textToSearch, searchTerm);

            var msg = $"Search Term '{searchTerm}' wasn't found in text '{textToSearch}'";
            Assert.Equal(1, hits.TotalHits);
        }

        private void AssertSearchNotFound(string textToSearch, string searchTerm)
        {
            var hits = GetHitsForSearch(textToSearch, searchTerm);

            var msg = $"Search Term '{searchTerm}' was found in text '{textToSearch}'";
            Assert.Equal(0, hits.TotalHits);
        }

        private TopDocs GetHitsForSearch(string textToSearch, string searchTerm)
        {
            var indexingAnalyzer = Mocks.Analyser().Indexing(true).Build();
            var searchingAnalyzer = Mocks.Analyser().Indexing(false).Build();
            var directory = new RAMDirectory();
            var writer = new IndexWriter(directory, indexingAnalyzer, true, IndexWriter.MaxFieldLength.UNLIMITED);

            var doc = new Document();
            doc.Add(new Field("term", textToSearch, Field.Store.YES, Field.Index.ANALYZED));
            writer.AddDocument(doc);
            writer.Dispose();

            var searcher = new IndexSearcher(directory);
            var parser = new QueryParser(LuceneVersion.Get(), "term", searchingAnalyzer);
            var query = parser.Parse(searchTerm);

            // This is useful for debugging:
//            AnalyzerUtils.DisplayTokensWithFullDetails(indexingAnalyzer, textToSearch);
//            AnalyzerUtils.DisplayTokensWithFullDetails(searchingAnalyzer, searchTerm);

            return searcher.Search(query, int.MaxValue);
        }

        private class Mocks
        {
            internal static AnalyzerBuilder Analyser()
            {
                return new AnalyzerBuilder();
            }

            internal sealed class AnalyzerBuilder
            {
                private bool _indexing;

                internal AnalyzerBuilder Indexing(bool indexing)
                {
                    _indexing = indexing;
                    return this;
                }

                internal QuoteFlowAnalyzer Build()
                {
                    return new QuoteFlowAnalyzer(_indexing, QuoteFlowAnalyzer.Stemming.On, QuoteFlowAnalyzer.StopWordRemoval.On);
                }
            }
        }

    }
}