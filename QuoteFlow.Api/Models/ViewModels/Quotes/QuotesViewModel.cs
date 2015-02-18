using System.Collections.Generic;

namespace QuoteFlow.Api.Models.ViewModels.Quotes
{
    public class QuotesViewModel
    {
        public QuotesViewModel(IEnumerable<Quote> quotes)
        {
            Quotes = quotes;
        }

        public IEnumerable<Quote> Quotes { get; set; } 
    }
}
