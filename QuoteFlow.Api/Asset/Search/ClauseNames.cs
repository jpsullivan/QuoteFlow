using System;
using System.Collections.Generic;
using System.Linq;
using QuoteFlow.Api.Infrastructure.Extensions;

namespace QuoteFlow.Api.Asset.Search
{
    /// <summary>
    /// Represents a JQL clause name. The primary name is the name which uniquely identifies the clause, 
    /// the names is a set of alternate names that the clause can be known as.
    /// </summary>
    public sealed class ClauseNames
    {
        public List<string> JqlFieldNames { get; set; }
        public string PrimaryName { get; set; }

        public ClauseNames(string primaryName)
            : this(primaryName, Enumerable.Empty<string>())
        {
        }

        public ClauseNames(string primaryName, params string[] names)
            : this(primaryName, new List<string>(names))
        {
        }

        public ClauseNames(string primaryName, IEnumerable<string> names)
        {
            var newNames = new SortedSet<string>(StringComparer.OrdinalIgnoreCase);

            // ensure that the primary name isn't empty
            if (primaryName.IsNullOrWhiteSpace())
            {
                throw new ArgumentNullException(nameof(primaryName));
            }

            // ensure that names doesn't contain any empty elements
            if (names.Any(name => name.IsNullOrWhiteSpace()))
            {
                throw new ArgumentNullException(nameof(names));
            }

            newNames.UnionWith(names);
            PrimaryName = primaryName;
            // Always make sure the names contains the primary name as well
            newNames.Add(PrimaryName);
            JqlFieldNames = new List<string>(newNames);
        }

        public bool Contains(string name)
        {
            return JqlFieldNames.Contains(name);
        }

        private bool Equals(ClauseNames other)
        {
            return JqlFieldNames.SequenceEqual(other.JqlFieldNames) && string.Equals(PrimaryName, other.PrimaryName);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj is ClauseNames && Equals((ClauseNames) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((JqlFieldNames != null ? JqlFieldNames.GetHashCode() : 0)*397) ^ (PrimaryName != null ? PrimaryName.GetHashCode() : 0);
            }
        }
    }
}