using System.Collections.Generic;

namespace QuoteFlow.Api.Models.ViewModels.Customers
{
    public class CustomerShowQuotesModel
    {
        public CustomerShowQuotesModel(Customer customer, IEnumerable<Quote> quotes)
        {
            Customer = customer;
            Quotes = quotes;
        }

        /// <summary>
        /// The customer themselves.
        /// </summary>
        public Customer Customer { get; set; }

        /// <summary>
        /// All of the customers' quotes.
        /// </summary>
        public IEnumerable<Quote> Quotes { get; set; }
    }
}