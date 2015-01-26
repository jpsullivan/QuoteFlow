using System;
using System.Collections.Generic;
using System.Linq;
using QuoteFlow.Api.Jql.Query.Clause;

namespace QuoteFlow.Core.Jql.Util
{
    public class FieldReference
    {
        public string Name { get; set; }
        public Property Property { get; set; }

        public FieldReference(IEnumerable<string> names, IEnumerable<string> keys, IEnumerable<string> objectReferences)
        {
            Name = String.Join(".", names);
            Property = new Property(keys, objectReferences);
        }

        public bool IsEntityProperty()
        {
            return Property.Keys.Any();
        }
    }
}