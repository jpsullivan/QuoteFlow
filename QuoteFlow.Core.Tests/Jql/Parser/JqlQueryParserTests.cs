using System.Collections.Generic;
using QuoteFlow.Api.Jql.Query;
using QuoteFlow.Api.Jql.Query.Clause;
using QuoteFlow.Core.Jql.Parser;
using Xunit;
using Xunit.Extensions;

namespace QuoteFlow.Core.Tests.Jql.Parser
{
    /// <summary>
    /// Tests for the <see cref="JqlQueryParser"/>.
    /// </summary>
    public class JqlQueryParserTests
    {
        // Set of characters that are reserved but not currently used within JQL.
        private string _illegalCharsString = "{}*/%+$#@?;";

        private List<string> _currentWords = new List<string>()
        {
            "empty",
            "null",
            "and",
            "or",
            "not",
            "in",
            "is",
            "cf",
            "issue.property",
            "order",
            "by",
            "desc",
            "asc",
            "on",
            "before",
            "to",
            "after",
            "from",
            "was",
            "changed"
        };

        [Theory]
        [InlineData("priority = \"qwerty\"")]
        [InlineData("priority=\"qwerty\"")]
        [InlineData("priority=qwerty")]
        [InlineData("  priority=qwerty  ")]
        [InlineData("priority=     qwerty order      by priority, other")]
        public void WhiteSpace_IsIgnored_DuringParsing(string jql)
        {
            var expectedClause = new TerminalClause("priority", Operator.EQUALS, "qwerty");
            Evaluate(jql, expectedClause);
        }

//        [Theory]
//        [InlineData("key = one-1", new TerminalClause("key", Operator.EQUALS, "one-1"))]
//        [InlineData(new []{ new TerminalClause("key", Operator.EQUALS, "one-1")})]
//        public void CheckingForMinus_AfterRemoving_FromTheReservedCharsList()
//        {
//            
//        }

        private static void Evaluate(string jql, IClause expectedClause)
        {
            var parser = new JqlQueryParser();
            var query = parser.ParseQuery(jql);
            var whereClause = query.WhereClause;
            Assert.Same(jql, query.QueryString);
            Assert.Equal(expectedClause, whereClause);
        }
    }
}
