using System;
using System.IO;
using Moq;
using QuoteFlow.Api.Util.Support;
using QuoteFlow.Core.Index;
using QuoteFlow.Core.Lucene.Index;
using Xunit;

namespace QuoteFlow.Core.Tests.Index
{
    public class IndexTests
    {
        [Fact]
        public void TestPerformThrowsForNullOperation()
        {
            var index = new Core.Lucene.Index.Index(new Mock<IEngine>().Object);
            Assert.Throws<ArgumentNullException>(() => index.Perform(null));
        }

        [Fact]
        public void TestPerformReturnsFailureIfExceptionWriting()
        {
            var blah = new IOException("blah");

            var mockIndexEngine = new Mock<IEngine>();
            mockIndexEngine.Setup(x => x.Write(It.IsAny<Operation>())).Callback(() => { throw blah; });
            var index = new Core.Lucene.Index.Index(mockIndexEngine.Object);
            var result = index.Perform(new MockOperation());
            Assert.True(result.Done);

            try
            {
                result.Await();
            }
            catch (Exception ex)
            {
                Assert.Same(blah, ex);
            }
        }

        [Fact]
        public void TestDispose()
        {
            var count = new AtomicInteger();
            var mockIndexEngine = new MockIndexEngine {OnDispose = () => { count.IncrementAndGet(); }};
            var index = new Core.Lucene.Index.Index(mockIndexEngine);

            Assert.Equal(0, count.Get());
            index.Dispose();
            Assert.Equal(1, count.Get());
            index.Dispose();
            Assert.Equal(2, count.Get());
        }
    }
}