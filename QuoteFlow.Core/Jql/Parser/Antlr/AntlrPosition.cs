using System.Text;
using Antlr.Runtime;

namespace QuoteFlow.Core.Jql.Parser.Antlr
{
    /// <summary>
    /// A simple pair to hold both a position and a type (both integer). Implementing in here to keep the grammar a clean
    /// as possible (yeah right).
    /// </summary>
    public class AntlrPosition
    {
        public int TokenType { get; set; }
        public int Index { get; set; }
        public int CharPosition { get; set; }
        public int LinePosition { get; set; }

        public AntlrPosition(int tokenType, ICharStream stream)
        {
            Index = stream.Index;
            LinePosition = stream.Line;
            CharPosition = stream.CharPositionInLine;
            TokenType = tokenType;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendFormat("Index: {0}", Index.ToString());
            sb.AppendFormat("Line Position: {0}", LinePosition.ToString());
            sb.AppendFormat("Char Position: {0}", CharPosition.ToString());
            sb.AppendFormat("Token Type: {0}", TokenType.ToString());
            return sb.ToString();
        }

        public override bool Equals(object o)
        {
            if (this == o)
            {
                return true;
            }
            if (o == null || GetType() != o.GetType())
            {
                return false;
            }

            var that = (AntlrPosition) o;

            if (CharPosition != that.CharPosition)
            {
                return false;
            }
            if (Index != that.Index)
            {
                return false;
            }
            if (LinePosition != that.LinePosition)
            {
                return false;
            }
            if (TokenType != that.TokenType)
            {
                return false;
            }

            return true;
        }

        public override int GetHashCode()
        {
            int result = TokenType;
            result = 31 * result + Index;
            result = 31 * result + CharPosition;
            result = 31 * result + LinePosition;
            return result;
        }
    }
}