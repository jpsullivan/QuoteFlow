using Version = Lucene.Net.Util.Version;

namespace QuoteFlow.Api.Lucene.Index
{
    /// <summary>
    /// This is the value used by QuoteFlow when it interacts with Lucene.Net classes.
    /// </summary>
    public class LuceneVersion
    {
        /// <summary>
        /// Keep the private field and the public accessor method. Otherwise, this 
        /// will be inlined as it will be considered to be a compile time constant.
        /// </summary>
        private const Version Value = Version.LUCENE_30;

        /// <summary>
        /// Gets the value used by QuoteFlow when it interacts with Apache Lucene classes. 
        /// </summary>
        /// <returns>A Version instance.</returns>
        public static Version Get()
        {
            return Value;
        }
    }
}