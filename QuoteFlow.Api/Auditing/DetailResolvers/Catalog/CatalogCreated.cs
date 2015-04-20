using Jil;

namespace QuoteFlow.Api.Auditing.DetailResolvers.Catalog
{
    public class CatalogCreated : IDetailResolver
    {
        public CatalogCreated()
        {
        }

        public CatalogCreated(AuditLogRecord log)
        {
            Log = log;
        }

        /// <summary>
        /// The newly created catalog ID.
        /// </summary>
        public AuditLogRecord Log { get; set; }

        public string Serialize()
        {
            return JSON.Serialize(this);
        }
    }
}