using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using QuoteFlow.Api.Lucene.Index;

namespace QuoteFlow.Core.Lucene.Index
{
    /// <summary>
    /// Used to build a <seealso cref="IIndexResult"/> implementation that accumulates results from
    /// other operations and awaits on them all.
    /// 
    /// For operations that are complete it just aggregates their results.
    /// </summary>
    public sealed class AccumulatingResultBuilder
    {
        //private static readonly RateLimitingLogger log = new RateLimitingLogger(typeof(AccumulatingResultBuilder));
        private readonly IEnumerable<InFlightResult> inFlightResults = new BlockingCollection<InFlightResult>();
        private int successesToDate = 0;
        private int failuresToDate = 0;

        public AccumulatingResultBuilder Add(IIndexResult result)
        {
            if (result == null)
            {
                throw new ArgumentNullException("result");
            }

            var compResult = result as CompositeResult;
            if (compResult != null)
            {
                CompositeResult compositeResult = compResult;
                foreach (InFlightResult ifr in compositeResult.Results)
                {
                    addInternal(ifr);
                }
                successesToDate += compositeResult.Successes;
                failuresToDate += compositeResult.Failures;
            }
            else
            {
                addInternal(null, null, result);
            }
            return this;
        }

        private static void logFailure(string indexName, long? identifier, Exception e)
        {
            // We don't want to flood the logs if an indexing operation is going awry, but
            // we do want the ability to debug it in production when required.
            if (indexName != null)
            {
                Debug.WriteLine("Indexing failed for {0} - '{1}'", indexName, identifier);
            }
            Debug.WriteLine(e.Message, e);
        }

        /// <summary>
        /// This class holds the actual result objects and aggregates them. Once a
        /// result has been awaited then it can be discarded.
        /// </summary>
        private sealed class CompositeResult : IIndexResult
        {
            private readonly IEnumerable<InFlightResult> results;
            private readonly LinkedList<ThreadStart> completionTasks;

            internal CompositeResult(IEnumerable<InFlightResult> inFlightResults, int successes, int failures, ICollection<ThreadStart> completionTasks)
            {
                Successes = successes;
                Failures = failures;

                var bc = new BlockingCollection<InFlightResult>();
                foreach (var inFlightResult in inFlightResults)
                {
                    bc.Add(inFlightResult);
                }
                results = bc;
                this.completionTasks = new LinkedList<ThreadStart>(completionTasks);
            }

            public void Await()
			{
                for (IEnumerator<InFlightResult> it = results.GetEnumerator(); it.MoveNext();)
                {
                    // all threads should await
                    InFlightResult ifr = it.Current;
                    IIndexResult result = ifr.Result;
                    
                    try
                    {
                        result.Await();
                        Successes++;
                    }
                    catch (Exception e)
                    {
                        Failures++;
                        logFailure(ifr.IndexName, ifr.Identifier, e);
                    }

                    // once run, they should be removed
                    it.Remove();
                }

				if (Failures > 0)
				{
					throw new IndexingFailureException(Failures);
				}
				Complete();
			}

            internal void Complete()
            {
                while (completionTasks.Count > 0)
                {
                    // only one thread should run these tasks
                    completionTasks.RemoveFirst();
                    if (completionTasks.Any())
                    {
                        completionTasks.First().Invoke();
                    }
                }
            }

            public bool Await(long time, TimeSpan unit)
            {
				var timeout = TimeSpan.getNanosTimeout(time, unit);
                for (IEnumerator<InFlightResult> it = results.GetEnumerator(); it.MoveNext(); )
                {
                    // all threads should await
                    InFlightResult ifr = it.Current;
                    IIndexResult result = ifr.Result;
                    try
                    {
                        if (!result.Await(timeout.Time, timeout.Unit))
                        {
                            return false;
                        }
                        Successes++;
                    }
                    catch (Exception e)
                    {
                        Failures++;
                        logFailure(ifr.IndexName, ifr.Identifier, e);
                    }
                    // once run, they should be removed
                    it.remove();
                }

                if (Failures > 0)
                {
                    throw new IndexingFailureException(Failures);
                }
                
                Complete();
                return true;
			}

            public bool Done
            {
                get { return results.All(ifr => ifr.Result.Done); }
            }

            internal IEnumerable<InFlightResult> Results
            {
                get { return results; }
            }

            internal int Successes { get; set; }
            internal int Failures { get; set; }
        }

        private class InFlightResult
        {
            public virtual string IndexName { get; private set; }
            public virtual long? Identifier { get; private set; }
            public virtual IIndexResult Result { get; private set; }

            internal InFlightResult(string indexName, long? identifier, IIndexResult result)
            {
                IndexName = indexName;
                Identifier = identifier;
                Result = result;
            }
        }
    }
}
