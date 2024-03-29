﻿using System.Collections.Generic;
using QuoteFlow.Api.Asset.Index;
using QuoteFlow.Api.Jql;
using QuoteFlow.Api.Jql.Query;
using Wintellect.PowerCollections;

namespace QuoteFlow.Api.Asset.Search.Constants
{
    /// <summary>
    /// Holds searching constants for user system fields.
    /// </summary>
    public class UserFieldSearchConstants : IClauseInformation
    {
        public ClauseNames JqlClauseNames { get; private set; }
        public string IndexField { get; private set; }
        public string FieldId { get; private set; }
        public HashSet<Operator> SupportedOperators { get; private set; }
        public IQuoteFlowDataType DataType { get; private set; }

        public string SearcherId { get; set; }
        public string FieldUrlParameter { get; set; }
        public string SelectUrlParameter { get; set; }
        public string CurrentUserSelectFlag { get; set; }
        public string SpecificUserSelectFlag { get; set; }
        public string SpecificGroupSelectFlag { get; set; }

        public UserFieldSearchConstants(string indexField, ClauseNames names, string fieldUrlParameter, string selectUrlParameter, string searcherId, string fieldId, string currentUserSelectFlag, string specificUserSelectFlag, string specificGroupSelectFlag, HashSet<Operator> supportedOperators)
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

        public UserFieldSearchConstants(string indexField, string jqlClauseName, string fieldUrlParameter, string selectUrlParameter, string searcherId, string emptySelectFlag, string fieldId, HashSet<Operator> supportedOperators)
            : this(indexField, new ClauseNames(jqlClauseName), fieldUrlParameter, selectUrlParameter, searcherId, fieldId, DocumentConstants.AssetCurrentUser, DocumentConstants.SpecificUser, DocumentConstants.SpecificGroup, supportedOperators)
        {
        }
    }
}