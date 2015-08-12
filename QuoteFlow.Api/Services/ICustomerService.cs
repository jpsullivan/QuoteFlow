using System.Collections.Generic;
using QuoteFlow.Api.Models;

namespace QuoteFlow.Api.Services
{
    public interface ICustomerService
    {
        /// <summary>
        /// Retrieves a <see cref="Customer"/> by its indentifier.
        /// </summary>
        /// <param name="contactId">The <see cref="Customer"/> indentifier</param>
        /// <returns></returns>
        Customer GetCustomer(int contactId);

        /// <summary>
        /// Returns a list of customers that are assigned to a particular organization.
        /// </summary>
        /// <param name="organizationId">The organization Id to search for</param>
        /// <returns>List of customers</returns>
        IEnumerable<Customer> GetCustomersFromOrganization(int organizationId);

        /// <summary>
        /// Inserts a new customer into the database.
        /// </summary>
        /// <param name="customer" type="Contact">The customer to save</param>
        /// <returns>The inserted customer</returns>
        Customer CreateCustomer(Customer customer);

        /// <summary>
        /// Check if a customer exists within an organization that matches the information passed in.
        /// </summary>
        /// <param name="firstName">The customers' first name</param>
        /// <param name="lastName">The customers' last name</param>
        /// <param name="email">The email address of the customer</param>
        /// <param name="organizationId">The organization id that the customer belongs to</param>
        /// <returns type="Boolean"></returns>
        bool CustomerExists(string firstName, string lastName, string email, int organizationId);
    }
}