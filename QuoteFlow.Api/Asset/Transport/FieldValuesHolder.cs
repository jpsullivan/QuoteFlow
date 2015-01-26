using System.Collections.Generic;

namespace QuoteFlow.Api.Asset.Transport
{
    public class FieldValuesHolder : Dictionary<string, object>, IFieldValuesHolder
    {
        public FieldValuesHolder()
		{
		}

		public FieldValuesHolder(int initialCapacity) : base(initialCapacity)
		{
		}
    }

    public class FieldValuesHolder<T> : Dictionary<string, T>, IFieldValuesHolder
    {
        public FieldValuesHolder(IDictionary<string, T> map)
        {
            PutAll(map);
        }

        private void PutAll(IEnumerable<KeyValuePair<string, T>> map)
        {
            foreach (KeyValuePair<string, T> entry in map)
			{
				this[entry.Key] = entry.Value;
			}
        }

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            throw new System.NotImplementedException();
        }

        public void Add(KeyValuePair<string, object> item)
        {
            throw new System.NotImplementedException();
        }

        public bool Contains(KeyValuePair<string, object> item)
        {
            throw new System.NotImplementedException();
        }

        public void CopyTo(KeyValuePair<string, object>[] array, int arrayIndex)
        {
            throw new System.NotImplementedException();
        }

        public bool Remove(KeyValuePair<string, object> item)
        {
            throw new System.NotImplementedException();
        }

        public bool IsReadOnly { get; private set; }
        public void Add(string key, object value)
        {
            throw new System.NotImplementedException();
        }

        public bool TryGetValue(string key, out object value)
        {
            throw new System.NotImplementedException();
        }

        public object this[string key]
        {
            get { throw new System.NotImplementedException(); }
            set { throw new System.NotImplementedException(); }
        }

        public ICollection<string> Keys { get; private set; }
        public ICollection<object> Values { get; private set; }
    }
}