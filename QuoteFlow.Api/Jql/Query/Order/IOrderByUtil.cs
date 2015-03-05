namespace QuoteFlow.Api.Jql.Query.Order
{
    public interface IOrderByUtil
    {
        SortBy GenerateSortBy(IQuery query);
    }
}