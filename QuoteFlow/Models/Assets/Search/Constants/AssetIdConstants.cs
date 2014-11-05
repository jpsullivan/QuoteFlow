using QuoteFlow.Infrastructure;
using QuoteFlow.Models.Assets.Index;
using QuoteFlow.Models.Search.Jql;
using QuoteFlow.Models.Search.Jql.Clauses;
using QuoteFlow.Models.Search.Jql.Query;
using Wintellect.PowerCollections;

namespace QuoteFlow.Models.Assets.Search.Constants
{
    /// <summary>
    /// Searching constants for the "AssetKey" JQL clause.
    /// </summary>
    public class AssetIdConstants : IClauseInformation
    {
        private static readonly AssetIdConstants instance = new AssetIdConstants();

        private AssetIdConstants()
		{
			JqlClauseNames = new ClauseNames("key", "issue", "issuekey", "id");
			FieldId = DocumentConstants.AssetId;
			IndexField = DocumentConstants.AssetId;
			SupportedOperators = OperatorClasses.EqualityAndRelational;
		}

        public ClauseNames JqlClauseNames { get; private set; }
        public string IndexField { get; private set; }
        public string FieldId { get; private set; }
        public Set<Operator> SupportedOperators { get; private set; }
        public IQuoteFlowDataType DataType { get; private set; }

        public static AssetIdConstants Instance
        {
            get { return instance; }
        }
    }
}