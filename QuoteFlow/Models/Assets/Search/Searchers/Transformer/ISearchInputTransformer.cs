using System.Web.Http.ModelBinding;
using QuoteFlow.Models.Assets.Transport;
using QuoteFlow.Models.Search.Jql.Query;
using QuoteFlow.Models.Search.Jql.Query.Clause;

namespace QuoteFlow.Models.Assets.Search.Searchers.Transformer
{
    /// <summary>
    /// Used to convert input parameters as submitted by a <seealso cref="SearchRenderer"/>
    /// into the intermediate form, as stored in a <see cref="IFieldValuesHolder"/> and then
    /// from a FieldValuesHolder into an object form that is stored in the
    /// <seealso cref="ISearchRequest"/> that is used to execute a search in QuoteFlow.
    /// </summary>
    public interface ISearchInputTransformer
    {
        /// <summary>
        /// Populate <seealso cref="FieldValuesHolder"/> object with whatever values the searcher is interested in from the
        /// <seealso cref="com.atlassian.jira.issue.transport.ActionParams"/>. This transforms the "raw" request parameters
        /// into a form that the other processing methods can handle (usually a mapping of the fields name as the key
        /// and a list of the values as the value).
        /// </summary>
        /// <param name="user">The <see cref="User"/> performing this action.</param>
        /// <param name="fieldValuesHolder">The object that should have its values set by this method and that will contain
        /// any other values that have been set by other SearchInputTransformers. </param>
        /// <param name="actionParams"> params from the webwork front end that contains a String[] of values as submitted via the </param>
        void PopulateFromParams(User user, IFieldValuesHolder fieldValuesHolder, ActionParams actionParams);

        /// <summary>
        /// Adds error messages to the errors object if values in the fieldValuesHolder fails validation. This should be
        /// called once the fieldValuesHolder has been populated.
        /// </summary>
        /// <param name="user"> performing this action. </param>
        /// <param name="searchContext"> the context of the search (i.e. projects and issue types selected). </param>
        /// <param name="fieldValuesHolder"> contains values populated by the populate methods of this input transformer. </param>
        /// <param name="errors"> the ErrorCollection that contains the messages we want to display to the users. </param>
        void ValidateParams(User user, SearchContext searchContext, IFieldValuesHolder fieldValuesHolder, ModelState errors);

        /// <summary>
        /// This method transforms any query information contained in the query that is relevant to this
        /// SearchInputTransformer into the values that the HTML rendering expects. This should
        /// populate the <see cref="IFieldValuesHolder"/> from the a query information in the
        /// query.
        /// The query elements that are considered "relevant" to this method would be those that are produced by the
        /// <seealso cref="GetSearchClause(User, IFieldValuesHolder)"/> method.
        /// </summary>
        /// <param name="user">The user performing the action.</param>
        /// <param name="fieldValuesHolder">Is the object that should have its values set by this method and that will contain
        /// any other values that have been set by other SearchInputTransformers.</param>
        /// <param name="query">The search criteria used to populate the field values holder.</param>
        /// <param name="searchContext">Contains the projects and issue types that the search and filter form is restricted to.</param>
        void PopulateFromQuery(User user, IFieldValuesHolder fieldValuesHolder, Query query, SearchContext searchContext);

        /// <summary>
        /// Tells the caller whether or not the relevant clauses from the passed query can be represented on the issue
        /// navigator. Implementers of this method needs to ensure that it can represent *ALL* related clauses on the
        /// navigator, and that the clauses' structure conforms to the simple navigator structure.
        /// <p/>
        /// The method should only be concerned with the clauses related to this transformer. Other irrelevant clauses should
        /// be ignored. 
        /// </summary>
        /// <param name="user"> performing this action. </param>
        /// <param name="query"> to check if it can fit in the simple (GUI form based) issue navigator. </param>
        /// <param name="searchContext"> contains the projects and issue types that the search and filter form is restricted to </param>
        /// <returns> true if the query can be represented on navigator. </returns>
        bool DoRelevantClausesFitFilterForm(User user, IQuery query, SearchContext searchContext);

        /// <summary>
        /// Gets the portion of the Search Query that this searcher is responsible for.
        /// </summary>
        /// <param name="user"> performing this action. </param>
        /// <param name="fieldValuesHolder"> contains values populated by the searchers </param>
        /// <returns> a <seealso cref="com.atlassian.query.clause.Clause"/> that represents the users search based on the fieldValuesHolder;
        /// null if this searcher has no responsibility in the given input. </returns>
        IClause GetSearchClause(User user, IFieldValuesHolder fieldValuesHolder);
    }
}