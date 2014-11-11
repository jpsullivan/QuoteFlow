using System;
using System.Linq;
using QuoteFlow.Models.Assets.Search.Constants;
using QuoteFlow.Models.Assets.Search.Managers;
using QuoteFlow.Models.Search.Jql.Util;

namespace QuoteFlow.Models.Assets.CustomFields.Searchers.Transformer
{
    /// <summary>
    /// Default implementation of <see cref="ICustomFieldInputHelper"/>
    /// </summary>
    public class CustomFieldInputHelper : ICustomFieldInputHelper
    {
        private readonly SearchHandlerManager searchHandlerManager;

        public CustomFieldInputHelper(SearchHandlerManager searchHandlerManager)
        {
            if (searchHandlerManager == null)
            {
                throw new ArgumentNullException("searchHandlerManager");
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