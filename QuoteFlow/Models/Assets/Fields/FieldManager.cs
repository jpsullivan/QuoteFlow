using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using QuoteFlow.Models.Search.Jql.Context;

namespace QuoteFlow.Models.Assets.Fields
{
    public class FieldManager : IFieldManager
    {
        private readonly IDictionary<string, IField> _fields = new Dictionary<string, IField>(); 
        private readonly ICollection<IOrderableField> _orderableFields = new Collection<IOrderableField>();
        private readonly List<ISearchableField> _searchableFields = new List<ISearchableField>(); 

        public FieldManager(CatalogSystemField catalogSystemField)
        {
            _fields.Add(catalogSystemField.Id, catalogSystemField);

            // special case: CatalogSystemField is not orderable, even though it implements IOrderableField
            foreach (var field in _fields)
            {
                _searchableFields.Add((ISearchableField) field.Value);
            }
        }

        public IField GetField(string id)
        {
            return IsCustomField(id) ? GetCustomField(id) : _fields[id];
        }

        public bool IsCustomField(string id)
        {
            throw new NotImplementedException();
        }

        public bool IsCustomField(IField field)
        {
            throw new NotImplementedException();
        }

        public bool IsNavigableField(string id)
        {
            return IsCustomField(id) || IsNavigableField(_fields[id]);
        }

        public bool IsNavigableField(IField field)
        {
            // CustomField implements INavigableField, so checking for it would be redundant
            return field is INavigableField;
        }

        public INavigableField GetNavigableField(string id)
        {
            if (IsCustomField(id))
            {
                return GetCustomField(id);
            }

            var field = _fields[id];
            if (field is INavigableField)
            {
                return (INavigableField) field;
            }

            throw new Exception("The field with id '" + id + "' is not a NavigableField.");
        }

        public ICustomField GetCustomField(string id)
        {
            throw new NotImplementedException();
        }

        public ISet<IOrderableField> OrderableFields { get; private set; }
        public ISet<INavigableField> NavigableFields { get; private set; }
        public ISet<IField> UnavailableFields { get; private set; }

        public void Refresh()
        {
            throw new NotImplementedException();
        }
        
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

        public IEnumerable<ISearchableField> SystemSearchableFields
        {
            get { return new HashSet<ISearchableField>(_searchableFields); }
            set { throw new NotImplementedException(); }
        }
        
        public IAssetTypeField IssueTypeField { get; private set; }
        public ICatalogField CatalogField { get; private set; }
    }
}