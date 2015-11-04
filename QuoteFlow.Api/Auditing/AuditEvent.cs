namespace QuoteFlow.Api.Auditing
{
    /// <summary>
    /// A simple enum to track audit events. 
    /// 
    /// DO NOT CHANGE THE VALUE OF AN EVENT NAME! Audit logs store the 
    /// event value, not the event name.
    /// </summary>
    public enum AuditEvent
    {
        None = 0,

        CatalogCreated = 100,
        CatalogUpdated = 101,
        CatalogDeleted = 102,
        CatalogAssetsImported = 103,

        AssetCreated = 200,
        AssetUpdated = 201,
        AssetDeleted = 202,
        AssetCommentAdded = 203,
        AssetCommentEdited = 204,
        AssetCommentDeleted = 205,
        AssetImported = 206,

        ManufacturerAdded = 300,
        ManufacturerUpdated = 301,
        ManufacturerDeleted = 302,

        QuoteAdded = 400,
        QuoteUpdated = 401,
        QuoteDeleted = 402,
        QuoteStatusChanged = 403,
        QuoteLineItemAdded = 404,
        QuoteMemberAdded = 405,

        UserAdded = 500,
        UserEdited = 501,
        UserDeleted = 502,
        UserLogin = 503,
        UserPasswordChange = 504
    }
}