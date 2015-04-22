using Jil;

namespace QuoteFlow.Api.Auditing.DetailResolvers.Asset
{
    public class AssetCreated : IDetailResolver
    {
        public AssetCreated()
        {
        }

        public AssetCreated(AuditLogRecord log)
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