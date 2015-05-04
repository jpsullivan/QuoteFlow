using System.Collections.Generic;

namespace QuoteFlow.Api.Asset.Search
{
    /// <summary>
    /// Implementations of this interface are used to sort Lucene search results of Issue Documents.
    /// <p/>
    /// <strong>NOTE</strong>: instances of this interface are <strong>cached</strong> by Lucene and are
    /// <strong>REUSED</strong> to sort multiple Lucene search results. The Comparator returned by the
    /// <see cref="Comparator"/> method could be used by Lucene from multiple threads at once.
    /// <p/>
    /// Therefore, the implementations of this interface <strong>MUST</strong> implement the <see cref="object#equals(Object)"/>
    /// and <see cref="GetHashCode"/> methods correctly to ensure that Lucene can find the implementations of this class
    /// in its cache and reuse it, rather than make the cache grow indefinitely. (Unfortunately the Lucene cache is rather
    /// primitive at the moment, and is not bound).
    /// <p/>
    /// Also, ensure that the <see cref="Comparator"/> returned by the <see cref="Comparator()"/> method is <strong>thread
    /// safe</strong>.
    /// <p/>
    /// As instances of this and the <see cref="Comparator"/> returned by this object are cached and reused by Lucene to sort
    /// multiple search results, the best thing to do is to ensure the implementations of this interface and the
    /// <see cref="Comparator"/> that is returned <strong>are immutable</strong> and that the <see cref="Equals(object)"/> and
    /// <see cref="GetHashCode()"/> methods respect the state of the object.
    /// </summary>
    public interface ILuceneFieldSorter<T>
    {
        /// <summary>
        /// Get the constant that this field is indexed with. 
        /// </summary>
        string DocumentConstant { get; }

        /// <summary>
        /// Convert the lucene document field back to the object that you wish to use to display it.
        /// <para>
        /// eg. '1000' -> Version 1.
        /// </para>
        /// <para>
        /// This does the reverse of what <see cref="IssueDocument"/> does.
        /// </para>
        /// <para>
        /// For custom fields, the return value will be passed to
        /// <see cref="CustomFieldSearcherModuleDescriptor#StatHtml(ICustomField, Object, String)"/>
        /// 
        /// </para>
        /// </summary>
        /// <param name="documentValue">   The value of the field in the lucene index </param>
        /// <returns>  The value that will be passed to the display </returns>
        T GetValueFromLuceneField(string documentValue);

        /// <summary>
        /// A comparator that can be used to order objects returned by <see cref="#getValueFromLuceneField(String)"/>.
        /// <p/>
        /// The Comparator <strong>must</strong> be reentrant as it could be used by Lucene from multiple threads at once.
        /// </summary>
        IComparer<T> Comparator { get; }

        /// <summary>
        /// As this object is used as a key in a cache, this method <strong>must</strong> be provided and respect all internal state.
        /// </summary>
        bool Equals(object obj);

        /// <summary>
        /// As this object is used as a key in a cache, this method <strong>must</strong> be provided and respect all internal state.
        /// </summary>
        int GetHashCode();
    }
}