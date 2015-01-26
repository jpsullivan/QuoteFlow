using System;
using System.Collections.Generic;
using System.Linq;

namespace QuoteFlow.Api.Asset.Search
{
    public class SearchResult
    {
        public int Hits { get; private set; }
        public DateTime? IndexTimestampUtc { get; private set; }
        public IEnumerable<Models.Asset> Data { get; private set; }

        public SearchResult(int hits, DateTime? indexTimestampUtc)
            : this(hits, indexTimestampUtc, Enumerable.Empty<Models.Asset>().AsEnumerable())
        {
        }

        public SearchResult(int hits, DateTime? indexTimestampUtc, IEnumerable<Models.Asset> data)
        {
            Hits = hits;
            Data = data;
            IndexTimestampUtc = indexTimestampUtc;
        }
    }
}