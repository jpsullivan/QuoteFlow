using System.Collections.Generic;
using Lucene.Net.Documents;

namespace QuoteFlow.Api.Index
{
    /// <summary>
    /// Provides ability to add fields to Document during indexing.
    /// </summary>
    public interface IEntitySearchExtractor<T>
    {
        ISet<string> IndexEntity(IEntitySearchExtractorContext<T> context, Document doc);
    }

    public interface IEntitySearchExtractorContext<T>
    {
        T Entity { get; }

        /// <summary>
		/// Currently for one of assets, comments, changes See constant in <see cref="SearchProviderFactory"/> 
		/// for available indexes.
		/// </summary>
		/// <returns>Returns the index name.</returns>
        string IndexName { get; }
    }
}