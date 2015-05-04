using Lucene.Net.Search;
using QuoteFlow.Api.Lucene.Index;
using QuoteFlow.Core.Index;

namespace QuoteFlow.Core.Lucene.Index
{
    /// <summary>
    /// An <see cref="QuoteFlow.Api.Lucene.Index"/> is where data is stored for fast retrieval. The
    /// <see cref="QuoteFlow.Api.Lucene.Index"/> itself has <see cref="Operation operations"/> performed on it to
    /// update it. The index is held by a <see cref="Manager"/> where you can access a
    /// <see cref="IndexSearcher"/> that reflects the latest update that has
    /// completed.
    /// 
    /// Note: in order to guarantee that an <see cref="IndexSearcher"/> returned from
    /// <see cref="QuoteFlow.Api.Lucene.Index.Manager#openSearcher()"/> contains a particular <see cref="IOperation"/>
    /// that is <see cref="Perform(Operation{DelegateType}) performed"/>, the <see cref="Result"/> must be
    /// <see cref="IIndex.Result.Await() waited on"/>.
    /// </summary>
    public interface IIndex
    {
        /// <summary>
        /// Perform an <see cref="Operation"/> on the index.
        /// </summary>
        /// <param name="operation"> the work to do. </param>
        /// <returns> a Result object </returns>
        IIndexResult Perform(Operation operation);
    }
}