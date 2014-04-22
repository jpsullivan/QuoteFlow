using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuoteFlow.Models
{
    public class OrganizationUser
    {
        public int Id { get; set; }
        public int OrganizationId { get; set; }
        public int UserId { get; set; }
    }
}