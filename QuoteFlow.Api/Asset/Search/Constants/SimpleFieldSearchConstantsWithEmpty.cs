using System.Collections.Generic;
using QuoteFlow.Api.Jql;
using QuoteFlow.Api.Jql.Query;

namespace QuoteFlow.Api.Asset.Search.Constants
{
    /// <summary>
    /// Holds searching constants for simple system fields.
    /// </summary>
    public sealed class SimpleFieldSearchConstantsWithEmpty : IClauseInformation
    {
        public string IndexField { get; set; }
        public ClauseNames JqlClauseNames { get; private set; }
        public string UrlParameter { get; private set; }
        public string SearcherId { get; private set; }
        public HashSet<Operator> SupportedOperators { get; private set; }
        public string EmptySelectFlag { get; private set; }
        public string EmptyIndexValue { get; private set; }
        public string FieldId { get; private set; }
        public IQuoteFlowDataType DataType { get; private set; }

        public SimpleFieldSearchConstantsWithEmpty(string field, HashSet<Operator> supportedOperators, IQuoteFlowDataType supportedType)
            : this(field, field, field, field, field, field, field, supportedOperators, supportedType)
        {
        }

        public SimpleFieldSearchConstantsWithEmpty(string indexField, ClauseNames jqlClauseNames, string urlParameter, string searcherId, HashSet<Operator> supportedOperators, string emptySelectFlag, string emptyIndexValue, string fieldId, IQuoteFlowDataType supportedType)
        {
            IndexField = indexField;
            JqlClauseNames = jqlClauseNames;
            UrlParameter = urlParameter;
            SearcherId = searcherId;
            SupportedOperators = supportedOperators;
            EmptySelectFlag = emptySelectFlag;
            EmptyIndexValue = emptyIndexValue;
            FieldId = fieldId;
            DataType = supportedType;
        }

        public SimpleFieldSearchConstantsWithEmpty(string indexField, string jqlClauseName, string urlParameter, string searcherId, string emptySelectFlag, string emptyIndexValue, string fieldId, HashSet<Operator> supportedOperators, IQuoteFlowDataType supportedType)
            : this(indexField, new ClauseNames(jqlClauseName), urlParameter, searcherId, supportedOperators, emptySelectFlag, emptyIndexValue, fieldId, supportedType)
        {
        }
    }
}