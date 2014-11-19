using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Tokenattributes;

namespace QuoteFlow.Infrastructure.Lucene
{
    /// <summary>
    /// This TokenFilter emits each incoming token twice once as keyword and once non-keyword, in other words once with
    /// <see cref="IKeywordAttribute.Keyword()"/> set to <code>true</code> and once set to <code>false</code>.
    /// This is useful if used with a stem filter that respects the <seealso cref="IKeywordAttribute"/> to 
    /// index the stemmed and the un-stemmed version of a term into the same field.
    /// </summary>
    public sealed class KeywordRepeatFilter : TokenFilter
    {
        private readonly IKeywordAttribute _keywordAttribute;
        private readonly IPositionIncrementAttribute _posIncAttr;
        private State _state;

        /// <summary>
        /// Construct a token stream filtering the given input.
        /// </summary>
        public KeywordRepeatFilter(TokenStream input)
            : base(input)
        {
            //_keywordAttribute = AddAttribute(typeof(IKeywordAttribute));
            _keywordAttribute = AddAttribute<IKeywordAttribute>();
            _posIncAttr = AddAttribute<IPositionIncrementAttribute>();
        }

        public override void Reset()
        {
            base.Reset();
            _state = null;
        }

        public override bool IncrementToken()
        {
            if (_state != null)
            {
                RestoreState(_state);
                _posIncAttr.PositionIncrement = 0;
                _keywordAttribute.Keyword = false;
                _state = null;
                return true;
            }
            
            if (input.IncrementToken())
            {
                _state = CaptureState();
                _keywordAttribute.Keyword = true;
                return true;
            }

            return false;
        }
    }
}