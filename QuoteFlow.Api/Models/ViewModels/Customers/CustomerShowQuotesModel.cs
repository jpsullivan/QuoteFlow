using System.Collections.Generic;

namespace QuoteFlow.Api.Models.ViewModels.Customers
{
    public class CustomerShowQuotesModel
    {
        public CustomerShowQuotesModel(Customer customer, IEnumerable<Quote> quotes, IEnumerable<QuoteStatus> statuses)
        {
            Customer = customer;
            Quotes = quotes;
            Statuses = statuses;
        }

        /// <summary>
        /// The customer themselves.
        /// </summary>
        public Customer Customer { get; set; }

        /// <summary>
        /// All of the customers' quotes.
        /// </summary>
        public IEnumerable<Quote> Quotes { get; set; }

        /// <summary>
        /// All of the quote statuses in the database. Used for 
        /// status lookups when iterating over the quotes for UI rendering.
        /// </summary>
        public IEnumerable<QuoteStatus> Statuses { get; set; } 
    }
}