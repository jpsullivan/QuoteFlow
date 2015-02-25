using QuoteFlow.Api.Infrastructure.Helpers;

namespace QuoteFlow.Api.Models.ViewModels.Quotes
{
    public class QuoteLineItemsViewModel
    {
        public QuoteLineItemsViewModel(Quote quote, User quoteCreator, PagedList<QuoteLineItem> lineItems, int currentPage, string paginationUrl)
        {
            Quote = quote;
            QuoteCreator = quoteCreator;
            LineItems = lineItems;
            CurrentPage = currentPage;
            PaginationUrl = paginationUrl;
        }

        public Quote Quote { get; set; }
        public User QuoteCreator { get; set; }

        public PagedList<QuoteLineItem> LineItems { get; set; }
        public int CurrentPage { get; set; }
        public string PaginationUrl { get; set; }
    }
}
