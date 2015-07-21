using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using QuoteFlow.Api.Lucene.Index;

namespace QuoteFlow.Core.Lucene.Index
{
    /// <summary>
    /// Used to build a <see cref="IIndexResult"/> implementation that accumulates results from
    /// other operations and awaits on them all.
    /// 
    /// For operations that are complete it just aggregates their results.
    /// </summary>
    public sealed class AccumulatingResultBuilder
    {
        //private static readonly RateLimitingLogger log = new RateLimitingLogger(typeof(AccumulatingResultBuilder));
        private readonly List<InFlightResult> _inFlightResults = new List<InFlightResult>();
        private int successesToDate = 0;
        private int failuresToDate = 0;
        private readonly ICollection<ThreadStart> completionTasks = new LinkedList<ThreadStart>();

        public AccumulatingResultBuilder Add(IIndexResult result)
        {
            if (result == null)
            {
                throw new ArgumentNullException(nameof(result));
            }

            var compResult = result as CompositeResult;
            if (compResult != null)
            {
                CompositeResult compositeResult = compResult;
                foreach (InFlightResult ifr in compositeResult.Results)
                {
                    AddInternal(ifr);
                }

                successesToDate += compositeResult.Successes;
                failuresToDate += compositeResult.Failures;
            }
            else
            {
                AddInternal(null, null, result);
            }
            return this;
        }

        public AccumulatingResultBuilder Add(string indexName, int identifier, IIndexResult result)
        {
            if (result == null) throw new ArgumentNullException(nameof(result));

            if (result is CompositeResult)
            {
                Add(result);
            }
            else
            {
                AddInternal(indexName, identifier, result);
            }

            return this;
        }

        private void AddInternal(InFlightResult ifr)
        {
            CheckCompleted();
            if (ifr.Result.Done)
            {
                CollectResult(ifr.IndexName, ifr.Identifier, ifr.Result);
            }
            else
            {
                _inFlightResults.Add(ifr);
            }
        }

        private void AddInternal(string indexName, long? identifier, IIndexResult result)
        {
            CheckCompleted();
            if (result.Done)
            {
                CollectResult(indexName, identifier, result);
            }
            else
            {
                _inFlightResults.Add(new InFlightResult(indexName, identifier, result));
            }
        }

        public void AddCompletionTask(ThreadStart runnable)
        {
            completionTasks.Add(runnable);
        }

        /// <summary>
        /// Keep the results list small, we don't want to waste too much ram with
        /// complete results.
        /// </summary>
        private void CheckCompleted()
        {
            for (IEnumerator<InFlightResult> iterator = _inFlightResults.GetEnumerator(); iterator.MoveNext();)
            {
                InFlightResult ifr = iterator.Current;
                if (ifr.Result.Done)
                {
                    CollectResult(ifr.IndexName, ifr.Identifier, ifr.Result);
                    _inFlightResults.Remove(ifr);
                }
            }
        }

        private void CollectResult(string indexName, long? identifier, IIndexResult result)
        {
            try
            {
                result.Await();
                successesToDate++;
            }
            catch (Exception e)
            {
                failuresToDate++;
                LogFailure(indexName, identifier, e);
            }
        }

        public IIndexResult ToResult()
        {
            return new CompositeResult(_inFlightResults, successesToDate, failuresToDate, completionTasks);
        }

        private static void LogFailure(string indexName, long? identifier, Exception e)
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
            private readonly List<InFlightResult> results;
            private readonly LinkedList<ThreadStart> completionTasks;

            internal CompositeResult(IEnumerable<InFlightResult> inFlightResults, int successes, int failures, ICollection<ThreadStart> completionTasks)
            {
                Successes = successes;
                Failures = failures;

                var bc = inFlightResults.ToList();
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
                        LogFailure(ifr.IndexName, ifr.Identifier, e);
                    }

                    // once run, they should be removed
                    results.Remove(ifr);
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

//            public bool Await(long time, TimeSpan unit)
//            {
//				var timeout = TimeSpan.getNanosTimeout(time, unit);
//                for (IEnumerator<InFlightResult> it = results.GetEnumerator(); it.MoveNext(); )
//                {
//                    // all threads should await
//                    InFlightResult ifr = it.Current;
//                    IIndexResult result = ifr.Result;
//                    try
//                    {
//                        if (!result.Await(timeout.Time, timeout.Unit))
//                        {
//                            return false;
//                        }
//                        Successes++;
//                    }
//                    catch (Exception e)
//                    {
//                        Failures++;
//                        LogFailure(ifr.IndexName, ifr.Identifier, e);
//                    }
//                    // once run, they should be removed
//                    it.Remove();
//                }
//
//                if (Failures > 0)
//                {
//                    throw new IndexingFailureException(Failures);
//                }
//                
//                Complete();
//                return true;
//			}

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
