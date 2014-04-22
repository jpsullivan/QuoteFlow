using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuoteFlow.Models
{
    public class Contact
    {
        public int Id { get; set; }
        public int OrganizationId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Organization { get; set; }
        public string Title { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string Zipcode { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime LastUpdated { get; set; }

        public string FullName
        {
            get { return string.Format("{0} {1}", FirstName, LastName); }
        }
    }
}