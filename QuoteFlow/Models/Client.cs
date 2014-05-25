using System;
using System.Collections.Generic;

namespace QuoteFlow.Models
{
    public class Client
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int OrganizationId { get; set; }
        public DateTime LastUpdated { get; set; }
        public DateTime CreationDate { get; set; }

        public IEnumerable<ClientCatalog> Catalogs { get; set; }
        public IEnumerable<ClientUser> User { get; set; }
    }
}
