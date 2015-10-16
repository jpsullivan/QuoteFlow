using System.Linq;
using Lucene.Net.Documents;
using QuoteFlow.Core.Asset.Index.Indexers;
using Xunit;

namespace QuoteFlow.Core.Tests.Asset.Index.Indexers
{
    public class AssetIdIndexerTests
    {
        [Fact]
        public void TestNullAsset()
        {
            var indexer = new AssetIdIndexer();
            var doc = new Document();
            indexer.AddIndex(doc, null);
            Assert.False(doc.GetFields().Any());
        }

        [Fact]
        public void TestZeroId()
        {
            var asset = new Api.Models.Asset();
            var indexer = new AssetIdIndexer();
            var doc = new Document();
            indexer.AddIndex(doc, asset);
            Assert.False(doc.GetFields().Any());
        }
    }
}