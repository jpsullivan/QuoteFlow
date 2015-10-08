using QuoteFlow.Api.Auditing;
using QuoteFlow.Api.Models.CatalogImport;
using QuoteFlow.Api.UserTracking;

namespace QuoteFlow.Api.Models
{
    public class QuoteFlowDatabase : Dapper.Database<QuoteFlowDatabase>
    {
        public Table<Asset> Assets { get; private set; }
        public Table<AssetComment> AssetComments { get; private set; } 
        public Table<AssetVar> AssetVars { get; private set; }
        public Table<AssetVarValue> AssetVarValues { get; private set; }
        public Table<AuditLogRecord> AuditLog { get; private set; } 
        public Table<Catalog> Catalogs { get; private set; }
        public Table<CatalogImportSummaryRecord> CatalogImportSummaryRecords { get; private set; }
        public Table<Credential> Credentials { get; private set; }
        public Table<Customer> Customers { get; private set; }
        public Table<Manufacturer> Manufacturers { get; private set; }
        public Table<ManufacturerLogo> ManufacturerLogos { get; private set; } 
        public Table<Organization> Organizations { get; private set; }
        public Table<OrganizationUser> OrganizationUsers { get; private set; }
        public Table<QuoteLineItem> QuoteLineItems { get; private set; } 
        public Table<Quote> Quotes { get; private set; }
        public Table<QuoteStatus> QuoteStatus { get; private set; }
        public Table<RecentLink> RecentlyVisitedLinks { get; private set; } 
        public Table<User> Users { get; private set; }
        public Table<UserActivity> UserActivities { get; private set; } 
    }
}