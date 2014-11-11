namespace QuoteFlow.Models.Assets.CustomFields.Searchers.Transformer
{
    /// <summary>
    /// Provides help for constructing clauses for custom fields from Search Input Transformers.
    /// </summary>
    public interface ICustomFieldInputHelper
    {
        /// <summary>
        /// Given the primary clause name and the field name, returns the "unique" clause name that should be used when
        /// constructing terminal clauses for this clause name. Uniqueness is calculated per user; a name could be unique
        /// for one user since he only has limited view of fields, but for another user it could be non-unique.
        /// </summary>
        /// <param name="user">The user performing the search.</param>
        /// <param name="primaryName">The primary name of a clause, e.g. <code>cf[10000]</code> or <code>project</code>.</param>
        /// <param name="fieldName">The name of the field associated to the clause, e.g. <code>My Custom Field</code> or <code>project</code> </param>
        /// <returns>
        /// The clause name which should be used in construction of terminal clauses, to guarantee that this clause
        /// refers only to the one specific field. 
        /// </returns>
        string GetUniqueClauseName(User user, string primaryName, string fieldName);
    }
}