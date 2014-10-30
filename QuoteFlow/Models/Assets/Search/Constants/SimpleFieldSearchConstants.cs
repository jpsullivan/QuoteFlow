﻿using System.Collections.Generic;
using QuoteFlow.Infrastructure;
using QuoteFlow.Models.Search.Jql.Clauses;
using QuoteFlow.Models.Search.Jql.Query;
using Wintellect.PowerCollections;

namespace QuoteFlow.Models.Assets.Search.Constants
{
    /// <summary>
    /// Holds searching constants for simple system fields.
    /// </summary>
    public sealed class SimpleFieldSearchConstants : IClauseInformation
    {
        public string IndexField { get; set; }
        public ClauseNames JqlClauseNames { get; set; }
        public string UrlParameter { get; set; }
        public string SearcherId { get; private set; }
        public string FieldId { get; private set; }
        public Set<Operator> SupportedOperators { get; private set; }
        public IQuoteFlowDataType DataType { get; private set; }

        public SimpleFieldSearchConstants(string field, IEnumerable<Operator> supportedOperators, IQuoteFlowDataType supportedType)
            : this(field, field, field, field, field, supportedOperators, supportedType)
        {
        }

        public SimpleFieldSearchConstants(string indexField, ClauseNames jqlClauseNames, string urlParameter, string searcherId, string fieldId, IEnumerable<Operator> supportedOperators, IQuoteFlowDataType supportedType)
        {
            IndexField = indexField;
            JqlClauseNames = jqlClauseNames;
            UrlParameter = urlParameter;
            SearcherId = searcherId;
            FieldId = fieldId;
            SupportedOperators = (Set<Operator>) supportedOperators;
            DataType = supportedType;
        }

        public SimpleFieldSearchConstants(string indexField, string jqlClauseName, string urlParameter, string searcherId, string fieldId, IEnumerable<Operator> supportedOperators, IQuoteFlowDataType supportedType)
            : this(indexField, new ClauseNames(jqlClauseName), urlParameter, searcherId, fieldId, supportedOperators, supportedType)
        {
        }
    }
}