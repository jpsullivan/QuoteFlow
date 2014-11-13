using System.Collections.Generic;
using System.Linq;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Analysis.Tokenattributes;

namespace QuoteFlow.Models.Assets.Index.Analyzer
{
    /// <summary>
    /// This Filter does some final filtering on the Tokens returned by the standard Lucene 
    /// tokenizers in order to create the exact tokens required for QuoteFlow.
    /// 
    /// Currently, the StandardTokenizer takes anything of the 'alpha.alpha.alpha' form, and keeps it all together, because
    /// it htinks it may be a server hostname (like "www.atlassian.com").
    /// This is useful, however it prevents searches on the words between the dots.
    /// An example is searching for 'NullPointerException' when 'java.lang.NullPointerException' has
    /// been indexed.
    /// This filter tokenizes the individual words, as well as the full phrase, allowing searching to
    /// be done on either. (JRA-6397)
    /// 
    /// In addition, a comma separated list of numbers (eg "123,456,789") is not tokenized at the commas.
    /// This prevents searching on just "123".
    /// This filter tokenizes the individual numbers, as well as the full phrase, allowing searching to
    /// be done on either. (JRA-7774)
    /// </summary>
    public class SubtokenFilter : TokenFilter
    {
        private static readonly string TOKEN_TYPE_HOST = StandardTokenizer.TOKEN_TYPES[StandardTokenizer.HOST];
        private static readonly string TOKEN_TYPE_NUM = StandardTokenizer.TOKEN_TYPES[StandardTokenizer.NUM];
        private const string TOKEN_TYPE_EXCEPTION = "EXCEPTION";

        private CharTermAttribute termAttribute;
        private PositionIncrementAttribute incrementAttribute;
        private TypeAttribute typeAttribute;

        private State current;
        private string nextType;
        private List<char> subtokenStack = new List<char>();

        public SubtokenFilter(TokenStream tokenStream)
            : base(tokenStream)
        {
            termAttribute = AddAttribute<CharTermAttribute>();
            incrementAttribute = AddAttribute<PositionIncrementAttribute>();
            AddAttribute<TypeAttribute>();
        }

        public override bool IncrementToken()
        {
            if (subtokenStack.Count > 0)
            {
                RestoreState(current);

                var remove = subtokenStack.ElementAt(0);
                subtokenStack.RemoveAt(0);
                termAttribute.SetLength(0).Append(remove);
                incrementAttribute.PositionIncrement = 0;
                typeAttribute.Type = nextType;

                return true;
            }

            if (!input.IncrementToken())
            {
                return false;
            }

            if (TOKEN_TYPE_HOST.Equals(typeAttribute.Type) || TypeAttribute.DEFAULT_TYPE.Equals(typeAttribute.Type))
            {
                AddSubtokensToStack('.', TOKEN_TYPE_EXCEPTION);
            }
            // Comma separated alphanum are not separated correctly
            else if (TOKEN_TYPE_NUM.Equals(typeAttribute.Type))
            {
                AddSubtokensToStack(',', TOKEN_TYPE_NUM);
            }

            return true;
        }

        private void AddSubtokensToStack(char separatorChar, string newTokenType)
        {
            char[] termBuffer = termAttribute.Buffer();
            int termLength = termAttribute.Length;
            int offset = 0;

            // We iterate over the array, trying to find the separatorChar ('.' or ',')
            for (int index = 0; index <= termLength; index++)
            {
                // Note that we actually iterate past the last character in the array. At this point index == termLength.
                // We must check for this condition first to stop ArrayIndexOutOfBoundsException.
                // Being at the end of the array is a subtoken border just like the separator character ('.'), except we don't want to
                // add a duplicate token if no separator was already found. Hence we also check for offset > 0.
                if ((index < termLength && termBuffer[index] == separatorChar) || (index == termLength && offset > 0))
                {
                    int subtokenLength = index - offset;
                    // Check that this is not an "empty" subtoken
                    if (subtokenLength > 0)
                    {
                        if (subtokenStack.Count == 0)
                        {
                            nextType = newTokenType;
                            current = CaptureState();
                        }
                        subtokenStack.Add(termAttribute.SubSequence(offset, subtokenLength + offset));
                    }
                    offset = index + 1;
                }
            }
        }

        public override void Reset()
        {
            base.Reset();
            current = null;
            nextType = null;
            subtokenStack.Clear();
        }
    }
}