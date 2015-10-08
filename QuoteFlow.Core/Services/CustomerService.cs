using System;
using System.Collections.Generic;
using System.Linq;
using QuoteFlow.Api.Models;
using QuoteFlow.Api.Services;

namespace QuoteFlow.Core.Services
{
    public class CustomerService : ICustomerService
    {
        /// <summary>
        /// Retrieves a <see cref="Customer"/> by its indentifier.
        /// </summary>
        /// <param name="contactId">the <see cref="Customer"/> indentifier</param>
        /// <returns>A single <see cref="Customer"/>.</returns>
        public Customer GetCustomer(int contactId)
        {
            return Current.DB.Customers.Get(contactId);
        }

        /// <summary>
        /// Returns a list of customers that are assigned to a particular organization.
        /// </summary>
        /// <param name="organizationId">The organization Id to search for</param>
        /// <returns>List of customers</returns>
        public IEnumerable<Customer> GetCustomersFromOrganization(int organizationId)
        {
            const string sql = "select * from Customers where OrganizationId = @organizationId";
            return Current.DB.Query<Customer>(sql, new { organizationId });
        }

        /// <summary>
        /// Inserts a new customer into the database.
        /// </summary>
        /// <param name="customer" type="Contact">The customer to save</param>
        /// <returns>The inserted customer</returns>
        public Customer CreateCustomer(Customer customer)
        {
            var parsedCustomer = new
            {
                OrganizationId = customer.OrganizationId,
                FirstName = customer.FirstName,
                LastName = customer.LastName,
                EmailAddress = customer.EmailAddress,
                Address1 = customer.Address1,
                Address2 = customer.Address2,
                Country = customer.Country,
                State = customer.State,
                City = customer.City,
                Phone = customer.Phone,
                Organization = customer.Organization,
                Title = customer.Title,
                Zipcode = customer.Zipcode,
                CreationDate = DateTime.Now,
                LastUpdated = DateTime.Now
            };

            int? insert = Current.DB.Customers.Insert(parsedCustomer);
            if (insert != null)
            {
                customer.Id = insert.Value;
            }

            return customer;
        }

        /// <summary>
        /// Check if a customer exists within an organization that matches the information passed in.
        /// </summary>
        /// <param name="firstName">The customers first name</param>
        /// <param name="lastName">The customers last name</param>
        /// <param name="email">The email address of the customer</param>
        /// <param name="organizationId">The organization id that the customer belongs to</param>
        /// <returns type="Boolean"></returns>
        public bool CustomerExists(string firstName, string lastName, string email, int organizationId)
        {
            const string sql = @"
                select * from Customers where FirstName = @firstName
                AND LastName = @lastName AND Email = @email AND OrganizationId = @organizationId";

            var customers = Current.DB.Query<Customer>(sql, new
            {
                firstName,
                lastName,
                email,
                organizationId
            });

            return customers.Any();
        }
    }
}