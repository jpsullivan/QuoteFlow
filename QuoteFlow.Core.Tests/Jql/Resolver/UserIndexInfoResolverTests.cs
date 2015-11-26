using System.Collections.Generic;
using Moq;
using QuoteFlow.Api.Jql.Resolver;
using QuoteFlow.Api.Models;
using QuoteFlow.Core.Jql.Resolver;
using Xunit;

namespace QuoteFlow.Core.Tests.Jql.Resolver
{
    public class UserIndexInfoResolverTests
    {
        private static readonly List<string> Empty = new List<string>();

        private const int Id = 10;
        private static readonly string IdString = Id.ToString();
        private static readonly string UserName = "name";
        private static readonly string OtherName = "otherName";

        private static readonly Mock<INameResolver<User>> MockResolver = new Mock<INameResolver<User>>();

        public class TheGetIdsFromNameMethod
        {
            [Fact]
            public void StringShouldReturnEmpty()
            {
                MockResolver.Setup(x => x.GetIdsFromName(UserName)).Returns(Empty);
                Assert.Equal(new List<string>(), Resolver().GetIndexedValues(UserName));
            }

            [Fact]
            public void IdShouldReturnEmpty()
            {
                MockResolver.Setup(x => x.GetIdsFromName(IdString)).Returns(Empty);
                Assert.Equal(new List<string>(), Resolver().GetIndexedValues(Id));
            }

            [Fact]
            public void StringReturnsList()
            {
                MockResolver.Setup(x => x.GetIdsFromName(IdString)).Returns(new List<string>{UserName});
                Assert.Equal(new List<string> {UserName}, Resolver().GetIndexedValues(Id));
            }
        }

        private static UserIndexInfoResolver Resolver()
        {
            return new UserIndexInfoResolver(MockResolver.Object);
        }
    }
}