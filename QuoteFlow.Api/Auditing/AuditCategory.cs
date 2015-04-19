namespace QuoteFlow.Api.Auditing
{
    /// <summary>
    /// A structure to provide audit logs with a grouping mechanism.
    /// </summary>
    public enum AuditCategory
    {
        Asset,
        Catalog,
        Manufacturer,
        Quote,
        User
    }
}