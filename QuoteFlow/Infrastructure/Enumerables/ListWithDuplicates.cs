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

    public class ListWithDuplicates<T> : List<KeyValuePair<string, T>>
    {
        public ListWithDuplicates(ListWithDuplicates<string[]> actionParams)
        {
            throw new System.NotImplementedException();
        }

        public void Add(string key, T value)
        {
            var element = new KeyValuePair<string, T>(key, value);
            Add(element);
        }
    }
}