using System.Collections.Generic;
using QuoteFlow.Api.Jql.Context;

namespace QuoteFlow.Core.Tests.Jql.Context
{
    public class MockClauseContext : IClauseContext
    {
        public ISet<ICatalogManufacturerContext> Contexts { get; private set; }

        public MockClauseContext()
        {
            Contexts = new HashSet<ICatalogManufacturerContext>();
        }

        public bool ContainsGlobalContext()
        {
            return Contexts.Contains(CatalogManufacturerContext.CreateGlobalContext());
        }
    }
}