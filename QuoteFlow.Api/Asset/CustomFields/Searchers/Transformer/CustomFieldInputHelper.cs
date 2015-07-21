using System;
using System.Linq;
using QuoteFlow.Api.Asset.Search.Constants;
using QuoteFlow.Api.Asset.Search.Managers;
using QuoteFlow.Api.Jql.Util;
using QuoteFlow.Api.Models;

namespace QuoteFlow.Api.Asset.CustomFields.Searchers.Transformer
{
    /// <summary>
    /// Default implementation of <see cref="ICustomFieldInputHelper"/>
    /// </summary>
    public class CustomFieldInputHelper : ICustomFieldInputHelper
    {
        private readonly ISearchHandlerManager searchHandlerManager;

        public CustomFieldInputHelper(ISearchHandlerManager searchHandlerManager)
        {
            if (searchHandlerManager == null)
            {
                throw new ArgumentNullException(nameof(searchHandlerManager));
            }

            this.searchHandlerManager = searchHandlerManager;
        }

        public virtual string GetUniqueClauseName(User user, string primaryName, string fieldName)
        {
            // we must check that the name of the field is not something that would cause it to not be registered in the
            // SearchHandlerManager, for this would mean that the name is potentially not unique
            if (SystemSearchConstants.IsSystemName(fieldName)) return primaryName;
            if (JqlCustomFieldId.IsJqlCustomFieldId(fieldName)) return primaryName;

            if (searchHandlerManager.GetClauseHandler(user, fieldName).Count() == 1)
            {
                return fieldName;
            }

            return primaryName;
        }
    }
}