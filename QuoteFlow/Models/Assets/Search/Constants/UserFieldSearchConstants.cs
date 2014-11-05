using QuoteFlow.Infrastructure;
using QuoteFlow.Models.Assets.Index;
using QuoteFlow.Models.Search.Jql.Clauses;
using QuoteFlow.Models.Search.Jql.Query;
using Wintellect.PowerCollections;

namespace QuoteFlow.Models.Assets.Search.Constants
{
    /// <summary>
    /// Holds searching constants for user system fields.
    /// </summary>
    public class UserFieldSearchConstants : IClauseInformation
    {
        private string SearcherId { get; set; }
        private string FieldUrlParameter { get; set; }
        private string SelectUrlParameter { get; set; }
        private string CurrentUserSelectFlag { get; set; }
        private string SpecificUserSelectFlag { get; set; }
        private string SpecificGroupSelectFlag { get; set; }

        public UserFieldSearchConstants(string indexField, ClauseNames names, string fieldUrlParameter, string selectUrlParameter, string searcherId, string fieldId, string currentUserSelectFlag, string specificUserSelectFlag, string specificGroupSelectFlag, Set<Operator> supportedOperators)
        {
            FieldId = fieldId;
            CurrentUserSelectFlag = currentUserSelectFlag;
            SpecificUserSelectFlag = specificUserSelectFlag;
            SpecificGroupSelectFlag = specificGroupSelectFlag;
            FieldUrlParameter = fieldUrlParameter;
            SelectUrlParameter = selectUrlParameter;
            IndexField = indexField;
            JqlClauseNames = names;
            SearcherId = searcherId;
            SupportedOperators = supportedOperators;
        }

        public UserFieldSearchConstants(string indexField, string jqlClauseName, string fieldUrlParameter, string selectUrlParameter, string searcherId, string emptySelectFlag, string fieldId, Set<Operator> supportedOperators)
            : this(indexField, new ClauseNames(jqlClauseName), fieldUrlParameter, selectUrlParameter, searcherId, fieldId, DocumentConstants.AssetCurrentUser, DocumentConstants.SpecificUser, DocumentConstants.SpecificGroup, supportedOperators)
        {
        }


        public ClauseNames JqlClauseNames { get; private set; }
        public string IndexField { get; private set; }
        public string FieldId { get; private set; }
        public Set<Operator> SupportedOperators { get; private set; }
        public IQuoteFlowDataType DataType { get; private set; }
    }
}