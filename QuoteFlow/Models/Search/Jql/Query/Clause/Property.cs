using System;
using System.Collections.Generic;

namespace QuoteFlow.Models.Search.Jql.Query.Clause
{
    /// <summary>
    /// Encapsulates the entity property key and object reference data.
    /// </summary>
    public class Property
    {
        /// <summary>
        /// The key of the entity property.
        /// </summary>
        public List<string> Keys { get; set; }

        /// <summary>
        /// The path to the searched json value.
        /// </summary>
        public List<string> ObjectReferences { get; set; } 

        public Property(List<string> keys, List<string> objectReferences)
        {
            Keys = keys;
            ObjectReferences = objectReferences;
        }

        public override string ToString()
        {
            return String.Format("[%s].%s", Keys, ObjectReferences);
        }
    }
}