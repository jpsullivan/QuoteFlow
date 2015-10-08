using Moq;
using QuoteFlow.Api.Services;
using QuoteFlow.Core.Asset.Index;
using QuoteFlow.Core.Index;
using Xunit;

namespace QuoteFlow.Core.Tests.Index
{
    public class AssetIndexHelperTests
    {
        private static readonly Mock<IAssetService> AssetService = new Mock<IAssetService>();
        private static readonly Mock<IAssetIndexer> AssetIndexer = new Mock<IAssetIndexer>();

        public class TheEnsureCapacityMethod
        {
            [Fact]
            public void Test()
            {
                var assetIndexHelper = new AssetIndexHelper(AssetService.Object, AssetIndexer.Object);

                CheckArrayExpandsCorrectly(assetIndexHelper, 0, 0);
                CheckArrayExpandsCorrectly(assetIndexHelper, 0, 1);
                CheckArrayExpandsCorrectly(assetIndexHelper, 1, 1);
                CheckArrayExpandsCorrectly(assetIndexHelper, 2, 2);
                CheckArrayExpandsCorrectly(assetIndexHelper, 10, 11);
                CheckArrayExpandsCorrectly(assetIndexHelper, 10, 100);
                CheckArrayExpandsCorrectly(assetIndexHelper, 1000, 1000);
                CheckArrayExpandsCorrectly(assetIndexHelper, 1000, 1001);
                CheckArrayExpandsCorrectly(assetIndexHelper, 1000, 999);
                CheckArrayExpandsCorrectly(assetIndexHelper, 1000, 99);
                CheckArrayExpandsCorrectly(assetIndexHelper, 1000, 1);
            }

            private void CheckArrayExpandsCorrectly(AssetIndexHelper assetIndexHelper, int currentSize, int entryToAdd)
            {
                var assetIds = new int[currentSize];
                assetIds = assetIndexHelper.EnsureCapacity(assetIds, entryToAdd + 1);
                assetIds[entryToAdd] = 0;
            }
        } 
    }
}