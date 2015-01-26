using System;
using QuoteFlow.Api.Asset.Fields;

namespace QuoteFlow.Core.Asset.Fields
{
    public class AbstractField : IField
    {
        public string Id { get; private set; }
        public string NameKey { get { return Name; } }
        public string Name { get; private set; }

        public AbstractField(string id, string name)
        {
            Id = id;
            Name = name;
        }

        public int CompareTo(object obj)
        {
            // NOTE: If this is being chnaged, chances are 
            // the compareTo method of the CustomFieldImpl object also needs to change.
            if (obj == null)
            {
                return 1;
            }

            if (!(obj is IField))
            {
                throw new ArgumentException("Can only compare Field objects.");
            }
            
            var field = (IField) obj;
            if (Name == null)
            {
                if (field.Name == null)
                {
                    return 0;
                }
                return -1;
            }
            
            if (field.Name == null)
            {
                return 1;
            }

            return Name.CompareTo(field.Name);
        }
    }
}