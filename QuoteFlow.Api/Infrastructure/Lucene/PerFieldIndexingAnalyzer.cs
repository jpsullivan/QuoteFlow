using System.IO;
using Lucene.Net.Analysis;
using QuoteFlow.Api.Asset.Index;
using QuoteFlow.Api.Asset.Index.Indexers.Phrase;

namespace QuoteFlow.Api.Infrastructure.Lucene
{
    /// <summary>
    /// An <see cref="Analyzer"/> that delegates analysis tasks to the appropriate <see cref="QuoteFlowAnalyzer"/>
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