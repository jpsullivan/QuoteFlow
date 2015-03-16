using Lucene.Net.Search;
using QuoteFlow.Api.Lucene.Index;
using QuoteFlow.Core.Index;

namespace QuoteFlow.Core.Lucene.Index
{
    /// <summary>
    /// An <seealso cref="QuoteFlow.Api.Lucene.Index"/> is where data is stored for fast retrieval. The
    /// <seealso cref="QuoteFlow.Api.Lucene.Index"/> itself has <seealso cref="Operation operations"/> performed on it to
    /// update it. The index is held by a <seealso cref="Manager"/> where you can access a
    /// <seealso cref="IndexSearcher"/> that reflects the latest update that has
    /// completed.
    /// 
    /// Note: in order to guarantee that an <seealso cref="IndexSearcher"/> returned from
    /// <seealso cref="QuoteFlow.Api.Lucene.Index.Manager#openSearcher()"/> contains a particular <seealso cref="IOperation"/>
    /// that is <seealso cref="Perform(Operation{DelegateType}) performed"/>, the <seealso cref="Result"/> must be
    /// <seealso cref="IIndex.Result.Await() waited on"/>.
    /// </summary>
    public interface IIndex
    {
        /// <summary>
        /// Perform an <seealso cref="Operation"/> on the index.
        /// </summary>
        /// <param name="operation"> the work to do. </param>
        /// <returns> a Result object </returns>
        IIndexResult Perform(IndexOperation operation);
    }
}