using System.Collections.Generic;
using QuoteFlow.Api.Jql;
using QuoteFlow.Api.Jql.Query;

namespace QuoteFlow.Api.Asset.Search.Constants
{
    /// <summary>
    /// All Text is strange because it does not have a fieldId or an index id.
    /// </summary>
    public class AllTextSearchConstants : IClauseInformation
    {
        public static readonly AllTextSearchConstants Instance = new AllTextSearchConstants();
        private readonly ClauseNames _allText;

        private AllTextSearchConstants()
        {
            _allText = new ClauseNames("text");
            SupportedOperators = new HashSet<Operator> { Operator.LIKE };
        }


        public ClauseNames JqlClauseNames { get { return _allText; } }

        // This makes this implementation strange since it has no associated index field, instead it indicates searching across 
        public string IndexField { get { return null; } }

        // This makes this implementation strance since it has no assciated field
        public string FieldId { get { return null; } }

        public HashSet<Operator> SupportedOperators { get; private set; }
        public IQuoteFlowDataType DataType { get; private set; }
    }
}