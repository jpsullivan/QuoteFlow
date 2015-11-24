namespace QuoteFlow.Api.Models.OrderBy
{
    /// <summary>
    /// A single option that the user can click to sort.
    /// </summary>
    public class OrderByOption
    {
        public string FieldId { get; set; }
        public string FieldName { get; set; }
        public string SortJql { get; set; }
    }
}