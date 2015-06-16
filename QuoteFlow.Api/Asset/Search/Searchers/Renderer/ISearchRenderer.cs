using System.Collections;
using System.Collections.Generic;
using QuoteFlow.Api.Asset.Search.Searchers.Transformer;
using QuoteFlow.Api.Asset.Transport;
using QuoteFlow.Api.Jql.Query;
using QuoteFlow.Api.Models;

namespace QuoteFlow.Api.Asset.Search.Searchers.Renderer
{
    /// <summary>
    /// Handles the rendering of field search information for the QuoteFlow quote builder. 
    /// The html that is produced by these methods will create request parameters that can be processed by a
    /// <seealso cref="ISearchInputTransformer"/> to populate a SearchRequest.
    /// </summary>
    public interface ISearchRenderer
    {
        /// <summary>
        /// Used to produce an HTML input that is rendered on the JIRA issue navigator. This HTML provides the UI
        /// for searching a fields content. There will be a corresponding <seealso cref="ISearchInputTransformer"/>
        /// that will know how to transform these input parameters into JIRA search objects.
        /// </summary>
        /// <param name="user"> performing this action. </param>
        /// <param name="searchContext"> the search context of the current search request that may be participating in rendering the
        /// issue navigator. </param>
        /// <param name="fieldValuesHolder"> contains any request parameters that the HTML input may need to use to pre-populate
        /// the input (e.g. if this is the priority renderer and the search request being rendered has two priorities already
        /// selected these params will contain these request parameters). These parameters will have been populated via a
        /// call to <see cref="ISearchInputTransformer.PopulateFromQuery"/>
        /// if there is a SearchRequest involved in the rendering this IssueNavigator call. </param>
        /// <param name="displayParameters">A map of "hints" that can be passed from the caller to this code which can use these
        /// hints to alter the way it renders the HTML.</param>
        /// <returns>String that contains HTML that can be rendered on the left-hand side of the QuoteFlow asset navigator/quote builder.</returns>
        string GetEditHtml(User user, ISearchContext searchContext, IFieldValuesHolder fieldValuesHolder, IDictionary<string, object> displayParameters);

        /// <summary>
        /// Checks if the searcher should be shown in this context on the Issue Navigator.
        /// </summary>
        /// <param name="user"> performing this action. </param>
        /// <param name="searchContext"> the context of the search (i.e. projects and issue types selected). </param>
        /// <returns> true if the searcher will appear in the issue navigator, false otherwise. </returns>
        bool IsShown(User user, ISearchContext searchContext);

        /// <summary>
        /// Used to produce the HTML that displays a searchers summary information (e.g. if this is the priority searcher
        /// and a user has selected two priorities then this method will render HTML that shows something like 'priority: Major, Minor').
        /// </summary>
        /// <param name="user">The user performing this action.</param>
        /// <param name="searchContext">The search context of the current search request that may be participating in rendering the quote builder.</param>
        /// <param name="fieldValuesHolder">Contains any request parameters that the HTML input may need to use to pre-populate
        ///     the input (e.g. if this is the priority renderer and the search request being rendered has two priorities already
        ///     selected these params will contain these request parameters). These parameters will have been populated via a
        ///     call to <seealso cref="ISearchInputTransformer.PopulateFromQuery"/>
        ///     if there is a SearchRequest involved in the rendering this IssueNavigator call. </param>
        /// <param name="displayParameters"> are a map of "hints" that can be passed from the caller to this code which can use these
        ///     hints to alter the way it renders the HTML. </param>
        /// <returns> a String that contains HTML that can be rendered on the left-hand side of the issue navigator to show
        /// a SearchRequest summary. </returns>
        string GetViewHtml(User user, ISearchContext searchContext, IFieldValuesHolder fieldValuesHolder, IDictionary<string, object> displayParameters);

        /// <summary>
        /// Checks if the searchRequest object has a search representation that was created by the searcher and is
        /// used to determine if the <see cref="GetViewHtml"/>
        /// method should be called when rendering the search summary.
        /// </summary>
        /// <param name="user">The user performing this action.</param>
        /// <param name="query">Contains the search criteria used to determine if this query is relevevant to the searcher.</param>
        /// <returns>True if the query has relevant clauses to the searchers, false otherwise.</returns>
        bool IsRelevantForQuery(User user, IQuery query);
    }
}