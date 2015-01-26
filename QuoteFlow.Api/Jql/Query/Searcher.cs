namespace QuoteFlow.Api.Jql.Query
{
    public class Searcher
    {
        public Searcher() { }

        public Searcher(string id, string name, string key, bool isShown, long lastViewed)
        {
            Id = id;
            Name = name;
            Key = key;
            IsShown = isShown;
            LastViewed = lastViewed;
        }

        public string Id { get; set; }
        public string Name { get; set; }
        public string Key { get; set; }
        public bool IsShown { get; set; }
        public long LastViewed { get; set; }
    }
}