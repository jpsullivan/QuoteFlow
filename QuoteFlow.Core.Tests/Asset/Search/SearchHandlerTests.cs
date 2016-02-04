using System;
using System.Collections.Generic;
using QuoteFlow.Api.Asset.Search;
using QuoteFlow.Core.Jql;
using QuoteFlow.Core.Tests.Jql;
using QuoteFlow.Core.Tests.Jql.Context;
using QuoteFlow.Core.Tests.Jql.Query;
using QuoteFlow.Core.Tests.Jql.Validator;
using Xunit;

namespace QuoteFlow.Core.Tests.Asset.Search
{
    public class SearchHandlerTests
    {
        [Fact]
        public void TestClauseRegistrationCtor()
        {
            var clauseFactory = new MockClauseQueryFactory();
            var clauseValidator = new MockClauseValidator();

            var otherNames = new List<string>() {"jaysche"};
            var names = new ClauseNames("jaysche", otherNames);
            var handler = new ClauseHandler(new MockClauseInformation(names), clauseFactory, clauseValidator, new MockClausePermissionChecker(), new MockClauseContextFactory());

            var rego = new SearchHandler.ClauseRegistration(handler);
            Assert.Equal(handler, rego.Handler);
            Assert.Equal(names, rego.Handler.Information.JqlClauseNames);

            rego = new SearchHandler.ClauseRegistration(new ClauseHandler(new MockClauseInformation(names), clauseFactory, clauseValidator, new MockClausePermissionChecker(), new MockClauseContextFactory()));
            Assert.Equal(names, rego.Handler.Information.JqlClauseNames);
            Assert.Equal(clauseFactory, rego.Handler.Factory);
            Assert.Equal(clauseValidator, rego.Handler.Validator);
        }

        [Fact]
        public void TestClauseRegistrationCtorBad()
        {
            var goodNames = new ClauseNames("dude", new List<string>() { "jack" });
            var clauseFactory = new MockClauseQueryFactory();
            var clauseValidator = new MockClauseValidator();
            var clauseContextFactory = new MockClauseContextFactory();

            Assert.Throws<ArgumentNullException>(() => new SearchHandler.ClauseRegistration(null));
            Assert.Throws<ArgumentNullException>(() => new ClauseHandler(new MockClauseInformation(goodNames), clauseFactory, null, null, clauseContextFactory));
            Assert.Throws<ArgumentNullException>(() => new ClauseHandler(new MockClauseInformation(goodNames), null, clauseValidator, null, clauseContextFactory));
        }
    }
}