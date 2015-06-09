using System.Collections.Generic;
using System.Linq;
using QuoteFlow.Api.Jql.Context;
using QuoteFlow.Core.Jql.Context;
using Xunit;

namespace QuoteFlow.Core.Tests.Jql.Context
{
    public class QueryContextTests
    {
        [Fact]
        public void GetCatalogManufacturerContexts_Explicit()
        {
            var manufacturers1 = new List<IManufacturerContext> {new ManufacturerContext(1), new ManufacturerContext(2)};
            var manufacturers2 = new List<IManufacturerContext> {new ManufacturerContext(1), new ManufacturerContext(3)};
            var context1 = new QueryContextCatalogManufacturerContexts(new CatalogContext(10), manufacturers1);
            var context2 = new QueryContextCatalogManufacturerContexts(new CatalogContext(20), manufacturers2);

            var inputContext1 = new CatalogManufacturerContext(new CatalogContext(10), new ManufacturerContext(1));
            var inputContext2 = new CatalogManufacturerContext(new CatalogContext(10), new ManufacturerContext(2));
            var inputContext3 = new CatalogManufacturerContext(new CatalogContext(20), new ManufacturerContext(1));
            var inputContext4 = new CatalogManufacturerContext(new CatalogContext(20), new ManufacturerContext(3));
            var clauseContext = new ClauseContext(new List<ICatalogManufacturerContext>
            {
                inputContext1,
                inputContext2,
                inputContext3,
                inputContext4
            });

            var queryContext = new QueryContext(clauseContext);
            var contextsCollection = queryContext.CatalogManufacturerContexts;
            Assert.Equal(2, contextsCollection.Count());
            Assert.True(contextsCollection.Contains(context1));
            Assert.True(contextsCollection.Contains(context2));
        }
    }
}