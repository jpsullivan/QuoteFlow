using System;

namespace QuoteFlow.Api.Models
{
    public class QuoteStatus
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int OrganizationId { get; set; }
        public int OrderNum { get; set; }
        public DateTime CreatedUtc { get; set; }
    }
}
