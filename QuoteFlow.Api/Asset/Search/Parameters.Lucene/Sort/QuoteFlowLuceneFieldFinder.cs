using System;
using System.Collections.Generic;
using System.IO;
using Lucene.Net.Index;

namespace QuoteFlow.Api.Asset.Search.Parameters.Lucene.Sort
{
    /// <summary>
    /// This used to be a cache of values but it was found that it consumed a hell of a lot of memory for no benefit
    /// (JRA-10111). So the cache.put was never called.
    /// <p/>
    /// This has been refactored into a "finder" of terms values for fields within documents.
    /// </summary>
    public class QuoteFlowLuceneFieldFinder
    {
        private static readonly QuoteFlowLuceneFieldFinder FieldFinder = new QuoteFlowLuceneFieldFinder();

        public static QuoteFlowLuceneFieldFinder Instance { get { return FieldFinder; } }

        /// <summary>
        /// This is used to retrieve values from the Lucence index. It returns an array that is the same 
        /// size as the number of documents in the reader and will have all null values if the field is 
        /// not present, otherwise it has the values of the field within the document.
        /// </summary>
        /// <param name="reader">The Lucence index reader.</param>
        /// <param name="field">The name of the field to find.</param>
        /// <param name="mappedSortComparator">The MappedSortComparator that we are acting on behalf of.</param>
        /// <returns>A non-null array of values, which may contain null values.</returns>
        /// <exception cref="IOException">If things dont play out well.</exception>
        public virtual object[] GetCustom(IndexReader reader, string field, MappedSortComparator mappedSortComparator)
        {
            string internedField = String.Intern(field);
            object[] retArray = new object[reader.MaxDoc];
            if (retArray.Length > 0)
            {
                TermDocs termDocs = reader.TermDocs();
                TermEnum termEnum = reader.Terms(new Term(internedField, ""));
                try
                {
                    // if we dont have a term in any of the documents
                    // then an array of null values is what we should return
                    if (termEnum.Term == null)
                    {
                        return retArray;
                    }
                    var comparator = mappedSortComparator.Comparator;
                    do
                    {
                        Term term = termEnum.Term;
                        // Because Lucence interns fields for us this is a bit quicker
                        //noinspection StringEquality
                        if (term.Field != internedField)
                        {
                            // if the next term is not our field then none of those
                            // terms are present in the set of documents and hence
                            // an array of null values is what we should return
                            break;
                        }
                        object termval = mappedSortComparator.GetComparable(term.Text);
                        termDocs.Seek(termEnum);
                        while (termDocs.Next())
                        {
                            object currentValue = retArray[termDocs.Doc];
                            //only replace the value if it is earlier than the current value
                            //noinspection unchecked
                            if (currentValue == null || comparator.Compare(termval, currentValue) < 1)
                            {
                                retArray[termDocs.Doc] = termval;
                            }
                        }
                    }
                    while (termEnum.Next());
                }
                finally
                {
                    termDocs.Dispose();
                    termEnum.Dispose();
                }
            }
            return retArray;
        }

        /// <summary>
        /// This method is used to retrieve term values form the Lucene index. It returns a list of the text
        /// representation of the terms that are present in a field - this list will be empty if a field has no terms.
        /// </summary>
        /// <param name="reader">The Lucence index reader.</param>
        /// <param name="field">The name of the field to find.</param>
        /// <returns>A list of strings that represent the term values for a specified field.</returns>
        /// <exception cref="IOException"></exception>
        public virtual IList<string> GetTermValuesForField(IndexReader reader, string field)
        {
            var terms = new List<string>();
            if (field != null)
            {
                string internedField = String.Intern(field);
                TermEnum termEnum = reader.Terms(new Term(internedField, ""));
                do
                {
                    Term currTerm = termEnum.Term;
                    // Because Lucence interns fields for us this is a bit quicker
                    //noinspection StringEquality
                    if (currTerm == null || currTerm.Field != internedField)
                    {
                        break;
                    }
                    terms.Add(currTerm.Text);
                } while (termEnum.Next());
            }
            return terms;
        }

