using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using QuoteFlow.Api.Asset.Fields;

namespace QuoteFlow.Api.OrderBy
{
    /// <summary>
    /// A field list that supports filtering, sorting, topN, etc.
    /// </summary>
    public class SuggestedFields
    {
        public ImmutableList<INavigableField> Fields { get; }

        public SuggestedFields(IEnumerable<INavigableField> fields)
        {
            Fields = fields.ToImmutableList();
        }

        public int Count()
        {
            return Fields.Count;
        }

        public SuggestedFields FilterBy(Predicate<INavigableField> predicate)
        {
            return new SuggestedFields(Fields.FindAll(predicate));
        }

        /// <summary>
		/// Ranks the fields using the given comparator and returns a new FieldList.
		/// </summary>
		/// <param name="comparator"> the Comparator to use for ranking </param>
		/// <returns> the top {@code maxResults} fields </returns>
		public virtual SuggestedFields SortBy(IComparer<IField> comparator)
        {
            var ranked = new List<INavigableField>(Fields);
            ranked.Sort(comparator);
            return new SuggestedFields(ranked);
        }

        /// <summary>
        /// Returns the top {@code maxResults} fields.
        /// </summary>
        public virtual SuggestedFields SelectTop(int maxResults)
        {
            var top = Math.Min(Fields.Count, maxResults);
            return new SuggestedFields(Fields.Take(top));
        }
    }
}