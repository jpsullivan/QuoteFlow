using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using QuoteFlow.Api.Asset.Index;
using QuoteFlow.Api.Infrastructure.Elmah;
using QuoteFlow.Api.Infrastructure.Lucene;

namespace QuoteFlow.Core.Util.Index
{
    /// <summary>
    /// Convenience class for managing known IndexManagers and calling them all.
    /// </summary>
    public class CompositeIndexLifecycleManager : IIndexLifecycleManager
    {
        private readonly IIndexLifecycleManager[] _delegates;

        public CompositeIndexLifecycleManager(IAssetIndexManager assetIndexManager)
        {
            _delegates = new IIndexLifecycleManager[] { assetIndexManager };
        }

        public ICollection<string> AllIndexPaths
        {
            get
            {
                var result = new List<string>();
                foreach (var @delegate in _delegates)
                {
                    result.AddRange(@delegate.AllIndexPaths);
                }
                return result;
            }
        }

        public int Size()
        {
            return _delegates.Sum(@delegate => @delegate.Size());
        }

        public bool IsEmpty
        {
            get { return Size() == 0; }
        }

        public int ReIndexAll()
        {
            Debug.WriteLine("Reindex All starting...");

            int result = 0;
            foreach (IIndexLifecycleManager @delegate in _delegates)
            {
                try
                {
                    int reIndexAll = @delegate.ReIndexAll();
                    Debug.WriteLine("Reindex took: {0}ms. Indexer: {1}", reIndexAll, @delegate);
                    result += reIndexAll;
                }
                catch (Exception re)
                {
                    QuietLog.LogHandledException(re);
                    Debug.WriteLine("Reindex All FAILED.  Indexer: {0}", @delegate.ToString());
                    throw re;
                }
            }
//            long newCounterValue = indexingCounterManager.incrementValue();
//            context.Name = "";
//            log.info("Reindex All complete. Total time: " + result + "ms. Reindex run: " + newCounterValue);
//
//            nodeReindexService.start();

            return result;
        }

        public int ReIndexAllIssuesInBackground()
        {
            return ReIndexAllIssuesInBackground(false);
        }

        public int ReIndexAllIssuesInBackground(bool reIndexComments)
        {
            Debug.WriteLine("Reindex All In Background starting...");
            int result = 0;
            foreach (IIndexLifecycleManager @delegate in _delegates)
            {
                try
                {
                    int reIndexAll = @delegate.ReIndexAllIssuesInBackground(reIndexComments);
                    Debug.WriteLine("Reindex took: {0}ms. Indexer: {1}", reIndexAll, @delegate);
                    result += reIndexAll;
                }
//                catch (RuntimeInterruptedException rie)
//                {
//                    log.warn("Reindex All In Background CANCELLED. Indexer: " + @delegate.ToString());
//                    throw rie;
//                }
                catch (Exception re)
                {
                    Debug.WriteLine("Reindex All In Background FAILED. Indexer: {0}", @delegate.ToString());
                    throw re;
                }
            }
//            long newCounterValue = indexingCounterManager.incrementValue();
//            context.Name = "";
//            log.info("Reindex All In Background complete. Total time: " + result + "ms. Reindex run: " + newCounterValue);

            return result;
        }

        public int Optimize()
        {
            Debug.WriteLine("Optimize Indexes starting...");

            int result = 0;
            foreach (IIndexLifecycleManager @delegate in _delegates)
            {
                int optimize = @delegate.Optimize();

                Debug.WriteLine("Optimize took: {0}ms. Indexer: {1}", optimize, @delegate);
                result += optimize;
            }
            Debug.WriteLine("Optimize Indexes complete. Total time: {0}ms.", result);
            return result;
        }

        public int Activate()
        {
            return Activate(true);
        }

        public int Activate(bool reindex)
        {
            return _delegates.Sum(@delegate => @delegate.Activate(reindex));
        }

        public void Deactivate()
        {
            foreach (IIndexLifecycleManager @delegate in _delegates)
            {
                @delegate.Deactivate();
            }
        }

        public bool IndexAvailable
        {
            get { return _delegates[0].IndexAvailable; }
        }

        public bool IndexConsistent
        {
            get { return _delegates.All(@delegate => @delegate.IndexConsistent); }
        }

        public void Shutdown()
        {
            foreach (IIndexLifecycleManager @delegate in _delegates)
            {
                @delegate.Shutdown();
            }
        }
    }
}