using System;
using Jil;

namespace QuoteFlow.Api.Jql.Query
{
    [Serializable]
    public class Searcher
    {
        public Searcher()
        {
        }

        public Searcher(string id, string name, string key, bool isShown, long lastViewed)
        {
            Id = id;
            Name = name;
            Key = key;
            IsShown = isShown;
            LastViewed = lastViewed;
        }

        [JilDirective("id")]
        public string Id { get; set; }

        [JilDirective("name")]
        public string Name { get; set; }

        [JilDirective("key")]
        public string Key { get; set; }

        [JilDirective("isShown")]
        public bool IsShown { get; set; }

        [JilDirective("lastViewed")]
        public long LastViewed { get; set; }
    }
}