using System;
using System.Collections.Generic;
using System.Linq;
using Ninject;
using QuoteFlow.Api.Asset.Fields;
using QuoteFlow.Api.Asset.Index.Indexers;
using QuoteFlow.Api.Asset.Search.Searchers;
using QuoteFlow.Api.Asset.Search.Searchers.Information;
using QuoteFlow.Api.Infrastructure.Concurrency;
using QuoteFlow.Core.DependencyResolution;

namespace QuoteFlow.Core.Asset.CustomFields.Searchers.Information
{
    public class GenericSearcherInformation<T> : ISearcherInformation<T> where T : class, ISearchableField
    {
        public string Id { get; private set; }
        public virtual string NameKey { get; private set; }
        private AtomicReference<T> FieldReference { get; set; }
        public SearcherGroupType SearcherGroupType { get; private set; }

        private IEnumerable<IFieldIndexer> indexers;

        public GenericSearcherInformation(string id, string nameKey, IEnumerable<IFieldIndexer> indexers, 
            AtomicReference<T> fieldReference, SearcherGroupType searcherGroupType)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentException("ID Cannot be empty", "id");
            }

            if (string.IsNullOrEmpty(nameKey))
            {
                throw new ArgumentException("NameKey Cannot be empty", "nameKey");
            }

            if (fieldReference == null)
            {
                throw new ArgumentNullException("fieldReference");
            }

            Id = id;
            NameKey = nameKey;
            this.indexers = indexers;
            FieldReference = fieldReference;
            SearcherGroupType = searcherGroupType;
        }

        public virtual IEnumerable<IFieldIndexer> RelatedIndexers
        {
            get
            {
                return indexers.Select(LoadIndexer).ToList();
            }
        }

        private static IFieldIndexer LoadIndexer(IFieldIndexer clazz)
        {
            try
            {
                var type = clazz.GetType();
                return (IFieldIndexer) Container.Kernel.TryGet(type);
            }
            catch (Exception e)
            {
                throw new Exception("Failed to load indexer '" + clazz.GetType() + "'", e);
            }
        }

        public virtual T Field
        {
            get { return FieldReference.Get(); }
        }
    }
}