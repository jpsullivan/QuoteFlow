using System.Collections.Generic;

namespace QuoteFlow.Infrastructure.Enumerables
{
    public class ListWithDuplicates : List<KeyValuePair<string, string>>
    {
        public void Add(string key, string value)
        {
            var element = new KeyValuePair<string, string>(key, value);
            Add(element);
        }
    }
}