using Antlr.Runtime;
using Moq;
using QuoteFlow.Core.Jql.Parser.Antlr;
using Xunit;

namespace QuoteFlow.Core.Tests.Jql.Parser.Antlr
{
    public class AntlrPositionTests
    {
        public class TheCtor
        {
            [Fact]
            public void ProperlyAssignsValues()
            {
                const int charPos = 79548;
                const int index = 5;
                const int line = 6;
                const int tokenType = 628273;

                var stream = new Mock<ICharStream>();
                stream.Setup(x => x.Index).Returns(index);
                stream.Setup(x => x.Line).Returns(line);
                stream.Setup(x => x.CharPositionInLine).Returns(charPos);

                var position = new AntlrPosition(tokenType, stream.Object);

                Assert.Equal(tokenType, position.TokenType);
                Assert.Equal(index, position.Index);
                Assert.Equal(line, position.LinePosition);
                Assert.Equal(charPos, position.CharPosition);

                stream.Verify();
            }
        }
    }
}
