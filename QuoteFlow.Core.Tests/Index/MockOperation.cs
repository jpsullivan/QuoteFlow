using QuoteFlow.Api.Lucene.Index;
using QuoteFlow.Core.Index;
using QuoteFlow.Core.Lucene.Index;

namespace QuoteFlow.Core.Tests.Index
{
    public class MockOperation : Operation
    {
        public bool Performed { get; set; }
        private UpdateMode _mode;

        public MockOperation() : this(UpdateMode.Interactive)
        {
            
        }

        public MockOperation(UpdateMode mode)
        {
            _mode = mode;
        }


        public override void Perform(IWriter writer)
        {
            Performed = true;
        }

        public override UpdateMode Mode()
        {
            return _mode;
        }
    }
}