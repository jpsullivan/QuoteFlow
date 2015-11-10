using System;
using System.Collections.Generic;
using System.Linq;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using QuoteFlow.Api.Lucene.Index;
using QuoteFlow.Core.Lucene.Index;
using WebBackgrounder;

namespace QuoteFlow.Core.Index
{
    public class Operations
    {
        private Operations()
        {
            throw new InvalidOperationException("Do not instantiate!");
        }

        public static Operation NewDelete(Term term, UpdateMode mode)
        {
            return new Delete(term, mode);
        }

        public static Operation NewCreate(Document document, UpdateMode mode)
        {
            return new Create(document, mode);
        }

        public static Operation NewCreate(IEnumerable<Document> documents, UpdateMode mode)
        {
            return new Create(documents, mode);
        }

        public static Operation NewUpdate(Term term, Document document, UpdateMode mode)
        {
            return new Update(term, document, mode);
        }

        public static Operation NewConditionalUpdate(Term term, Document document, UpdateMode mode, string optimisticLockField)
        {
            return new ConditionalUpdate(term, document, mode, optimisticLockField);
        }

        public static Operation NewUpdate(Term term, IEnumerable<Document> documents, UpdateMode mode)
        {
            return new Update(term, documents, mode);
        }

        public static Operation NewOptimize()
        {
            return new Optimize();
        }

        /// <summary>
        /// Create an operation that delegates to another Operation and then runs the supplied
        /// completionJob as soon as the operation is performed (in whatever thread the operation
        /// is performed in).
        /// </summary>
        /// <param name="operation">The operation to delegate the actual work to. </param>
        /// <param name="completionJob">The Job instance that is run after the supplied operation completes. </param>
        /// <returns>The new composite operation.</returns>
        public static Operation NewCompletionDelegate(Operation operation, Job completionJob)
        {
            return new Completion(operation, completionJob);
        }

        /// <summary>
        /// Holds a an identifying <see cref="Term"/> so we can delete pre-existing documents.
        /// </summary>
        private sealed class Delete : Operation
        {
            public Term Term { get; private set; }
            private readonly UpdateMode _mode;

            internal Delete(Term term, UpdateMode mode)
            {
                if (term == null) throw new ArgumentNullException(nameof(term));
                Term = term;
                _mode = mode;
            }

            public override void Perform(IWriter writer)
            {
                writer.DeleteDocuments(Term);
            }

            public override UpdateMode Mode()
            {
                return _mode;
            }
        }

        /// <summary>
        /// Holds <see cref="Document"/> to be created.
        /// </summary>
        private sealed class Create : Operation
        {
            public IEnumerable<Document> Documents { get; private set; } 
            private readonly UpdateMode _mode;

            internal Create(Document document, UpdateMode mode)
            {
                if (document == null) throw new ArgumentNullException(nameof(document));

                Documents = new List<Document> { document };
                _mode = mode;
            }

            internal Create(IEnumerable<Document> documents, UpdateMode mode)
            {
                if (documents == null) throw new ArgumentNullException(nameof(documents));

                Documents = documents.ToList();
                _mode = mode;
            }

            public override void Perform(IWriter writer)
            {
                writer.AddDocuments(Documents);
            }

            public override UpdateMode Mode()
            {
                return _mode;
            }
        }

        private sealed class Update : Operation
        {
            private readonly Delete _delete;
            private readonly Create _create;

            internal Update(Term term, Document document, UpdateMode mode)
            {
                _delete = new Delete(term, mode);
                _create = new Create(document, mode);
            }

            internal Update(Term term, IEnumerable<Document> documents, UpdateMode mode)
            {
                _delete = new Delete(term, mode);
                _create = new Create(documents, mode);
            }

            public override void Perform(IWriter writer)
            {
                writer.UpdateDocuments(_delete.Term, _create.Documents);
            }

            public override UpdateMode Mode()
            {
                return _delete.Mode();
            }
        }

        private sealed class ConditionalUpdate : Operation
        {
            private readonly Create _create;
            private readonly Delete _delete;
            private readonly string _optimisticLockField;

            internal ConditionalUpdate(Term term, Document document, UpdateMode mode, string optimisticLockField)
            {
                _optimisticLockField = optimisticLockField;
                _create = new Create(document, mode);
                _delete = new Delete(term, mode);
            }

            public override void Perform(IWriter writer)
            {
                writer.UpdateDocumentConditionally(_delete.Term, _create.Documents.First(), _optimisticLockField);
            }

            public override UpdateMode Mode()
            {
                return _create.Mode();
            }
        }

        private sealed class Optimize : Operation
        {
            public override void Perform(IWriter writer)
            {
                writer.Optimize();
            }

            public override UpdateMode Mode()
            {
                return UpdateMode.Batch;
            }
        }

        private sealed class Completion : Operation
        {
            private readonly Job _completionJob;
            private readonly Operation _delegate;

            public Completion(Operation @delegate, Job completionJob)
            {
                _delegate = @delegate;
                _completionJob = completionJob;
            }

            public override void Perform(IWriter writer)
            {
                try
                {
                    _delegate.Perform(writer);
                }
                finally
                {
                    _completionJob?.Execute();
                }
            }

            public override UpdateMode Mode()
            {
                return _delegate.Mode();
            }
        }
    }
}