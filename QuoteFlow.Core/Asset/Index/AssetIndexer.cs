using System;
using System.Collections.Generic;
using Lucene.Net.Search;
using QuoteFlow.Api.Infrastructure.Concurrency;
using QuoteFlow.Api.Lucene.Index;
using QuoteFlow.Api.Models;
using QuoteFlow.Core.Lucene.Index;
using WebBackgrounder;

namespace QuoteFlow.Core.Asset.Index
{
    public class AssetIndexer : IAssetIndexer
    {
        public IAssetDocumentFactory AssetDocumentFactory { get; protected set; }


        public IIndexResult IndexAssets(IEnumerable<IAsset> assets, Job context)
        {
            throw new NotImplementedException();
        }

        public IIndexResult DeIndexAssets(IEnumerable<IAsset> assets, Job context)
        {
            throw new NotImplementedException();
        }

        public IIndexResult ReIndexAssets(IEnumerable<IAsset> assets, Job context, bool reIndexComments, bool reIndexChangeHistory, bool conditionalUpdate)
        {
            throw new NotImplementedException();
        }

        public IIndexResult ReIndexComments(ICollection<AssetComment> comments, Job context)
        {
            throw new NotImplementedException();
        }

        public IIndexResult IndexAssetsBatchMode(IEnumerable<IAsset> assets, Job context)
        {
            throw new NotImplementedException();
        }

        public IIndexResult Optimize()
        {
            throw new NotImplementedException();
        }

        public void DeleteIndexes()
        {
            throw new NotImplementedException();
        }

        public void Shutdown()
        {
            throw new NotImplementedException();
        }

        public IndexSearcher OpenAssetSearcher()
        {
            throw new NotImplementedException();
        }

        public IndexSearcher OpenCommentSearcher()
        {
            throw new NotImplementedException();
        }

        public IndexSearcher OpenChangeHistorySearcher()
        {
            throw new NotImplementedException();
        }

        public IList<string> IndexPaths { get; private set; }
        public string IndexRootPath { get; private set; }


        /// <summary>
        /// Manage the life-cycle of the three index managers.
        /// </summary>
        private class Lifecycle : IEnumerable<IIndexManager>
        {
            internal readonly AtomicReference<IDictionary<IndexDirectoryFactory.Name, IIndexManager>> @ref = new AtomicReference<IDictionary<IndexDirectoryFactory.Name, Index.Manager>>();
            internal readonly IndexDirectoryFactory factory;

            //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
            //ORIGINAL LINE: public Lifecycle(@Nonnull final IndexDirectoryFactory factory)
            public Lifecycle(IndexDirectoryFactory factory)
            {
                this.factory = notNull("factory", factory);
            }

            public virtual IEnumerator<Index.Manager> GetEnumerator()
            {
                return open().Values.GetEnumerator();
            }

            internal virtual void close()
            {
                IDictionary<IndexDirectoryFactory.Name, Index.Manager> indexes = @ref.getAndSet(null);
                if (indexes == null)
                {
                    return;
                }
                foreach (Index.Manager manager in indexes.Values)
                {
                    manager.close();
                }
            }

            internal virtual IDictionary<IndexDirectoryFactory.Name, Index.Manager> open()
            {
                IDictionary<IndexDirectoryFactory.Name, Index.Manager> result = @ref.get();
                while (result == null)
                {
                    @ref.compareAndSet(null, factory.get());
                    result = @ref.get();
                }
                return result;
            }

            internal virtual IAsset IssueIndex
            {
                get
                {
                    return get(Name.ISSUE).Index;
                }
            }

            internal virtual IAsset CommentIndex
            {
                get
                {
                    return get(Name.COMMENT).Index;
                }
            }

            internal virtual IIndexManager get(Name key)
            {
                return open()[key];
            }

            internal virtual IList<string> IndexPaths
            {
                get
                {
                    return factory.IndexPaths;
                }
            }

            internal virtual string IndexRootPath
            {
                get
                {
                    return factory.IndexRootPath;
                }
            }

            internal virtual Mode Mode
            {
                set
                {
                    factory.IndexingMode = value;
                }
            }
        }

    }
}