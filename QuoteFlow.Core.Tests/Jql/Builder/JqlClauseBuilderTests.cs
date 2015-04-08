using Moq;
using QuoteFlow.Core.Jql.Builder;
using QuoteFlow.Core.Jql.Util;
using Xunit;

namespace QuoteFlow.Core.Tests.Jql.Builder
{
    public class JqlClauseBuilderTests
    {
        [Fact]
        public void TestClear()
        {
            var mockClauseBuilder = new Mock<ISimpleClauseBuilder>(MockBehavior.Strict);
            var mockClauseBuilder2 = new Mock<ISimpleClauseBuilder>(MockBehavior.Strict);

            mockClauseBuilder.Setup(x => x.Clear()).Returns(mockClauseBuilder2.Object);
            mockClauseBuilder2.Setup(x => x.Clear()).Returns(mockClauseBuilder.Object);

            var builder = CreateBuilder(mockClauseBuilder.Object);
            Assert.Equal(builder, builder.Clear());
            Assert.Equal(builder, builder.Clear());

            mockClauseBuilder.Verify();
            mockClauseBuilder2.Verify();
        }

        private static JqlClauseBuilder CreateBuilder(ISimpleClauseBuilder clauseBuilder)
        {
            JqlDateSupport dateSupport = new JqlDateSupport();
            return CreateBuilder(clauseBuilder, dateSupport);
        }

        private static JqlClauseBuilder CreateBuilder(ISimpleClauseBuilder clauseBuilder, JqlDateSupport dateSupport)
        {
            return new JqlClauseBuilder(null, clauseBuilder, dateSupport);
        }
    }
}