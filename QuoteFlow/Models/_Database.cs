namespace QuoteFlow.Models
{
    public class QuoteFlowDatabase : Dapper.Database<QuoteFlowDatabase>
    {
        public Table<Asset> Assets { get; private set; }
        public Table<AssetPrice> AssetPrices { get; private set; }
        public Table<AssetVar> AssetVars { get; private set; }
        public Table<AssetVarValue> AssetVarValues { get; private set; } 
        public Table<Catalog> Catalogs { get; private set; }
        public Table<Contact> Contacts { get; private set; }
        public Table<Credential> Credentials { get; private set; } 
        public Table<Manufacturer> Manufacturers { get; private set; }
        public Table<Organization> Organizations { get; private set; }
        public Table<OrganizationUser> OrganizationUsers { get; private set; }
        public Table<Quote> Quotes { get; private set; }
        public Table<User> Users { get; private set; }
        public Table<UserActivity> UserActivities { get; private set; } 
    }
}