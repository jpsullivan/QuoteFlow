using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using QuoteFlow.Models.Search.Jql.Context;

namespace QuoteFlow.Models.Assets.Fields
{
    public class FieldManager : IFieldManager
    {
        // WARNING!
        // If you add a column here - some tests will fail.  Make sure you run tests before you check in!
        // The order *is* significant!
        private static readonly IEnumerable<Type> SystemFieldClasses = new List<Type>
        {
            typeof(CatalogSystemField)
        }; 

        public IField GetField(string id)
        {
            throw new NotImplementedException();
        }

        public bool IsCustomField(string id)
        {
            throw new NotImplementedException();
        }

        public bool IsCustomField(IField field)
        {
            throw new NotImplementedException();
        }

        public ICustomField GetCustomField(string id)
        {
            throw new NotImplementedException();
        }

        public ISet<IOrderableField> OrderableFields { get; private set; }
        public ISet<INavigableField> NavigableFields { get; private set; }
        public void Refresh()
        {
            throw new NotImplementedException();
        }

        public ISet<IField> UnavailableFields { get; private set; }
        public ISet<INavigableField> GetAvailableNavigableFieldsWithScope(User user)
        {
            throw new NotImplementedException();
        }

        public ISet<INavigableField> GetAvailableNavigableFieldsWithScope(User user, IQueryContext queryContext)
        {
            throw new NotImplementedException();
        }

        public ISet<ICustomField> GetAvailableCustomFields(User remoteUser, Asset asset)
        {
            throw new NotImplementedException();
        }

        public ISet<INavigableField> AllAvailableNavigableFields { get; private set; }
        public ISet<INavigableField> GetAvailableNavigableFields(User remoteUser)
        {
            throw new NotImplementedException();
        }

        public ISet<ISearchableField> AllSearchableFields { get; private set; }
        public ISet<ISearchableField> SystemSearchableFields { get; private set; }
        public IssueTypeField IssueTypeField { get; private set; }
        public CatalogField CatalogField { get; private set; }
    }
}