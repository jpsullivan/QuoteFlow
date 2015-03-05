namespace QuoteFlow.Api.Jql.Query.Order
{
    /// <summary>
    /// Transfer object for the "sortBy" clause in the search.
    /// </summary>
    public class SortBy
    {
        public string FieldId { get; set; }
        public string FieldName { get; set; }
        public string Order { get; set; }
        public string ToggleJql { get; set; }

        public SortBy()
        {
        }

        public SortBy(string fieldId, string fieldName, string order, string toggleJql)
        {
            FieldId = fieldId;
            FieldName = fieldName;
            Order = order;
            ToggleJql = toggleJql;
        }
    }
}
