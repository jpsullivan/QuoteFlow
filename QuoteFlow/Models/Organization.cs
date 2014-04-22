namespace QuoteFlow.Models
{
    public class Organization
    {
        public int Id { get; set; }
        public string OrganizationName { get; set; }
        public int OwnerId { get; set; }
        public int CreatorId { get; set; }
    }
}