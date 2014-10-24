using System;
using System.Collections.Generic;
using System.Linq;

namespace QuoteFlow.Models.Assets.Search
{
    public class SearchResult
    {
        public int Hits { get; private set; }
        public DateTime? IndexTimestampUtc { get; private set; }
        public IEnumerable<Asset> Data { get; private set; }

        public SearchResult(int hits, DateTime? indexTimestampUtc)
            : this(hits, indexTimestampUtc, Enumerable.Empty<Asset>().AsEnumerable())
        {
        }

        public SearchResult(int hits, DateTime? indexTimestampUtc, IEnumerable<Asset> data)
        {
            Hits = hits;
            Data = data;
            IndexTimestampUtc = indexTimestampUtc;
        }
    }
}