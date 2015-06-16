using Moq;
using QuoteFlow.Api.Models;
using QuoteFlow.Api.Services;
using QuoteFlow.Core.Jql.Resolver;
using Xunit;

namespace QuoteFlow.Core.Tests.Jql.Resolver
{
    public class CatalogResolverTests
    {
        private static readonly Mock<ICatalogService> CatalogService = new Mock<ICatalogService>();
        private static readonly CatalogResolver Resolver = new CatalogResolver(CatalogService.Object);

        public class TheGetIdsFromNameMethod
        {

            [Fact]
            public void HappyPath()
            {
                var catalog = new Catalog
                {
                    Id = 1,
                    Name = "name"
                };

                CatalogService.Setup(x => x.GetCatalog(It.IsAny<string>())).Returns(catalog);

                var result = Resolver.GetIdsFromName("name");
                Assert.Contains("1", result);
            }
        }
    }
}