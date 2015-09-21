using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using QuoteFlow.Core.Asset.Index;
using QuoteFlow.Core.Lucene.Index;
using Xunit;

namespace QuoteFlow.Core.Tests.Asset.Index
{
    public class AssetIndexerTests
    {
        private Mock<IIndexDirectoryFactory> _indexDirectoryFactory;
        private Mock<IAssetDocumentFactory> _assetDocumentFactory;

        private IDictionary<IndexDirectoryFactoryName, IIndexManager> _indexManagers;
        private AssetIndexer _assetIndexer;

        public AssetIndexerTests()
        {
            _indexManagers = new Dictionary<IndexDirectoryFactoryName, IIndexManager>();
            var values = Enum.GetValues(typeof(IndexDirectoryFactoryName)).Cast<IndexDirectoryFactoryName>();
            foreach (var indexName in values)
            {
                _indexManagers.Add(indexName, new Mock<IIndexManager>().Object);
            }

            _indexDirectoryFactory.Setup(x => x.Get()).Returns(_indexManagers);
            _assetIndexer = new AssetIndexer(_indexDirectoryFactory.Object, _assetDocumentFactory.Object);
        }

        [Fact]
        public void TestDeleteIndexesOnSelectedIndexes()
        {
            _assetIndexer.DeleteIndexes(IssueIndexingParams.builder().withoutIssues().withComments().withWorklogs().build());
        }

        [Fact]
        public void TestDeleteIndexesWhenAllIndexAreSelected()
        {
            
        }
    }
}