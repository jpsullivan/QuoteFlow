using System.Collections.Generic;
using QuoteFlow.Api.Auditing.DetailResolvers;
using QuoteFlow.Api.Models;

namespace QuoteFlow.Api.Auditing
{
    public interface IAuditService
    {
        /// <summary>
        /// Returns every audit log in the database, ordered by most recent descending.
        /// </summary>
        /// <returns></returns>
        IEnumerable<AuditLogRecord> GetAuditLogs();

        /// <summary>
        /// Gets the audit logs for a specific <see cref="AuditCategory"/>.
        /// </summary>
        /// <param name="category">The auditing category to filter by.</param>
        /// <returns></returns>
        IEnumerable<AuditLogRecord> GetAuditLogs(AuditCategory category);

        /// <summary>
        /// Fetches the audit logs for a specific set of <see cref="AuditCategory"/> objects.
        /// </summary>
        /// <param name="categories"></param>
        /// <returns></returns>
        IEnumerable<AuditLogRecord> GetAuditLogs(params AuditCategory[] categories);

        /// <summary>
        /// Fetches the audit logs for a specific set of <see cref="AuditCategory"/> objects.
        /// </summary>
        /// <param name="actorId"></param>
        /// <param name="parentActorId"></param>
        /// <param name="categories"></param>
        /// <returns></returns>
        IEnumerable<AuditLogRecord> GetAuditLogs(int actorId, int? parentActorId, params AuditCategory[] categories);

            /// <summary>
        /// Gets the audit logs for a specific <see cref="User"/>.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        IEnumerable<AuditLogRecord> GetAuditLogs(int userId);

        /// <summary>
        /// Gets the audit logs for a specific <see cref="User"/> and <see cref="AuditCategory"/>.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="category"></param>
        /// <returns></returns>
        IEnumerable<AuditLogRecord> GetAuditLogs(int userId, AuditCategory category);

        /// <summary>
        /// Saves an audit record to the database.
        /// </summary>
        /// <param name="category"></param>
        /// <param name="event"></param>
        /// <param name="userId"></param>
        /// <param name="actorId"></param>
        /// <param name="parentActorId"></param>
        /// <param name="details"></param>
        void SaveAuditRecord(AuditCategory category, AuditEvent @event, int userId, int? actorId, int? parentActorId, string details);

        /// <summary>
        /// Fetches the activity history for a specific catalog. This includes 
        /// modifications made to the catalog itself, but also the assets nested inside.
        /// </summary>
        /// <param name="catalogId">The catalog ID</param>
        /// <returns></returns>
        IEnumerable<AuditLogRecord> GetCatalogAuditLogs(int catalogId);

        /// <summary>
        /// Saves a catalog audit record with no details to the database.
        /// </summary>
        /// <param name="event"></param>
        /// <param name="userId"></param>
        /// <param name="catalogId"></param>
        void SaveCatalogAuditRecord(AuditEvent @event, int userId, int catalogId);

        /// <summary>
        /// Saves a catalog audit record to the database.
        /// </summary>
        /// <param name="event"></param>
        /// <param name="userId"></param>
        /// <param name="catalogId"></param>
        /// <param name="details"></param>
        void SaveCatalogAuditRecord(AuditEvent @event, int userId, int catalogId, string details);

        /// <summary>
        /// Saves a catalog audit record to the database.
        /// </summary>
        /// <param name="event"></param>
        /// <param name="userId"></param>
        /// <param name="catalogId"></param>
        /// <param name="details"></param>
        void SaveCatalogAuditRecord(AuditEvent @event, int userId, int catalogId, IDetailResolver details);

        /// <summary>
        /// Saves an asset audit record with no details to the database.
        /// </summary>
        /// <param name="event"></param>
        /// <param name="userId"></param>
        /// <param name="assetId"></param>
        /// <param name="catalogId"></param>
        void SaveAssetAuditRecord(AuditEvent @event, int userId, int assetId, int catalogId);

        /// <summary>
        /// Saves an asset audit record to the database.
        /// </summary>
        /// <param name="event"></param>
        /// <param name="userId"></param>
        /// <param name="assetId"></param>
        /// <param name="catalogId"></param>
        /// <param name="details"></param>
        void SaveAssetAuditRecord(AuditEvent @event, int userId, int assetId, int catalogId, string details);

        /// <summary>
        /// Saves an audit record to the database.
        /// </summary>
        /// <param name="event"></param>
        /// <param name="userId"></param>
        /// <param name="assetId"></param>
        /// <param name="catalogId"></param>
        /// <param name="details"></param>
        void SaveAssetAuditRecord(AuditEvent @event, int userId, int assetId, int catalogId, IDetailResolver details);

        /// <summary>
        /// Saves a manufacturer with no details to the database.
        /// </summary>
        /// <param name="event"></param>
        /// <param name="userId"></param>
        /// <param name="manufacturerId"></param>
        void SaveManufacturerAuditRecord(AuditEvent @event, int userId, int manufacturerId);

        /// <summary>
        /// Saves a manufacturer audit record to the database.
        /// </summary>
        /// <param name="event"></param>
        /// <param name="userId"></param>
        /// <param name="manufacturerId"></param>
        /// <param name="details"></param>
        void SaveManufacturerAuditRecord(AuditEvent @event, int userId, int manufacturerId, string details);

        /// <summary>
        /// Saves a quote audit record to the database.
        /// </summary>
        /// <param name="event"></param>
        /// <param name="userId"></param>
        /// <param name="quoteId"></param>
        void SaveQuoteAuditRecord(AuditEvent @event, int userId, int quoteId);

        /// <summary>
        /// Saves a quote audit record to the database.
        /// </summary>
        /// <param name="event"></param>
        /// <param name="userId"></param>
        /// <param name="quoteId"></param>
        /// <param name="details"></param>
        void SaveQuoteAuditRecord(AuditEvent @event, int userId, int quoteId, string details);

        /// <summary>
        /// Saves a user audit record to the database.
        /// </summary>
        /// <param name="event"></param>
        /// <param name="userId"></param>
        /// <param name="actorUserId"></param>
        void SaveUserAuditRecord(AuditEvent @event, int userId, int actorUserId);

        /// <summary>
        /// Saves a user audit record to the database.
        /// </summary>
        /// <param name="event"></param>
        /// <param name="userId"></param>
        /// <param name="actorUserId"></param>
        /// <param name="details"></param>
        void SaveUserAuditRecord(AuditEvent @event, int userId, int actorUserId, string details);
    }
}