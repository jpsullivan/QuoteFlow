using System.Collections.Generic;

namespace QuoteFlow.Api.Models.ViewModels.Contacts
{
    public class ContactsViewModel
    {
        public ContactsViewModel(IEnumerable<Contact> contacts)
        {
            Contacts = contacts;
        }

        /// <summary>
        /// The collection of contacts to display on the page.
        /// </summary>
        public IEnumerable<Contact> Contacts { get; set; }
    }
}