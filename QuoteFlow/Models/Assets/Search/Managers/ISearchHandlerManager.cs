using System.Collections.Generic;
using QuoteFlow.Models.Assets.Fields;
using QuoteFlow.Models.Assets.Search.Searchers;
using QuoteFlow.Models.Search.Jql.Clauses;
using QuoteFlow.Models.Search.Jql.Values;

namespace QuoteFlow.Models.Assets.Search.Managers
{
    /// <summary>
    /// Manager that holds all references to search related information in QuoteFlow.
    /// </summary>
    public interface ISearchHandlerManager
    {
        /// <summary>
		/// Get searchers that are applicable for a given context. This is found through the {@link
		/// com.atlassian.jira.issue.search.searchers.AssetSearcher#getSearchRenderer()#isShown(com.atlassian.jira.issue.search.SearchContext)}
		/// method.
		/// </summary>
		/// <param name="searcher">The user performing this action.</param>
		/// <param name="context">The context for the list of searchers. Must not be null.</param>
		/// <returns>Collection of <see cref="IAssetSearcher"/>.</returns>
		ICollection<IAssetSearcher<ISearchableField>> GetSearchers(User searcher, ISearchContext context);

        /// <summary>
        /// Return all the active searchers in QuoteFlow. It will not return the searchers unless 
        /// they are associated with a field.
        /// </summary>
        /// <returns>All the searchers in QuoteFlow.</returns>
        ICollection<IAssetSearcher<ISearchableField>> GetAllSearchers();

		/// <summary>
		/// Get all searcher groups. Note that the <see cref="SearcherGroup"/> will
		/// still appear even if no <seealso cref="IAssetSearcher"/> are shown for the group.
		/// </summary>
		/// <returns>Collection of <see cref="SearcherGroup"/> </returns>
		ICollection<SearcherGroup> SearcherGroups {get;}

		/// <summary>
		/// Get a searcher by the searchers name.
		/// </summary>
		/// <param name="id">The string identifier returned by <see cref="IAssetSearcher.GetSearchInformation()#getId()"/>.</param>
		/// <returns>The searcher matching the id, null if none is found. </returns>
        IAssetSearcher<ISearchableField> GetSearcher(string id);

		/// <summary>
		/// Refreshes the <seealso cref="SearchHandlerManager"/>.
		/// </summary>
		void Refresh();

		/// <summary>
		/// Return a collection of <see cref="ClauseHandler"/>s registered against the passed JQL clause
		/// name. This will only return the handlers that the user has permission to see as specified by the {@link
		/// ClausePermissionHandler.HasPermissionToUseClause(User)} method. The reason this is returning 
		/// a collection is that custom fields can have the same JQL clause name and therefore resolve to 
		/// multiple clause handlers, this will never be the case for System fields, we don't allow it!
		/// </summary>
		/// <param name="user">The user that will be used to perform a permission check.</param>
		/// <param name="jqlClauseName">The clause name to search for.</param>
		/// <returns>
		/// A collection of ClauseHandler that are associated with the passed JQL clause name. 
		/// An empty collection will be returned to indicate failure.
		/// </returns>
        IEnumerable<IClauseHandler> GetClauseHandler(User user, string jqlClauseName);

		/// <summary>
		/// Return a collection of <see cref="ClauseHandler"/>s registered against the passed JQL clause
		/// name. This will return all available handlers, regardless of permissions. The reason this is 
		/// returning a collection is that custom fields can have the same JQL clause name and therefore 
		/// resolve to multiple clause handlers, this will never be the case for System fields, we don't 
		/// allow it!
		/// </summary>
		/// <param name="jqlClauseName">The clause name to search for. </param>
		/// <returns>
		/// A collection of ClauseHandler that are associated with the passed JQL clause name. An empty 
		/// collection will be returned to indicate failure.
		/// </returns>
        IEnumerable<IClauseHandler> GetClauseHandler(string jqlClauseName);

		/// <summary>
		/// Get the <seea cref="ClauseNames"/> associated with the provided field name.
		/// 
		/// A collection can be returned because it is possible for multiple clause handlers to register against the same
		/// field.
		/// </summary>
		/// <param name="fieldId">The <see cref="IField.Id"/>.</param>
		/// <returns>
		/// The <see cref="ClauseNames"/> associated with the provided field name. Empty collection
		/// is returned when the field has no JQL names (i.e. no clause handlers) associated with it. 
		/// </returns>
		ICollection<ClauseNames> GetJqlClauseNames(string fieldId);

		/// <summary>
		/// Gets the field ids that are associated with the provided jqlClauseName. The reason this returns a collection is
		/// that custom fields can have the same JQL clause name and therefore resolve to multiple field ids. This will only
		/// return the fields associated with clause handlers that the user has permission to see as specified by the {@link
		/// com.atlassian.jira.jql.permission.ClausePermissionHandler#hasPermissionToUseClause(com.atlassian.crowd.embedded.api.User)}
		/// method.
		/// </summary>
		/// <param name="searcher"> that will be used to perform a permission check.</param>
		/// <param name="jqlClauseName"> the clause name to find the field id for.</param>
		ICollection<string> GetFieldIds(User searcher, string jqlClauseName);

		/// <summary>
		/// Gets the field ids that are associated with the provided jqlClauseName. The reason this returns a collection is
		/// that custom fields can have the same JQL clause name and therefore resolve to multiple field ids.
		/// </summary>
		/// <param name="jqlClauseName"> the clause name to find the field id for. </param>
		/// <returns> the field ids that are associated with the provided jqlClauseName, empty collection if not found </returns>
		ICollection<string> GetFieldIds(string jqlClauseName);

		/// <summary>
		/// Get all the available clause names that the searcher can see.
		/// </summary>
		/// <param name="searcher">The searcher that will be used to perform a permission check.</param>
		/// <returns>
		/// The <see cref="ClauseNames"/> visible to the user. Empty collection is returned 
		/// when the can see no clauses.
		/// </returns>
		ICollection<ClauseNames> GetVisibleJqlClauseNames(User searcher);

		/// <summary>
		/// Get all the available clause handlers that the searcher can see.
		/// </summary>
		/// <param name="searcher">That will be used to perform a permission check.</param>
		/// <returns>
		/// The <see cref="IClauseHandler"/> visible to the user. Empty collection is returned 
		/// when the can see no clauses. 
		/// </returns>
		ICollection<IClauseHandler> getVisibleClauseHandlers(User searcher);

		/// <summary>
		/// Return a collection of <see cref="IAssetSearcher"/>s registered against the passed JQL 
		/// clause name. This will only return the IssueSearchers that the user has permission to 
		/// see as specified by the <see cref="SearchRenderer#isShown(User, SearchContext)"/> method.
		/// </summary>
		/// <param name="user">That will be used to perform a permission check.</param>
		/// <param name="jqlClauseName">The clause name to search for.</param>
		/// <returns>
		/// A collection of AssetSearchers that are associetd with the passed JQL clause name. 
		/// An empty collection will be returned to indicate failure.
		/// </returns>
		ICollection<IAssetSearcher<ISearchableField>> GetSearchersByClauseName(User user, string jqlClauseName);

    }
}