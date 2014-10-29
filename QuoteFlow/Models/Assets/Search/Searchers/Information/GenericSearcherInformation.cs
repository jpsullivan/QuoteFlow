using System;
using System.Collections.Generic;
using System.Linq;
using QuoteFlow.Models.Assets.Fields;
using QuoteFlow.Models.Assets.Index.Indexers;

namespace QuoteFlow.Models.Assets.Search.Searchers.Information
{
    public class GenericSearcherInformation<T> : ISearcherInformation<T> where T : ISearchableField
    {
        public string Id { get; private set; }
        public string NameKey { get; private set; }
        public T Field { get; private set; }
        public SearcherGroupType SearcherGroupType { get; private set; }

        private IEnumerable<IFieldIndexer> indexers; 

        public GenericSearcherInformation(string id, string nameKey, T field, IEnumerable<IFieldIndexer> relatedIndexers, SearcherGroupType searcherGroupType)
        {
            Id = id;
            NameKey = nameKey;
            Field = field;
            this.indexers = relatedIndexers;
            SearcherGroupType = searcherGroupType;
        }

        public IEnumerable<IFieldIndexer> RelatedIndexers
        {
            get { return (from Type clazz in indexers select LoadIndexer(clazz)).ToList(); }
        }

        internal IFieldIndexer LoadIndexer(Type clazz)
        {
            try
            {
                return (IFieldIndexer) Activator.CreateInstance(clazz);
            }
            catch (Exception e)
            {
                throw new Exception("Failed to load indexer '" + clazz.Name + "'", e);
            }
        }

    }
}