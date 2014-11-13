using System.Text;
using Wintellect.PowerCollections;

namespace QuoteFlow.Models.Assets.Search.Searchers.Util
{
    public class TextTermEscaper
    {
        // the full list of reserved chars in Lucene:
        // '\\', '+', '-', '!', '(', ')', ':', '^', '[', ']', '\"', '{', '}', '~', '*', '?', '|', '&'
        private static readonly Set<char> chars = new Set<char>(new[] {'\\', ':'});

        public static string Escape(char[] input)
        {
            return (new TextTermEscaper()).Get(input);
        }

        public virtual string Get(char[] input)
        {
            var escaped = new StringBuilder(input.Length * 2);
            foreach (char c in input)
            {
                if (chars.Contains(c))
                {
                    escaped.Append('\\');
                }
                escaped.Append(c);
            }
            return escaped.ToString();
        }
    }
}