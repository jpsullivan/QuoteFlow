using System.Collections.Generic;
using QuoteFlow.Models.Search.Jql.Context;

namespace QuoteFlow.Models.Assets.Fields
{
    public interface IFieldManager
    {
        /// <summary>
        /// Get a field by its id.
        /// </summary>
        /// <param name="id">An <seea cref="AssetFieldConstants"/> constant, or custom field key (eg. "customfield_10010").</param>
        /// <returns>the <see cref="IField"/></returns>
        IField GetField(string id);

        bool IsCustomField(string id);

        bool IsCustomField(IField field);

        /// <summary>
        /// Get a CustomField by its text key (eg 'customfield_10000'). 
        /// </summary>
        /// <param name="id"> Eg. 'customfield_10000' </param>
        /// <returns>The <seealso cref="CustomField"/> or null if not found.</returns>
        ICustomField GetCustomField(string id);

        ISet<IOrderableField> OrderableFields { get; }

        ISet<INavigableField> NavigableFields { get; }

        /// <summary>
        /// Invalidates <em>all field-related caches</em> in JIRA.
        /// <font color="red"><h1>WARNING</h1></font>
        /// This method invalidates a whole lot of JIRA caches, which means that JIRA performance significantly degrades
        /// after this method has been called. For this reason, you should <b>avoid calling this method at all costs</b>.
        /// <p/>
        /// The correct approach to invalidate the cache entries is to do it in the "store" inside the {@code FooStore.updateFoo()}
        /// method, where you can invalidate a <b>single</b> cache entry. If the cache lives in another class then the store
        /// should raise a {@code FooUpdatedEvent} which that class can listen to in order to keep its caches up to date.
        /// <p/>
        /// If you add any calls to this method in JIRA I will hunt you down and subject you to a Spanish inquisition.
        /// </summary>
        void Refresh();

        ISet<IField> UnavailableFields { get; }

        /// <summary>
        /// Gets all the available fields that the user can see, this is providing no context scope.
        /// </summary>
        /// <param name="user"> the remote user. </param>
        /// <returns> a set of NavigableFields that can be show because their visibility/configuration fall within what the
        /// user can see.
        /// </returns>
        /// <exception cref="FieldException"> thrown if there is a problem looking up the fields </exception>
        ISet<INavigableField> GetAvailableNavigableFieldsWithScope(User user);

        /// <summary>
        /// Gets all the available fields within the defined scope of the QueryContext.
        /// </summary>
        /// <param name="user"> the user making the request </param>
        /// <param name="queryContext"> the context of the search request. </param>
        /// <returns> a set of NavigableFields that can be show because their visibility/configuration fall within the specified
        /// context
        /// </returns>
        /// <exception cref="FieldException"> thrown if there is a problem looking up the fields </exception>
        ISet<INavigableField> GetAvailableNavigableFieldsWithScope(User user, IQueryContext queryContext);

        /// <summary>
        /// Retrieves custom fields in scope for the given issue
        /// </summary>
        /// <param name="remoteUser"> Remote User </param>
        /// <param name="issue"> Issue </param>
        /// <exception cref="FieldException"> if cannot retrieve the projects the user can see, or if cannot retrieve
        ///                        the field layouts for the viewable projects </exception>
        /// <returns> custom fields in scope for the given issue </returns>
        ISet<ICustomField> GetAvailableCustomFields(User remoteUser, Asset asset);

        ISet<INavigableField> AllAvailableNavigableFields { get; }

        ISet<INavigableField> GetAvailableNavigableFields(User remoteUser);

        /// <summary>
        /// Return all the searchable fields in the system. This set will included all defined custom fields.
        /// </summary>
        /// <returns> the set of all searchable fields in the system. </returns>
        ISet<ISearchableField> AllSearchableFields { get; }

        /// <summary>
        /// Return all the searchable systems fields. This set will *NOT* include defined custom fields.
        /// </summary>
        /// <returns> the set of all searchable systems fields defined. </returns>
        IEnumerable<ISearchableField> SystemSearchableFields { get; }

        /// <summary>
        /// Retrieve the IssueType system Field. </summary>
        /// <returns> the IssueType system Field. </returns>
        IAssetTypeField IssueTypeField { get; }

        /// <summary>
        /// Retrieve the Project system Field. </summary>
        /// <returns> the Project system Field. </returns>
        ICatalogField CatalogField { get; }
    }
}