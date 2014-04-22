using System;
using System.Linq;
using QuoteFlow.Models;
using QuoteFlow.Models.ViewModels;
using QuoteFlow.Services.Interfaces;

namespace QuoteFlow.Services
{
    public class CatalogService : ICatalogService
    {
        /// <summary>
        /// Fetch a <see cref="Catalog"/> based on its id.
        /// </summary>
        /// <param name="catalogId">The <see cref="Catalog"/> identifier.</param>
        /// <returns></returns>
        public Catalog GetCatalog(int catalogId)
        {
            return Current.DB.Catalogs.Get(catalogId);
        }

        /// <summary>
        /// Fetches a <see cref="Catalog"/> based on its name.
        /// </summary>
        /// <param name="catalogName">The <see cref="Catalog"/> name</param>
        /// <returns></returns>
        public Catalog GetCatalog(string catalogName)
        {
            return
                Current.DB.Query<Catalog>("select * from Catalogs where Name = @catalogName", new {catalogName})
                    .FirstOrDefault();
        }

        /// <summary>
        /// Adds a new catalog from the database based on ViewModel data from the new catalog form
        /// </summary>
        /// <param name="model">NewCatalogModel ViewModel</param>
        /// <param name="userId">The identifier of the <see cref="User"/> who is creating this catalog.</param>
        /// <returns></returns>
        public Catalog CreateCatalog(NewCatalogModel model, int userId)
        {
            if (!model.Expirable)
            {
                model.ExpirationDate = null;
            }

            var catalog = new Catalog
            {
                CreationDate = DateTime.UtcNow,
                CreatorId = userId,
                Enabled = true,
                ExpirationDate = model.ExpirationDate,
                LastUpdated = DateTime.UtcNow,
                Name = model.Name,
                Description = model.Description,
                OrganizationId = model.Organization.Id
            };

            var insert = Current.DB.Catalogs.Insert(new
            {
                catalog.Name,
                catalog.Description,
                catalog.OrganizationId,
                catalog.CreatorId,
                catalog.ExpirationDate,
                catalog.CreationDate,
                catalog.LastUpdated,
                catalog.Enabled
            });

            if (insert != null)
            {
                catalog.Id = insert.Value;
            }

            return catalog;
        }

        /// <summary>
        /// Check to see if any catalog's with the supplied name currently exists within
        /// the supplied organization id.
        /// </summary>
        /// <param name="catalogName"></param>
        /// <param name="organizationId"></param>
        /// <returns></returns>
        public bool CatalogNameExists(string catalogName, int organizationId)
        {
            var catalog =
                Current.DB.Query<Catalog>(
                    "select * from Catalogs where Name = @catalogName AND OrganizationId = @organizationId", new
                    {
                        catalogName,
                        organizationId
                    }).FirstOrDefault();

            return catalog != null;
        }

        /// <summary>
        /// Returns the catalog creator name.
        /// </summary>
        /// <param name="creatorId">the id of the creator</param>
        /// <returns></returns>
        public string GetCreatorName(int creatorId)
        {
            return
                Current.DB.Query<string>("select FullName from Users where Id = @creatorId", new { creatorId })
                    .FirstOrDefault();
        }
    }
}