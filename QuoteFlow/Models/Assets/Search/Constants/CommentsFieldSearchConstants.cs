﻿using QuoteFlow.Infrastructure;
using QuoteFlow.Models.Assets.Index;
using QuoteFlow.Models.Search.Jql.Clauses;
using QuoteFlow.Models.Search.Jql.Query;
using Wintellect.PowerCollections;

namespace QuoteFlow.Models.Assets.Search.Constants
{
    public class CommentsFieldSearchConstants : IClauseInformation
    {
        private static readonly CommentsFieldSearchConstants instance = new CommentsFieldSearchConstants();

        private CommentsFieldSearchConstants()
        {
            JqlClauseNames = new ClauseNames(AssetFieldConstants.Comment);
            SupportedOperators = new Set<Operator> { Operator.LIKE, Operator.NOT_LIKE};
        }

        public ClauseNames JqlClauseNames { get; private set; }

        public string IndexField { get { return DocumentConstants.CommentId; } }
        public string FieldId { get { return AssetFieldConstants.Comment; } }
        public Set<Operator> SupportedOperators { get; private set; }
        public IQuoteFlowDataType DataType { get; private set; }

        internal static CommentsFieldSearchConstants Instance
        {
            get { return instance; }
        }
    }
}