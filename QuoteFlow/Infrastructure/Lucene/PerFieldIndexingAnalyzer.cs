using System.IO;
using Lucene.Net.Analysis;
using QuoteFlow.Models.Assets.Index;
using QuoteFlow.Models.Assets.Index.Indexers.Phrase;

namespace QuoteFlow.Infrastructure.Lucene
{
    /// <summary>
    /// An <seealso cref="Analyzer"/> that delegates analysis tasks to the appropriate <seealso cref="QuoteFlowAnalyzer"/>
    /// instance depending on whether it is a standard text field or a phrase query support text field.
    /// </summary>
    public class PerFieldIndexingAnalyzer : Analyzer
    {
        private readonly Analyzer _phraseQuerySupportTextFieldAnalyzer = new QuoteFlowAnalyzer(true, QuoteFlowAnalyzer.Stemming.Off, QuoteFlowAnalyzer.StopWordRemoval.Off);
        private readonly Analyzer _textFieldIndexingAnalyzer = new QuoteFlowAnalyzer(true, QuoteFlowAnalyzer.Stemming.On, QuoteFlowAnalyzer.StopWordRemoval.On);

        public override TokenStream TokenStream(string fieldName, TextReader reader)
        {
            if (PhraseQuerySupportField.IsPhraseQuerySupportField(fieldName))
            {
                return _phraseQuerySupportTextFieldAnalyzer.TokenStream(fieldName, reader);
            }
            return _textFieldIndexingAnalyzer.TokenStream(fieldName, reader);
        }

        public override TokenStream ReusableTokenStream(string fieldName, TextReader reader)
        {
            if (PhraseQuerySupportField.IsPhraseQuerySupportField(fieldName))
            {
                return _phraseQuerySupportTextFieldAnalyzer.ReusableTokenStream(fieldName, reader);
            }
            return _textFieldIndexingAnalyzer.ReusableTokenStream(fieldName, reader);
        }
    }
}