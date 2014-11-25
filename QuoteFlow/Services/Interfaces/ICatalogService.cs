using System.Collections.Generic;
using QuoteFlow.Models;
using QuoteFlow.Models.ViewModels;

namespace QuoteFlow.Services.Interfaces
{
    public interface ICatalogService
    {
        /// <summary>
        /// Fetch a <see cref="Catalog"/> based on its id.
        /// </summary>
        /// <param name="catalogId">The <see cref="Catalog"/> identifier.</param>
        /// <returns></returns>
        Catalog GetCatalog(int catalogId);

        /// <summary>
        /// Fetches a <see cref="Catalog"/> based on its name.
        /// </summary>
        /// <param name="catalogName">The <see cref="Catalog"/> name</param>
        /// <returns></returns>
        Catalog GetCatalog(string catalogName);

        /// <summary>
        /// Fetches all of the catalogs within a given organization.
        /// </summary>
        /// <param name="organizationId">The organization id</param>
        /// <returns></returns>
        IEnumerable<Catalog> GetCatalogs(int organizationId);
            
        /// <summary>
        /// Adds a new catalog from the database based on ViewModel data from the new catalog form
        /// </summary>
        /// <param name="model">NewCatalogModel ViewModel</param>
        /// <param name="userId">The identifier of the <see cref="User"/> who is creating this catalog.</param>
        /// <returns></returns>
        Catalog CreateCatalog(NewCatalogModel model, int userId);

        /// <summary>
        /// Check to see if any catalog's with the supplied name currently exists within
        /// the supplied organization id.
        /// </summary>
        /// <param name="catalogName"></param>
        /// <param name="organizationId"></param>
        /// <returns></returns>
        bool CatalogNameExists(string catalogName, int organizationId);

        /// <summary>
        /// Returns the catalog creator name.
        /// </summary>
        /// <param name="creatorId">the id of the creator</param>
        /// <returns></returns>
        string GetCreatorName(int creatorId);

        /// <summary>
        /// Fetches catalogs that exist within a series of <see cref="Organization"/>s.
        /// </summary>
        /// <param name="orgs"></param>
        /// <returns></returns>
        IEnumerable<Catalog> GetCatalogsWithinOrganizations(ICollection<Organization> orgs);
    }
}