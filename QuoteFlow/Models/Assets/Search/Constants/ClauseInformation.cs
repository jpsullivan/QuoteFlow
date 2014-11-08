using QuoteFlow.Infrastructure;
using QuoteFlow.Models.Search.Jql.Clauses;
using QuoteFlow.Models.Search.Jql.Query;
using Wintellect.PowerCollections;

namespace QuoteFlow.Models.Assets.Search.Constants
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