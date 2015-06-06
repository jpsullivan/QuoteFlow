using System.Collections.Generic;
using QuoteFlow.Api.Asset.Index;
using QuoteFlow.Api.Jql.Query;

namespace QuoteFlow.Api.Asset.Search.Constants
{
    /// <summary>
    /// Holds searching constants for user system fields.
    /// </summary>
    public sealed class UserFieldSearchConstantsWithEmpty : UserFieldSearchConstants
    {
        public string EmptySelectFlag { get; set; }
        public string EmptyIndexValue { get; set; }

        public UserFieldSearchConstantsWithEmpty(string indexField, ClauseNames names, string fieldUrlParameter, string selectUrlParameter, string searcherId, string emptySelectFlag, string fieldId, string currentUserSelectFlag, string specificUserSelectFlag, string specificGroupSelectFlag, string emptyIndexValue, HashSet<Operator> supportedOperators)
            : base(indexField, names, fieldUrlParameter, selectUrlParameter, searcherId, fieldId, currentUserSelectFlag, specificUserSelectFlag, specificGroupSelectFlag, supportedOperators)
        {
            EmptySelectFlag = emptySelectFlag;
            EmptyIndexValue = emptyIndexValue;
        }

        public UserFieldSearchConstantsWithEmpty(string indexField, string jqlClauseName, string fieldUrlParameter, string selectUrlParameter, string searcherId, string emptySelectFlag, string fieldId, HashSet<Operator> supportedOperators)
            : this(indexField, new ClauseNames(jqlClauseName), fieldUrlParameter, selectUrlParameter, searcherId, emptySelectFlag, fieldId, DocumentConstants.AssetCurrentUser, DocumentConstants.SpecificUser, DocumentConstants.SpecificGroup, emptySelectFlag, supportedOperators)
        {
        }
    }
}