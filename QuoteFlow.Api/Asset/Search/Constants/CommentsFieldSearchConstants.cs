using System.Collections.Generic;
using QuoteFlow.Api.Asset.Index;
using QuoteFlow.Api.Jql;
using QuoteFlow.Api.Jql.Query;

namespace QuoteFlow.Api.Asset.Search.Constants
{
    public class CommentsFieldSearchConstants : IClauseInformation
    {
        private static readonly CommentsFieldSearchConstants instance = new CommentsFieldSearchConstants();

        private CommentsFieldSearchConstants()
        {
            JqlClauseNames = new ClauseNames(AssetFieldConstants.Comment);
            SupportedOperators = new HashSet<Operator> { Operator.LIKE, Operator.NOT_LIKE };
        }

        public ClauseNames JqlClauseNames { get; private set; }

        public string IndexField { get { return DocumentConstants.CommentId; } }
        public string FieldId { get { return AssetFieldConstants.Comment; } }
        public HashSet<Operator> SupportedOperators { get; private set; }
        public IQuoteFlowDataType DataType { get; private set; }

        internal static CommentsFieldSearchConstants Instance
        {
            get { return instance; }
        }
    }
}