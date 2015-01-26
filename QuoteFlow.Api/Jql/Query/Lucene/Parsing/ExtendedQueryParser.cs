//using System.Collections.Generic;
//using System.Diagnostics;
//using System.IO;
//using System.Linq;
//using Lucene.Net.Analysis;
//using Lucene.Net.Analysis.Tokenattributes;
//using Lucene.Net.Index;
//using Lucene.Net.QueryParsers;
//using Lucene.Net.Search;
//using Version = Lucene.Net.Util.Version;
//
//namespace QuoteFlow.Models.Search.Jql.Query.Lucene.Parsing
//{
//    /// <summary>
//    /// An extended version of the original Lucene <seealso cref="QueryParser"/> with the following 
//    /// set of additions:
//    /// 
//    /// - Allows the host application to reuse the field query creation algorithm and specify an 
//    /// <see cref="Analyzer analyzer"/> when overriding <seealso cref="GetFieldQuery(string, string)"/>
//    /// </summary>
//    public class ExtendedQueryParser : QueryParser
//    {
//        public ExtendedQueryParser(Version matchVersion, string f, Analyzer a) : base(matchVersion, f, a)
//        {
//        }
//
//        protected internal ExtendedQueryParser(ICharStream stream) : base(stream)
//        {
//        }
//
//        protected ExtendedQueryParser(QueryParserTokenManager tm) : base(tm)
//        {
//        }
//
//        protected override global::Lucene.Net.Search.Query GetFieldQuery(string field, string queryText)
//        {
//            return NewFieldQuery(Analyzer, field, queryText);
//        }
//
//        /// <summary>
//        /// Creates a new field query using the specified <seealso cref="Analyzer analyzer"/> instance for reuse in sub-classes.
//        /// 
//        /// <p>Contains the same logic specified in the {@code getFieldQuery} method of the {@code LuceneQueryParser} parent
//        /// class with an additional analyzer parameter so we can specify a different analyzer.</p>
//        /// 
//        /// <p>
//        ///     Backported to Lucene 3.3.0, as per the fix to
//        ///     <a href="https://issues.apache.org/jira/browse/LUCENE-2892">LUCENE-2892</a>.
//        /// </p>
//        /// <p>
//        ///     <strong>IMPORTANT:</strong>
//        ///     <ul>
//        ///          <li>Please update this algorithm to match the behaviour of
//        ///          {@code super.getFieldQuery(String field, String queryText, boolean quoted)} when updating the lucene
//        ///          version targeted by this library.
//        ///          </li>
//        ///          <li>This is part of lucene core since 4.0-ALPHA, so it is only needed for versions before that.</li>
//        ///     </ul>
//        /// </p>
//        /// </summary>
//        protected virtual global::Lucene.Net.Search.Query NewFieldQuery(Analyzer analyzer, string field, string queryText)
//        {
//            // Use the analyzer to get all the tokens, and then build a TermQuery,
//            // PhraseQuery, or nothing based on the term count
//
//            TokenStream source;
//            try
//            {
//                source = analyzer.ReusableTokenStream(field, new StringReader(queryText));
//                source.Reset();
//            }
//            catch (IOException e)
//            {
//                source = analyzer.TokenStream(field, new StringReader(queryText));
//            }
//            CachingTokenFilter buffer = new CachingTokenFilter(source);
//            CharTermAttribute termAtt = null;
//            PositionIncrementAttribute posIncrAtt = null;
//            int numTokens = 0;
//
//            bool success = false;
//            try
//            {
//                buffer.Reset();
//                success = true;
//            }
//            catch (IOException e)
//            {
//                // success==false if we hit an exception
//            }
//            if (success)
//            {
//                if (buffer.HasAttribute<CharTermAttribute>())
//                {
//                    termAtt = buffer.GetAttribute<CharTermAttribute>();
//                }
//                if (buffer.HasAttribute<PositionIncrementAttribute>())
//                {
//                    posIncrAtt = buffer.GetAttribute <PositionIncrementAttribute>();
//                }
//            }
//
//            int positionCount = 0;
//            bool severalTokensAtSamePosition = false;
//
//            if (termAtt != null)
//            {
//                try
//                {
//                    bool hasMoreTokens = buffer.IncrementToken();
//                    while (hasMoreTokens)
//                    {
//                        numTokens++;
//                        int positionIncrement = (posIncrAtt != null) ? posIncrAtt.PositionIncrement : 1;
//                        if (positionIncrement != 0)
//                        {
//                            positionCount += positionIncrement;
//                        }
//                        else
//                        {
//                            severalTokensAtSamePosition = true;
//                        }
//                        hasMoreTokens = buffer.IncrementToken();
//                    }
//                }
//                catch (IOException e)
//                {
//                    // ignore
//                }
//            }
//            try
//            {
//                // rewind the buffer stream
//                buffer.Reset();
//
//                // close original stream - all tokens buffered
//                source.Close();
//            }
//            catch (IOException e)
//            {
//                // ignore
//            }
//
//            if (numTokens == 0)
//            {
//                return null;
//            }
//            else if (numTokens == 1)
//            {
//                string term = null;
//                try
//                {
//                    bool hasNext = buffer.IncrementToken();
//                    Debug.Assert(hasNext == true);
//                    term = termAtt.ToString();
//                }
//                catch (IOException e)
//                {
//                    // safe to ignore, because we know the number of tokens
//                }
//                return NewTermQuery(new Term(field, term));
//            }
//            else
//            {
//                if (severalTokensAtSamePosition || (!quoted && !AutoGeneratePhraseQueries))
//                {
//                    if (positionCount == 1 || (!quoted && !AutoGeneratePhraseQueries))
//                    {
//                        // no phrase query:
//                        BooleanQuery q = NewBooleanQuery(positionCount == 1);
//
//                        Occur occur = positionCount > 1 && DefaultOperator == AND_OPERATOR ? Occur.MUST : Occur.SHOULD;
//
//                        for (int i = 0; i < numTokens; i++)
//                        {
//                            string term = null;
//                            try
//                            {
//                                bool hasNext = buffer.IncrementToken();
//                                Debug.Assert(hasNext == true);
//                                term = termAtt.ToString();
//                            }
//                            catch (IOException e)
//                            {
//                                // safe to ignore, because we know the number of tokens
//                            }
//
//                            Query currentQuery = NewTermQuery(new Term(field, term));
//                            q.Add(currentQuery, occur);
//                        }
//                        return q;
//                    }
//                    else
//                    {
//                        // phrase query:
//                        MultiPhraseQuery mpq = NewMultiPhraseQuery();
//                        mpq.Slop = phraseSlop;
//                        IList<Term> multiTerms = new List<Term>();
//                        int position = -1;
//                        for (int i = 0; i < numTokens; i++)
//                        {
//                            string term = null;
//                            int positionIncrement = 1;
//                            try
//                            {
//                                bool hasNext = buffer.IncrementToken();
//                                Debug.Assert(hasNext == true);
//                                term = termAtt.ToString();
//                                if (posIncrAtt != null)
//                                {
//                                    positionIncrement = posIncrAtt.PositionIncrement;
//                                }
//                            }
//                            catch (IOException e)
//                            {
//                                // safe to ignore, because we know the number of tokens
//                            }
//
//                            if (positionIncrement > 0 && multiTerms.Count > 0)
//                            {
//                                if (enablePositionIncrements)
//                                {
//                                    mpq.Add(multiTerms.ToArray(), position);
//                                }
//                                else
//                                {
//                                    mpq.Add(multiTerms.ToArray());
//                                }
//                                multiTerms.Clear();
//                            }
//                            position += positionIncrement;
//                            multiTerms.Add(new Term(field, term));
//                        }
//                        if (enablePositionIncrements)
//                        {
//                            mpq.Add(multiTerms.ToArray(), position);
//                        }
//                        else
//                        {
//                            mpq.Add(multiTerms.ToArray());
//                        }
//                        return mpq;
//                    }
//                }
//                else
//                {
//                    PhraseQuery pq = NewPhraseQuery();
//                    pq.Slop = phraseSlop;
//                    int position = -1;
//
//
//                    for (int i = 0; i < numTokens; i++)
//                    {
//                        string term = null;
//                        int positionIncrement = 1;
//
//                        try
//                        {
//                            bool hasNext = buffer.IncrementToken();
//                            Debug.Assert(hasNext == true);
//                            term = termAtt.ToString();
//                            if (posIncrAtt != null)
//                            {
//                                positionIncrement = posIncrAtt.PositionIncrement;
//                            }
//                        }
//                        catch (IOException e)
//                        {
//                            // safe to ignore, because we know the number of tokens
//                        }
//
//                        if (enablePositionIncrements)
//                        {
//                            position += positionIncrement;
//                            pq.Add(new Term(field, term), position);
//                        }
//                        else
//                        {
//                            pq.Add(new Term(field, term));
//                        }
//                    }
//                    return pq;
//                }
//            }
//        }
//    }
//}