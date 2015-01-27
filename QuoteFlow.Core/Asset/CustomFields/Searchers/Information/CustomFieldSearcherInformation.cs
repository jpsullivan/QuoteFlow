using System.Collections.Generic;
using QuoteFlow.Api.Asset.Fields;
using QuoteFlow.Api.Asset.Index.Indexers;
using QuoteFlow.Api.Asset.Search.Searchers;
using QuoteFlow.Api.Asset.Search.Searchers.Information;
using QuoteFlow.Api.Infrastructure.Concurrency;

namespace QuoteFlow.Core.Asset.CustomFields.Searchers.Information
{
    public class CustomFieldSearcherInformation : GenericSearcherInformation<ICustomField>
    {
        private readonly IEnumerable<IFieldIndexer> indexers;
        private readonly AtomicReference<ICustomField> fieldReference;

        public CustomFieldSearcherInformation(string id, string nameKey, IEnumerable<IFieldIndexer> indexers, AtomicReference<ICustomField> fieldReference)
            : base(id, nameKey, new List<IFieldIndexer>(), fieldReference, SearcherGroupType.Custom)
        {
            this.indexers = indexers;
            this.fieldReference = fieldReference;
        }

        /// <summary>
        /// Regular <seealso cref="IAssetSearcher"/>s get their <seealso cref="IFieldIndexer"/>s
        /// by instantiating the class objects passed to them. However, Custom Fields work differently because they
        /// have their indexers instantiated elsewhere for them.
        /// </summary>
        /// <returns> the indexers for this custom field searcher </returns>
        public override IEnumerable<IFieldIndexer> RelatedIndexers
        {
            get
            {
//                var customField = fieldReference.Get();
//                var relatedIndexers = customField.CustomFieldType.getRelatedIndexers(customField);
//                if (relatedIndexers != null)
//                {
//                    return relatedIndexers;
//                }
                return indexers;
            }
        }

        public override string NameKey
        {
            get { return fieldReference.Get().NameKey; }
        }
    }
}