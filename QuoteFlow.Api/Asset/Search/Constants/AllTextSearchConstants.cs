using QuoteFlow.Api.Jql;
using QuoteFlow.Api.Jql.Query;
using Wintellect.PowerCollections;

namespace QuoteFlow.Api.Asset.Search.Constants
{
    /// <summary>
    /// All Text is strange because it does not have a fieldId or an index id.
    /// </summary>
    public class AllTextSearchConstants : IClauseInformation
    {
        public static AllTextSearchConstants Instance = new AllTextSearchConstants();

        private readonly ClauseNames ALL_TEXT;
        private readonly Set<Operator> supportedOperators;

        private AllTextSearchConstants()
        {
            ALL_TEXT = new ClauseNames("text");
            SupportedOperators = new Set<Operator> { Operator.LIKE };
        }


        public ClauseNames JqlClauseNames { get { return ALL_TEXT; } }

        // This makes this implementation strange since it has no associated index field, instead it indicates searching across 
        public string IndexField { get { return null; } }

        // This makes this implementation strance since it has no assciated field
        public string FieldId { get { return null; } }

        public Set<Operator> SupportedOperators { get; private set; }
        public IQuoteFlowDataType DataType { get; private set; }
    }
}