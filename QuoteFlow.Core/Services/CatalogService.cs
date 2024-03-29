﻿using System;
using System.Collections.Generic;
using System.Linq;
using QuoteFlow.Api.Auditing;
using QuoteFlow.Api.Models;
using QuoteFlow.Api.Models.ViewModels;
using QuoteFlow.Api.Services;

namespace QuoteFlow.Core.Services
{
    public class CatalogService : ICatalogService
    {
        #region DI

        private IAuditService AuditService { get; set; }
        private ICatalogImportSummaryRecordsService CatalogSummaryService { get; set; }
        private IManufacturerService ManufacturerService { get; set; }

        public CatalogService()
        {
        }

        public CatalogService(IAuditService auditService, ICatalogImportSummaryRecordsService catalogSummaryService, IManufacturerService manufacturerService)
        {
            AuditService = auditService;
            CatalogSummaryService = catalogSummaryService;
            ManufacturerService = manufacturerService;
        }

        #endregion

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
            const string sql = "select * from Catalogs where Name = @catalogName";
            return Current.DB.Query<Catalog>(sql, new {catalogName}).FirstOrDefault();
        }

        /// <summary>
        /// Fetches all of the catalogs within a given organization.
        /// </summary>
        /// <param name="organizationId">The organization id</param>
        /// <returns></returns>
        public IEnumerable<Catalog> GetCatalogs(int organizationId)
        {
            const string sql = "select * from catalogs where OrganizationId = @organizationId";
            return Current.DB.Query<Catalog>(sql, new {organizationId});
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

            AuditService.SaveCatalogAuditRecord(AuditEvent.CatalogCreated, userId, catalog.Id);

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
            const string sql = "select * from Catalogs where Name = @catalogName AND OrganizationId = @organizationId";
            var catalog = Current.DB.Query<Catalog>(sql, new { catalogName, organizationId }).FirstOrDefault();
            return catalog != null;
        }

        /// <summary>
        /// Returns the catalog creator name.
        /// </summary>
        /// <param name="creatorId">the id of the creator</param>
        /// <returns></returns>
        public string GetCreatorName(int creatorId)
        {
            const string sql = "select FullName from Users where Id = @creatorId";
            return Current.DB.Query<string>(sql, new {creatorId}).FirstOrDefault();
        }

        /// <summary>
        /// Fetches catalogs that exist within a series of <see cref="Organization"/>s.
        /// </summary>
        /// <param name="orgs"></param>
        /// <returns></returns>
        public IEnumerable<Catalog> GetCatalogsWithinOrganizations(ICollection<Organization> orgs)
        {
            if (!orgs.Any()) {
                return null;
            }

            const string sql = "select * from Catalogs where OrganizationId in @orgs";
            return Current.DB.Query<Catalog>(sql, new { @orgs = orgs.Select(org => org.Id).ToArray() });
        }

        public IEnumerable<Manufacturer> GetManufacturers(int catalogId)
        {
            const string sql = @"select distinct m.* from Assets a
                                left join Manufacturers m on a.ManufacturerId = m.Id
                                where a.CatalogId = @catalogId
                                order by m.Name asc";

            return Current.DB.Query<Manufacturer>(sql, new {catalogId});
        }
    }
}
