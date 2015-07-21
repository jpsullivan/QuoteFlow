using System;
using QuoteFlow.Api.Lucene.Index;
using QuoteFlow.Core.Index;

namespace QuoteFlow.Core.Lucene.Index
{
    /// <summary>
    /// Uses a <see cref="Index.Engine"/> to perform actual writes to an index.
    /// </summary>
    public class Index : IDisposableIndex
    {
        private IEngine Engine { get; set; }

        public Index(IEngine engine)
        {
            if (engine == null)
            {
                throw new ArgumentNullException(nameof(engine));
            }

            Engine = engine;
        }
        
        public IIndexResult Perform(Operation operation)
        {
            if (operation == null)
            {
                throw new ArgumentNullException(nameof(operation));
            }

            try
            {
                Engine.Write(operation);
                return new Success();
            }
            catch (Exception e)
            {
                return new Failure(e);
            }
        }

        public void Dispose()
        {
            Engine.Dispose();
        }

        /// <summary>
        /// Indicate that an operation completed successfully.
        /// </summary>
        public sealed class Success : IIndexResult
        {
            public void Await()
            {
            }

            public bool Done => true;
        }

        /// <summary>
        /// Indicate that an operation failed.
        /// </summary>
        public sealed class Failure : IIndexResult
        {
            private readonly Exception _failure;

            public Failure(Exception failure)
            {
                _failure = failure;
            }

            public void Await()
            {
                DoThrow();
            }

            public bool Done => true;

            private bool DoThrow()
            {
                throw _failure;
            }
        }
    }
}
