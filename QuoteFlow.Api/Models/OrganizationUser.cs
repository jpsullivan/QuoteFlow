namespace QuoteFlow.Api.Models
{
    public class OrganizationUser
    {
        public int Id { get; set; }
        public int OrganizationId { get; set; }
        public int UserId { get; set; }
    }
}