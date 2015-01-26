using System;
using System.Collections.Generic;
using System.Linq;
using Wintellect.PowerCollections;

namespace QuoteFlow.Api.Asset.Search
{
    /// <summary>
    /// Represents a JQL clause name. The primary name is the name which uniquely identifies the clause, 
    /// the names is a set of alternate names that the clause can be known as.
    /// </summary>
    public sealed class ClauseNames
    {
        public Set<string> JqlFieldNames { get; set; }
        public string PrimaryName { get; set; }

        public ClauseNames(string primaryName)
            : this(primaryName, Enumerable.Empty<string>())
        {
        }

        public ClauseNames(string primaryName, params string[] names)
            : this(primaryName, new HashSet<string>(names))
        {
        }

        public ClauseNames(string primaryName, IEnumerable<string> names)
        {
            var newNames = new SortedSet<string>(StringComparer.OrdinalIgnoreCase);
            newNames.UnionWith(names);
            PrimaryName = primaryName;
            // Always make sure the names contains the primary name as well
            newNames.Add(PrimaryName);
            JqlFieldNames = new Set<string>(newNames);
        }

        public bool Contains(string name)
        {
            return JqlFieldNames.Contains(name);
        }
    }
}