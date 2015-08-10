using System;
using System.Collections.Generic;
using System.Linq;
using QuoteFlow.Api.Models;
using QuoteFlow.Api.Services;

namespace QuoteFlow.Core.Services
{
    public class ContactService : IContactService
    {
        /// <summary>
        /// Retrieves a <see cref="Contact"/> by its indentifier.
        /// </summary>
        /// <param name="contactId">the <see cref="Contact"/> indentifier</param>
        /// <returns>A single <see cref="Contact"/>.</returns>
        public Contact GetContact(int contactId)
        {
            return Current.DB.Contacts.Get(contactId);
        }

        /// <summary>
        /// Returns a list of customers that are assigned to a particular organization
        /// </summary>
        /// <param name="organizationId">The organization Id to search for</param>
        /// <returns>List of Contacts</returns>
        public IEnumerable<Contact> GetContactsFromOrganization(int organizationId)
        {
            const string sql = "select * from Contacts where OrganizationId = @organizationId";
            return Current.DB.Query<Contact>(sql, new { organizationId });
        }

        /// <summary>
        /// Inserts a new contact into the database
        /// </summary>
        /// <param name="contact" type="Contact">The contact to save</param>
        /// <returns>The inserted contact</returns>
        public Contact CreateContact(Contact contact)
        {
            var parsedContact = new
            {
                OrganizationId = contact.OrganizationId,
                FirstName = contact.FirstName,
                LastName = contact.LastName,
                Email = contact.Email,
                Address1 = contact.Address1,
                Address2 = contact.Address2,
                Country = contact.Country,
                State = contact.State,
                City = contact.City,
                Phone = contact.Phone,
                Organization = contact.Organization,
                Title = contact.Title,
                Zipcode = contact.Zipcode,
                CreationDate = DateTime.Now,
                LastUpdated = DateTime.Now
            };

            int? insert = Current.DB.Contacts.Insert(parsedContact);
            if (insert != null)
            {
                contact.Id = insert.Value;
            }

            return contact;
        }

        /// <summary>
        /// Check if a contact exists within an organization that matches the information passed in.
        /// </summary>
        /// <param name="firstName">The contacts' first name</param>
        /// <param name="lastName">The contacts' last name</param>
        /// <param name="email">The email address of the contact</param>
        /// <param name="organizationId">The organization id that the contact belongs to</param>
        /// <returns type="Boolean"></returns>
        public bool ContactExists(string firstName, string lastName, string email, int organizationId)
        {
            var contacts = Current.DB.Query<Contact>(@"
                select * from Contacts where FirstName = @firstName
                AND LastName = @lastName AND Email = @email AND OrganizationId = @organizationId", new
                {
                    firstName,
                    lastName,
                    email,
                    organizationId
                });
            return contacts.Any();
        }
    }
}