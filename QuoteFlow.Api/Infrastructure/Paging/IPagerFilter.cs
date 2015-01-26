namespace QuoteFlow.Api.Infrastructure.Paging
{
    public interface IPagerFilter
    {
        int Start { get; set; }

        int End { get; }

        int Max { get; set; }

        int PageSize { get; }

        int NextStart { get; }

        int PreviousStart { get; }
    }
}