        /// <summary>
        /// This method checks if a field contains a term.  
        /// </summary>
        /// <param name="reader">The Lucence index reader.</param>
        /// <param name="field">The name of the field to test.</param>
        /// <param name="term">The term you are looking for in the field.</param>
        /// <returns>A boolean indicating ehether the field contains this term.</returns>
        /// <exception cref="IOException"></exception>
        public virtual bool DoesFieldContainTerm(IndexReader reader, string field, string term)
        {
            if (field != null)
            {
                string internedField = String.Intern(field);
                Term searchTerm = new Term(field, term);
                TermEnum terms = reader.Terms(searchTerm);
                return (searchTerm.Equals(terms.Term));
            }
            return false;
        }

        /// <summary>
        /// For each document in the index, it returns an array of string collections for each matching term.
        /// Uses the <seealso cref="DefaultMatchHandler"/>.
        /// </summary>
        /// <param name="reader">The index to read.</param>
        /// <param name="field">The field to check the documents for.</param>
        /// <returns>An array of string collections for each term for each document.</returns>
        /// <exception cref="IOException"> if things dont play out well. </exception>
        public virtual ICollection<string>[] GetMatches(IndexReader reader, string field)
        {
            int maxDoc = reader.MaxDoc;
            var handler = new DefaultMatchHandler(maxDoc);
            GetMatches(reader, field, handler);
            return handler.Results;
        }

        /// <summary>
        /// For each document in the index, it returns an array of string collections for each matching term.
        /// Uses the <seealso cref="SingleValueMatchHandler"/>, so any collections that are returned are guaranteed
        /// to contain exactly one (possibly <tt>null</tt>) value in them.
        /// </summary>
        /// <param name="reader">The index to read.</param>
        /// <param name="field">The field to check the documents for.</param>
        /// <returns>An array of string collections for each term for each document.</returns>
        /// <exception cref="IOException">If things dont play out well.</exception>
        public virtual IList<string>[] GetUniqueMatches(IndexReader reader, string field)
        {
            int maxDoc = reader.MaxDoc;
            var handler = new SingleValueMatchHandler(maxDoc);
            GetMatches(reader, field, handler);
            return handler.Results;
        }

        /// <summary>
        /// For each document that has at least one value defined for the specified field,
        /// invokes <seealso cref="MatchHandler.HandleMatchedDocument(int, String)"/> with the document
        /// index and the field value as the argument.
        /// </summary>
        /// <param name="reader">The index to read.</param>
        /// <param name="field">The field to check the documents for.</param>
        /// <param name="handler">A handler that will be invoked for each matching term.</param>
        /// <exception cref="IOException">If things dont play out well.</exception>
        public virtual void GetMatches(IndexReader reader, string field, IMatchHandler handler)
        {
            if (reader.MaxDoc == 0)
            {
                return;
            }
            string internedField = String.Intern(field);
            TermDocs termDocs = reader.TermDocs();
            try
            {
                TermEnum termEnum = reader.Terms(new Term(internedField, ""));
                try
                {
                    if (termEnum.Term == null)
                    {
                        return;
                    }
                    do
                    {
                        Term term = termEnum.Term;
                        // Because Lucence interns fields for us this is a bit quicker
                        //noinspection StringEquality
                        if (term.Field != internedField)
                        {
                            break;
                        }
                        string termval = term.Text;
                        termDocs.Seek(termEnum);
                        while (termDocs.Next())
                        {
                            handler.HandleMatchedDocument(termDocs.Doc, termval);
                        }
                    }
                    while (termEnum.Next());
                }
                finally
                {
                    termEnum.Dispose();
                }
            }
            finally
            {
                termDocs.Dispose();
            }
        }
    }
}