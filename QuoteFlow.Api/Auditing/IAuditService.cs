using System;
using System.Collections.Generic;
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
        /// <param name="details"></param>
        void SaveAuditRecord(AuditCategory category, AuditEvent @event, int userId, string details);

        /// <summary>
        /// Saves a catalog audit record with no details to the database.
        /// </summary>
        /// <param name="event"></param>
        /// <param name="userId"></param>
        void SaveCatalogAuditRecord(AuditEvent @event, int userId);

        /// <summary>
        /// Saves a catalog audit record to the database.
        /// </summary>
        /// <param name="event"></param>
        /// <param name="userId"></param>
        /// <param name="details"></param>
        void SaveCatalogAuditRecord(AuditEvent @event, int userId, string details);

        /// <summary>
        /// Saves an asset audit record with no details to the database.
        /// </summary>
        /// <param name="event"></param>
        /// <param name="userId"></param>
        void SaveAssetAuditRecord(AuditEvent @event, int userId);

        /// <summary>
        /// Saves an asset audit record to the database.
        /// </summary>
        /// <param name="event"></param>
        /// <param name="userId"></param>
        /// <param name="details"></param>
        void SaveAssetAuditRecord(AuditEvent @event, int userId, string details);

        /// <summary>
        /// Saves a manufacturer with no details to the database.
        /// </summary>
        /// <param name="event"></param>
        /// <param name="userId"></param>
        void SaveManufacturerAuditRecord(AuditEvent @event, int userId);

        /// <summary>
        /// Saves a manufacturer audit record to the database.
        /// </summary>
        /// <param name="event"></param>
        /// <param name="userId"></param>
        /// <param name="details"></param>
        void SaveManufacturerAuditRecord(AuditEvent @event, int userId, string details);

        /// <summary>
        /// Saves a quote audit record to the database.
        /// </summary>
        /// <param name="event"></param>
        /// <param name="userId"></param>
        void SaveQuoteAuditRecord(AuditEvent @event, int userId);

        /// <summary>
        /// Saves a quote audit record to the database.
        /// </summary>
        /// <param name="event"></param>
        /// <param name="userId"></param>
        /// <param name="details"></param>
        void SaveQuoteAuditRecord(AuditEvent @event, int userId, string details);

        /// <summary>
        /// Saves a user audit record to the database.
        /// </summary>
        /// <param name="event"></param>
        /// <param name="userId"></param>
        void SaveUserAuditRecord(AuditEvent @event, int userId);

        /// <summary>
        /// Saves a user audit record to the database.
        /// </summary>
        /// <param name="event"></param>
        /// <param name="userId"></param>
        /// <param name="details"></param>
        void SaveUserAuditRecord(AuditEvent @event, int userId, string details);
    }
}