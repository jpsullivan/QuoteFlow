using System.Collections.Generic;
using System.Linq;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Moq;
using QuoteFlow.Api.Lucene.Index;
using QuoteFlow.Core.Index;
using QuoteFlow.Core.Lucene.Index;
using Xunit;

namespace QuoteFlow.Core.Tests.Index
{
    public class OperationsTests
    {
        [Fact]
        public void TestDelete()
        {
            var term = new Term("one", "delete");
            var delete = Operations.NewDelete(term, UpdateMode.Batch);

            var mockWriter = new Mock<IWriter>();
            mockWriter
                .Setup(x => x.DeleteDocuments(It.IsAny<Term>()))
                .Callback((Term t) => Assert.Same(term, t));

            delete.Perform(mockWriter.Object);
        }

        [Fact]
        public void TestCreate()
        {
            var document = new Document();
            var create = Operations.NewCreate(document, UpdateMode.Batch);

            var mockWriter = new Mock<IWriter>();
            mockWriter
                .Setup(x => x.AddDocuments(It.IsAny<IEnumerable<Document>>()))
                .Callback((IEnumerable<Document> documents) => documents.ToList().ForEach(d => Assert.Same(document, d)));

            create.Perform(mockWriter.Object);
        }

        [Fact]
        public void TestCreateMany()
        {
            var documents = new List<Document> {new Document(), new Document()};
            var create = Operations.NewCreate(documents, UpdateMode.Batch);

            var mockWriter = new Mock<IWriter>();
            mockWriter
                .Setup(x => x.AddDocuments(It.IsAny<IEnumerable<Document>>()))
                .Callback((IEnumerable<Document> docs) => Assert.Equal(documents, docs));

            create.Perform(mockWriter.Object);
        }

        [Fact]
        public void TestUpdate()
        {
            var term = new Term("one", "delete");
            var document = new Document();
            var delete = Operations.NewUpdate(term, document, UpdateMode.Batch);

            var mockWriter = new Mock<IWriter>();
            mockWriter
                .Setup(x => x.UpdateDocuments(It.IsAny<Term>(), It.IsAny<IEnumerable<Document>>()))
                .Callback((Term t, IEnumerable<Document> docs) =>
                {
                    Assert.Same(term, t);
                    Assert.Equal(new List<Document> { document }, docs);
                });

            delete.Perform(mockWriter.Object);
        }

        [Fact]
        public void TestUpdateMany()
        {
            var term = new Term("one", "delete");
            var documents = new List<Document> { new Document(), new Document() };
            var delete = Operations.NewUpdate(term, documents, UpdateMode.Batch);

            var mockWriter = new Mock<IWriter>();
            mockWriter
                .Setup(x => x.UpdateDocuments(It.IsAny<Term>(), It.IsAny<IEnumerable<Document>>()))
                .Callback((Term t, IEnumerable<Document> docs) =>
                {
                    Assert.Same(term, t);
                    Assert.Equal(documents, docs);
                });

            delete.Perform(mockWriter.Object);
        }
    }
}