using QuoteFlow.Api.Asset.Index;
using QuoteFlow.Api.Jql;
using QuoteFlow.Api.Jql.Operator;
using QuoteFlow.Api.Jql.Query;
using Wintellect.PowerCollections;

namespace QuoteFlow.Api.Asset.Search.Constants
{
    /// <summary>
    /// Searching constants for the "AssetKey" JQL clause.
    /// </summary>
    public class AssetIdConstants : IClauseInformation
    {
        public ClauseNames JqlClauseNames { get; private set; }
        public string IndexField { get; private set; }
        public string FieldId { get; private set; }
        public Set<Operator> SupportedOperators { get; private set; }
        public IQuoteFlowDataType DataType { get; private set; }

        private static readonly AssetIdConstants instance = new AssetIdConstants();

        private AssetIdConstants()
		{
			JqlClauseNames = new ClauseNames("id", "issue", "issuekey");
			FieldId = DocumentConstants.AssetId;
			IndexField = DocumentConstants.AssetId;
			SupportedOperators = OperatorClasses.EqualityAndRelational;
		}

        public static AssetIdConstants Instance
        {
            get { return instance; }
        }
    }
}