using System;

namespace QuoteFlow.Core.Asset.Search.Searchers.Util
{
    /// <summary>
    /// Simple helper class that generates navigator param and form names
    /// for a given date field id.
    /// </summary>
    public class CostSearcherConfig
    {
        private const string MinSuffix = ":min";
        private const string MaxSuffix = ":max";

        public string Id { get; }
        public string Min { get; set; }
        public string Max { get; set; }

        public CostSearcherConfig(string id)
        {
            if (id == null) throw new ArgumentNullException(nameof(id));

            Id = id;
            Min = Id + MinSuffix;
            Max = Id + MaxSuffix;
        }

        public override string ToString()
        {
            return $"Min: '{Min}', Max: '{Max}'";
        }

        private bool Equals(CostSearcherConfig other)
        {
            return string.Equals(Id, other.Id);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((CostSearcherConfig) obj);
        }

        public override int GetHashCode()
        {
            return Id?.GetHashCode() ?? 0;
        }
    }
}