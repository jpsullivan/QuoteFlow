﻿using System;
using System.Collections.Generic;
using System.Linq;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using QuoteFlow.Api.Lucene.Index;
using QuoteFlow.Core.Lucene.Index;

namespace QuoteFlow.Core.Index
{
    public class Operations
    {
        private Operations()
        {
            throw new AssertionError("cannot instantiate!");
        }

        public static Operation NewDelete(Term term, UpdateMode mode)
        {
            return new Delete(term, mode);
        }

        public static Operation NewCreate(Document document, UpdateMode mode)
        {
            return new Create(document, mode);
        }

        public static Operation NewCreate(ICollection<Document> documents, UpdateMode mode)
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

        public static Operation NewUpdate(Term term, ICollection<Document> documents, UpdateMode mode)
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
        /// <param name="completionJob">The Runnable instance that is run after the supplied operation completes. </param>
        /// <returns>The new composite operation.</returns>
        public static Operation NewCompletionDelegate(Operation operation, Runnable completionJob)
        {
            return new Completion(operation, completionJob);
        }

        /// <summary>
        /// Holds a an identifying <seealso cref="Term"/> so we can delete pre-existing documents.
        /// </summary>
        internal sealed class Delete : Operation
        {
            public Term Term { get; private set; }
            private readonly UpdateMode _mode;

            internal Delete(Term term, UpdateMode mode)
            {
                if (term == null) throw new ArgumentNullException("term");
                Term = term;
                _mode = mode;
            }

            internal override void Perform(IWriter writer)
            {
                writer.DeleteDocuments(Term);
            }

            internal override UpdateMode Mode()
            {
                return _mode;
            }
        }

        /// <summary>
        /// Holds <seealso cref="Document"/> to be created.
        /// </summary>
        internal sealed class Create : Operation
        {
            public IEnumerable<Document> Documents { get; private set; } 
            private readonly UpdateMode _mode;

            //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
            //ORIGINAL LINE: Create(@Nonnull final org.apache.lucene.document.Document document, @Nonnull final com.atlassian.jira.index.Index.UpdateMode mode)
            internal Create(Document document, UpdateMode mode)
            {
                if (document == null) throw new ArgumentNullException("document");

                Documents = new List<Document> { document };
                _mode = mode;
            }

            //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
            //ORIGINAL LINE: Create(@Nonnull final java.util.Collection<org.apache.lucene.document.Document> documents, @Nonnull final com.atlassian.jira.index.Index.UpdateMode mode)
            internal Create(ICollection<Document> documents, UpdateMode mode)
            {
                if (documents == null) throw new ArgumentNullException("documents");

                Documents = documents.ToList();
                _mode = mode;
            }

            internal override void Perform(IWriter writer)
            {
                writer.AddDocuments(Documents);
            }

            internal override UpdateMode Mode()
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

            internal Update(Term term, ICollection<Document> documents, UpdateMode mode)
            {
                _delete = new Delete(term, mode);
                _create = new Create(documents, mode);
            }

            internal override void Perform(IWriter writer)
            {
                writer.UpdateDocuments(_delete.Term, _create.Documents);
            }

            internal override UpdateMode Mode()
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

            internal override void Perform(IWriter writer)
            {
                writer.UpdateDocumentConditionally(_delete.Term, _create.Documents.First(), _optimisticLockField);
            }

            internal override UpdateMode Mode()
            {
                return _create.Mode();
            }
        }

        private sealed class Optimize : Operation
        {
            internal override void Perform(IWriter writer)
            {
                writer.Optimize();
            }

            internal override UpdateMode Mode()
            {
                return UpdateMode.Batch;
            }
        }

        internal sealed class Completion : Operation
        {
            internal readonly Runnable completionJob;
            internal readonly Operation @delegate;

            public Completion(Operation @delegate, Runnable completionJob)
            {
                this.@delegate = @delegate;
                this.completionJob = completionJob;
            }

            internal override void Perform(IWriter writer)
            {
                try
                {
                    @delegate.perform(writer);
                }
                finally
                {
                    completionJob.run();
                }
            }

            internal override UpdateMode Mode()
            {
                return @delegate.Mode();
            }
        }

    }
}