using System.Collections.Generic;
using QuoteFlow.Api.Jql.Query.Clause;
using Xunit;

namespace QuoteFlow.Core.Tests.Jql.Query.Clause
{
    public class PropertyTests
    {
        [Fact(Skip = "Xunit is getting confused here...")]
        public void SplitsKeyAndObjectReferences_If_ContainsDots()
        {
            var property = new Property(new List<string> {"key1.key2", "key3"}, new List<string> {"prop1.prop2", "prop3"});
            //Assert.Contains(property.Keys, new List<string> {"key1", "key2", "key3"});
        }
    }
}
