using System;
using System.Collections.Generic;
using System.Linq;
using Ninject;
using QuoteFlow.Infrastructure.Concurrency;
using QuoteFlow.Infrastructure.Extensions;
using QuoteFlow.Models.Assets.Fields;
using QuoteFlow.Models.Assets.Index.Indexers;

namespace QuoteFlow.Models.Assets.Search.Searchers.Information
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
            if (id.IsNullOrEmpty())
            {
                throw new ArgumentException("ID Cannot be empty", "id");
            }

            if (nameKey.IsNullOrEmpty())
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
                return (IFieldIndexer) Container.Kernel.Get(type);
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