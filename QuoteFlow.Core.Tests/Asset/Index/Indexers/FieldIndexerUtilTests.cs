using System;
using QuoteFlow.Api.Asset.Index.Indexers;
using Xunit;

namespace QuoteFlow.Core.Tests.Asset.Index.Indexers
{
    public class FieldIndexerUtilTests
    {
        [Fact]
        public void TestGetValueForSortingTrims()
        {
            Assert.Equal("test", FieldIndexerUtil.GetValueForSorting("   test  "));
        }

        [Fact]
        public void TestGetValueForSortingTrimsLarge()
        {
            Assert.Equal(
            "test",
            FieldIndexerUtil.GetValueForSorting("                                                               test                                                           "));
        }

        [Fact]
        public void TestGetValueForSortingLarge()
        {
            Assert.Equal(
            50,
            FieldIndexerUtil.GetValueForSorting(
                "0123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789").Length);
        }

        [Fact]
        public void TestGetValueForSortingNull()
        {
            Assert.Equal(Convert.ToString('\ufffd'), FieldIndexerUtil.GetValueForSorting(null));
        }
    }
}