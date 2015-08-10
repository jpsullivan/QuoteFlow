using System.Collections.Generic;
using QuoteFlow.Api.Models;

namespace QuoteFlow.Api.Services
{
    public interface IContactService
    {
        /// <summary>
        /// Retrieves a <see cref="Contact"/> by its indentifier.
        /// </summary>
        /// <param name="contactId">the <see cref="Contact"/> indentifier</param>
        /// <returns></returns>
        Contact GetContact(int contactId);

        /// <summary>
        /// Returns a list of customers that are assigned to a particular organization
        /// </summary>
        /// <param name="organizationId">The organization Id to search for</param>
        /// <returns>List of Contacts</returns>
        IEnumerable<Contact> GetContactsFromOrganization(int organizationId);

        /// <summary>
        /// Inserts a new contact into the database
        /// </summary>
        /// <param name="contact" type="Contact">The contact to save</param>
        /// <returns>The inserted contact</returns>
        Contact CreateContact(Contact contact);

        /// <summary>
        /// Check if a contact exists within an organization that matches the information passed in.
        /// </summary>
        /// <param name="firstName">The contacts' first name</param>
        /// <param name="lastName">The contacts' last name</param>
        /// <param name="email">The email address of the contact</param>
        /// <param name="organizationId">The organization id that the contact belongs to</param>
        /// <returns type="Boolean"></returns>
        bool ContactExists(string firstName, string lastName, string email, int organizationId);
    }
}