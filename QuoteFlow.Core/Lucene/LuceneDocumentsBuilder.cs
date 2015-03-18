using System.Collections.Generic;
using System.Collections.Immutable;
using Lucene.Net.Documents;

namespace QuoteFlow.Core.Lucene
{
    public class LuceneDocumentsBuilder
    {
        private static readonly ImmutableList<Document>.Builder Builder = ImmutableList.CreateBuilder<Document>();

        public static IEnumerable<Document> BuildAll(IEnumerable<Document> documents)
        {
            foreach (var document in documents)
            {
                Builder.Add(document);
            }

            return Builder;
        }
    }
}