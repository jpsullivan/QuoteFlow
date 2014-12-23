using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuoteFlow.Api.Index
{
    /// <summary>
    /// Used to build a <seealso cref="Result"/> implementation that accumulates results from
    /// other operations and awaits on them all.
    /// 
    /// For operations that are complete it just aggregates their results.
    /// </summary>
    public sealed class AccumulatingResultBuilder
    {
        //private static readonly RateLimitingLogger log = new RateLimitingLogger(typeof(AccumulatingResultBuilder));
        private readonly ICollection<InFlightResult> inFlightResults = new LinkedBlockingQueue<InFlightResult>();
        private int successesToDate = 0;
        private int failuresToDate = 0;

        private class InFlightResult
        {
            internal readonly string indexName;
            internal readonly long? identifier;
            internal readonly Result result;

            internal InFlightResult(string indexName, long? identifier, Result result)
            {
                this.indexName = indexName;
                this.identifier = identifier;
                this.result = result;
            }

            internal virtual string IndexName
            {
                get { return indexName; }
            }

            internal virtual long? Identifier
            {
                get { return identifier; }
            }

            internal virtual Result Result
            {
                get { return result; }
            }
        }
    }
}
