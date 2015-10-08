using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using QuoteFlow.Api.Asset.Index;
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

            _indexDirectoryFactory = new Mock<IIndexDirectoryFactory>();
            _indexDirectoryFactory.Setup(x => x.Get()).Returns(_indexManagers);
            _assetIndexer = new AssetIndexer(_indexDirectoryFactory.Object, _assetDocumentFactory.Object);
        }

        [Fact]
        public void TestDeleteIndexesOnSelectedIndexes()
        {
            _assetIndexer.DeleteIndexes(AssetIndexingParams.Builder().WithoutAssets().WithComments().Build());

            IIndexManager manager;
            if (_indexManagers.TryGetValue(IndexDirectoryFactoryName.Asset, out manager))
            {
                
            }
        }

        [Fact]
        public void TestDeleteIndexesWhenAllIndexAreSelected()
        {
            _assetIndexer.DeleteIndexes(AssetIndexingParams.Index_All);
            foreach (var manager in _indexManagers.Values)
            {
                
            }
        }
    }
}