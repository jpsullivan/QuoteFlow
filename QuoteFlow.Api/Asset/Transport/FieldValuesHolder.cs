using System;
using System.Collections.Generic;

namespace QuoteFlow.Api.Asset.Transport
{
    [Serializable]
    public class FieldValuesHolder : Dictionary<string, object>, IFieldValuesHolder
    {
        public FieldValuesHolder()
		{
		}

		public FieldValuesHolder(int initialCapacity) : base(initialCapacity)
		{
		}
    }
}