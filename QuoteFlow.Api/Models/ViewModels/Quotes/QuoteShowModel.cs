using System.Collections.Generic;

namespace QuoteFlow.Api.Models.ViewModels.Quotes
{
    public class QuoteShowModel
    {
        public QuoteShowModel(Quote quote, User quoteCreator)
        {
            Quote = quote;
            QuoteCreator = quoteCreator;
        }

        public QuoteShowModel(Quote quote, User quoteCreator, IEnumerable<QuoteStatus> quoteStatuses)
        {
            Quote = quote;
            QuoteCreator = quoteCreator;
            QuoteStatuses = quoteStatuses;
        }

        public Quote Quote { get; set; }
        public User QuoteCreator { get; set; }
        public IEnumerable<QuoteStatus> QuoteStatuses { get; set; } 
    }
}