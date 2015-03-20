//using Lucene.Net.Analysis;
//using Lucene.Net.Util;
//
//namespace QuoteFlow.Api.Infrastructure.Lucene
//{
//    /// <summary>
//    /// This attribute can be used to mark a token as a keyword. Keyword aware
//    /// <seealso cref="TokenStream"/>s can decide to modify a token based on the return value
//    /// of <seealso cref="IsKeyword()"/> if the token is modified. Stemming filters for
//    /// instance can use this attribute to conditionally skip a term if
//    /// <seealso cref="IsKeyword()"/> returns <code>true</code>.
//    /// </summary>
//    public interface IKeywordAttribute : IAttribute
//    {
//        bool Keyword { get; set; }
//
//        bool IsKeyword();
//    }
//}