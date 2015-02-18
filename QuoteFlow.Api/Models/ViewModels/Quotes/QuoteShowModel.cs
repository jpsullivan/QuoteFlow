namespace QuoteFlow.Api.Models.ViewModels.Quotes
{
    public class QuoteShowModel
    {
        public Quote Quote { get; set; }
        public User QuoteCreator { get; set; }
    }
}