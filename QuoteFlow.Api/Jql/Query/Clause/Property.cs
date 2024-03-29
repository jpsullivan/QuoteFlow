﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace QuoteFlow.Api.Jql.Query.Clause
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

        public Property(IEnumerable<string> keys, IEnumerable<string> objectReferences)
        {
            Keys = keys.ToList();
            ObjectReferences = objectReferences.ToList();
        }

        public override string ToString()
        {
            return String.Format("[{0}].{1}", KeysAsString(), ObjectReferencesAsString());
        }

        public string KeysAsString()
        {
            return String.Join(".", Keys);
        }

        public string ObjectReferencesAsString()
        {
            return String.Join(".", ObjectReferences);
        }
    }
}