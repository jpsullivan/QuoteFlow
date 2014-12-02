using System.Collections.Generic;
using System.Linq;
using QuoteFlow.Models.Assets.Fields;
using QuoteFlow.Models.Assets.Index.Indexers;
using QuoteFlow.Models.Assets.Search.Searchers;
using QuoteFlow.Models.Search.Jql.Clauses;

namespace QuoteFlow.Models.Assets.Search
{
    /// <summary>
    /// Object used by the field to indicate how it can be searched and indexed. 
    /// The field controls how QuoteFlow indexes an issue by specifying a list of <seealso cref="IFieldIndexer"/>s. 
    /// Each JQL clause in QuoteFlow is represented by a <seealso cref="ClauseRegistration"/>.
    /// It consists of a set of JQL names and the <seealso cref="IClauseHandler"/> that can be used to process
    /// those JQL names in a query.
    /// 
    /// Each field *may* have one <seealso cref="IAssetSearcher{T}"/> that uses a list of JQL
    /// clauses to create a JQL search. This is specified in the <seealso cref="SearcherRegistration"/>
    /// on the SearchHandler. QuoteFlow will keep the association between the AssetSearcher 
    /// and JQL clauses (ClauseHandler) to perform the mapping from JQL to the GUI version of 
    /// Asset Navigator. Listing ClauseHandlers in the SearcherRegistration that are not related to 
    /// the AssetSearcher may result QuoteFlowe falsely determining that a JQL query cannot be 
    /// represented on the navigator GUI. Note that the JQL clauses listed in the SearcherRegistration 
    /// will also be available directly in JQL.
    /// 
    /// The field may also have another set of JQL clauses that can be used against the field but are 
    /// not associated with any AssetSearcher. The clauses are held in the list of ClauseRegistrations 
    /// on the ClauseHandler itself. These JQL clauses cannot be represented on navigtor as they have 
    /// no AssetSearcher.
    /// 
    /// The same ClauseHandler should not be listed in both the SearcherRegistration and the SearchHandler 
    /// directly. Doing so could result in doing QuoteFlow performing the same search twice.
    /// </summary>
    public sealed class SearchHandler
    {
        /// <summary>
        /// The list of indexers that QuoteFlow will use to index the field.
        /// </summary>
        public IEnumerable<IFieldIndexer> FieldIndexers { get; set; }

        /// <summary>
        /// The list of JQL clauses provided for searching. These JQL clauses are not associated with any 
        /// <see cref="IAssetSearcher" /> and as such cannot be represented on the GUI version of the Asset Navigator.
        /// </summary>
        public IEnumerable<ClauseRegistration> ClauseRegistrations { get; set; }

        /// <summary>
        /// The <see cref="SearcherRegistration"/> provided for searching.
        /// </summary>
        public SearcherRegistration SearcherReg { get; set; }

        /// <summary>
        /// Create a new handler.
        /// </summary>
        /// <param name="fieldIndexers">The indexers to associate with the handler. May not be null.</param>
        /// <param name="searcherRegistration">The registration to associate with the handler. May be null.</param>
        /// <param name="clauseRegistrations">The JQL clauses to associate with the chanler. May not be null.</param>
        public SearchHandler(IEnumerable<IFieldIndexer> fieldIndexers, SearcherRegistration searcherRegistration, IEnumerable<ClauseRegistration> clauseRegistrations)
        {
            ClauseRegistrations = clauseRegistrations;
            FieldIndexers = fieldIndexers;
            SearcherReg = searcherRegistration;
        }

        /// <summary>
        /// Create a new handler with the passed <seealso cref="IFieldIndexer"/>s and <see cref="SearcherRegistration"/>.
        /// Same as calling {@code this(fieldIndexers, searcherRegistration,Collections.<ClauseRegistration>emptyList());}
        /// </summary>
        /// <param name="fieldIndexers">The indexers to associate with the handler.</param>
        /// <param name="searcherRegistration">The searcher (and its associated clauses) to associate with the handler. May be null.</param>
        public SearchHandler(IEnumerable<IFieldIndexer> fieldIndexers, SearcherRegistration searcherRegistration)
            : this(fieldIndexers, searcherRegistration, Enumerable.Empty<ClauseRegistration>())
        {
        }

        /// <summary>
        /// Holds the link between an <seealso cref="AssetSearcher"/> and the JQL clauses (as
        /// <seealso cref="ClauseRegistration"/>s) that it uses in the background to implement searching. 
        /// This relationship is kept within QuoteFlow so that is can perform the JQL to Asset navigator
        /// translation.
        /// </summary>
        public class SearcherRegistration
        {
            public IAssetSearcher<ISearchableField> AssetSearcher { get; set; }
            public IEnumerable<ClauseRegistration> ClauseHandlers { get; set; }

            public SearcherRegistration(IAssetSearcher<ISearchableField> searcher, IClauseHandler clauseHandler)
                : this(searcher, new ClauseRegistration(clauseHandler))
            {
            }

            public SearcherRegistration(IAssetSearcher<ISearchableField> searcher, IEnumerable<ClauseRegistration> clauseHandlers)
            {
                AssetSearcher = searcher;
                ClauseHandlers = clauseHandlers;
            }

            public SearcherRegistration(IAssetSearcher<ISearchableField> searcher, ClauseRegistration clauseRegistration)
                : this(searcher, new List<ClauseRegistration> { clauseRegistration })
            {
            }
        }

        /// <summary>
        /// Represents a JQL clause and how to process it. Fields may use these objects to register 
        /// new JQL clauses within QuoteFlow.
        /// </summary>
        public class ClauseRegistration
        {
            public IClauseHandler Handler { get; set; }

            public ClauseRegistration(IClauseHandler handler)
            {
                Handler = handler;
            }
        }
    }
}