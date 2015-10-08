using System.Collections.Generic;

namespace QuoteFlow.Api.Models.ViewModels.Customers
{
    public class CustomersViewModel
    {
        public CustomersViewModel(IEnumerable<Customer> customers)
        {
            Customers = customers;
        }

        /// <summary>
        /// The collection of customers to display on the page.
        /// </summary>
        public IEnumerable<Customer> Customers { get; set; }
    }
}