using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using QuoteFlow.Api.Asset.Fields;
using QuoteFlow.Api.Jql.Context;
using QuoteFlow.Api.Models;

namespace QuoteFlow.Core.Asset.Fields
{
    public class FieldManager : IFieldManager
    {
        private readonly IDictionary<string, IField> _fields = new Dictionary<string, IField>(); 
        private readonly ICollection<IOrderableField> _orderableFields = new Collection<IOrderableField>();
        private readonly ICollection<INavigableField> _navigableFields = new List<INavigableField>();
        private readonly List<ISearchableField> _searchableFields = new List<ISearchableField>(); 

        public FieldManager(CatalogSystemField catalogSystemField, SummarySystemField summarySystemField,
            ManufacturerSystemField manufacturerSystemField, CreatorSystemField creatorSystemField,
            CostSystemField costSystemField)
        {
            _fields.Add(catalogSystemField.Id, catalogSystemField);
            _fields.Add(summarySystemField.Id, summarySystemField);
            _fields.Add(manufacturerSystemField.Id, manufacturerSystemField);
            _fields.Add(creatorSystemField.Id, creatorSystemField);
            _fields.Add(costSystemField.Id, costSystemField);

            // special case: CatalogSystemField is not orderable, even though it implements IOrderableField
            foreach (var field in _fields)
            {
                var orderableField = field.Value as IOrderableField;
                if (orderableField != null && field.Value.GetType() != typeof(CatalogSystemField))
                {
                    _orderableFields.Add(orderableField);
                }

                _navigableFields.Add((INavigableField) field.Value);
                _searchableFields.Add((ISearchableField) field.Value);
            }
        }

        public IField GetField(string id)
        {
            return IsCustomField(id) ? GetCustomField(id) : _fields[id];
        }

        public bool IsCustomField(string id)
        {
            return false;
        }

        public bool IsCustomField(IField field)
        {
            return false;
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

        public ISet<ICustomField> GetAvailableCustomFields(User remoteUser, Api.Models.Asset asset)
        {
            throw new NotImplementedException();
        }

        public ISet<INavigableField> AllAvailableNavigableFields()
        {
            var availableFields = new HashSet<INavigableField>(_navigableFields);
            return availableFields;
        }

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
        
        public IManufacturerField IssueTypeField { get; private set; }
        public ICatalogField CatalogField { get; private set; }
    }
}