using QuoteFlow.Api.Jql;
using QuoteFlow.Api.Jql.Query;
using Wintellect.PowerCollections;

namespace QuoteFlow.Api.Asset.Search.Constants
{
    public class ClauseInformation : IClauseInformation
    {
        public ClauseNames JqlClauseNames { get; private set; }
        public string IndexField { get; private set; }
        public string FieldId { get; private set; }
        public Set<Operator> SupportedOperators { get; private set; }
        public IQuoteFlowDataType DataType { get; private set; }

		public ClauseInformation(string indexField, ClauseNames names, string fieldId, Set<Operator> supportedOperators, IQuoteFlowDataType supportedType)
		{
			DataType = supportedType;
			IndexField = indexField;
			JqlClauseNames = names;
			FieldId = fieldId;
			SupportedOperators = supportedOperators;
		}

        public ClauseInformation(string indexField, string jqlClauseName, string fieldId, Set<Operator> supportedOperators, IQuoteFlowDataType supportedType)
            : this(indexField, new ClauseNames(jqlClauseName), fieldId, supportedOperators, supportedType)
		{
		}
    }
}