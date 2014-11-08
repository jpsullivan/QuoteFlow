using System.Collections.Generic;

namespace QuoteFlow.Models.Search.Jql.Resolver
{
    /// <summary>
    /// Looks up domain objects from the database.
    /// </summary>
    public interface INameResolver<T>
    {
        /// <summary>
        /// Returns the list of ids of T objects that have the given name. Names may be unique but often are not, hence
        /// the List return type.
        /// </summary>
        /// <param name="name">The name of the T.</param>
        /// <returns>All IDs of objects matching the name or the empty list on name lookup failure.</returns>
        List<string> GetIdsFromName(string name);

        /// <summary>
        /// Returns true if the name would resolve to a domain object.
        /// </summary>
        /// <param name="name">The addressable name.</param>
        /// <returns>True only if the name resolves to a domain object in the database.</returns>
        bool NameExists(string name);

        /// <summary>
        /// Returns true if the id would resolve to a domain object.
        /// </summary>
        /// <param name="id">The primary key.</param>
        /// <returns>True only if the id resolves to a domain object in the database.</returns>
        bool IdExists(int id);

        /// <summary>
        /// Get by id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>The domain object or null on lookup failure.</returns>
        T Get(int id);

        /// <summary>
        /// Gets all domain objects of this type in the underlying database. Note that calling this may not be a good
        /// idea for some domain object types.
        /// </summary>
        /// <returns>All objects of the configured type in the database (possibly empty, never null).</returns>
        ICollection<T> GetAll();
    }
}