using System.Collections.Generic;
using QuoteFlow.Api;
using QuoteFlow.Api.Asset.Search;
using QuoteFlow.Api.Jql;
using QuoteFlow.Api.Jql.Query;

namespace QuoteFlow.Core.Tests.Jql
{
    public class MockClauseInformation : IClauseInformation
    {
        public ClauseNames JqlClauseNames { get; private set; }
        public string IndexField { get; private set; }
        public string FieldId { get; private set; }
        public HashSet<Operator> SupportedOperators { get; private set; }
        public IQuoteFlowDataType DataType { get; private set; }

        public MockClauseInformation(ClauseNames clauseNames)
        {
            JqlClauseNames = clauseNames;
            IndexField = null;
            FieldId = null;
            SupportedOperators = new HashSet<Operator>();
            DataType = QuoteFlowDataTypes.All;
        }

        public MockClauseInformation(ClauseNames clauseNames, string indexField, string fieldId)
        {
            JqlClauseNames = clauseNames;
            IndexField = indexField;
            FieldId = fieldId;
            SupportedOperators = new HashSet<Operator>();
            DataType = QuoteFlowDataTypes.All;
        }

        protected bool Equals(MockClauseInformation other)
        {
            return Equals(JqlClauseNames, other.JqlClauseNames) && string.Equals(IndexField, other.IndexField) &&
                   string.Equals(FieldId, other.FieldId) && Equals(SupportedOperators, other.SupportedOperators) &&
                   Equals(DataType, other.DataType);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((MockClauseInformation) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (JqlClauseNames != null ? JqlClauseNames.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (IndexField != null ? IndexField.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (FieldId != null ? FieldId.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (SupportedOperators != null ? SupportedOperators.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (DataType != null ? DataType.GetHashCode() : 0);
                return hashCode;
            }
        }
    }
}