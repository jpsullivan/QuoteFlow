using System;
using System.Collections.Generic;
using QuoteFlow.Api.Auditing;
using QuoteFlow.Api.Models;

namespace QuoteFlow.Core.Auditing
{
    public class AuditService : IAuditService
    {
        public IEnumerable<AuditLogRecord> GetAuditLogs()
        {
            const string sql = @"
                select * from AuditLog log 
                left join Users u on u.Id = log.UserId
            ";

            return Current.DB.Query<AuditLogRecord, User, AuditLogRecord>(sql, (ch, user) =>
            {
                ch.User = user;
                return ch;
            });
        }

        public IEnumerable<AuditLogRecord> GetAuditLogs(AuditCategory category)
        {
            const string sql = @"
                select * from AuditLog log 
                left join Users u on u.Id = log.UserId
                where log.Category = @category
            ";

            return Current.DB.Query<AuditLogRecord, User, AuditLogRecord>(sql, (ch, user) =>
            {
                ch.User = user;
                return ch;
            }, new { category });
        }

        public IEnumerable<AuditLogRecord> GetAuditLogs(int userId)
        {
            const string sql = @"
                select * from AuditLog log 
                left join Users u on u.Id = log.UserId
                where log.UserId = @userId
            ";

            return Current.DB.Query<AuditLogRecord, User, AuditLogRecord>(sql, (ch, user) =>
            {
                ch.User = user;
                return ch;
            }, new { userId });
        }

        public IEnumerable<AuditLogRecord> GetAuditLogs(int userId, AuditCategory category)
        {
            const string sql = @"
                select * from AuditLog log 
                left join Users u on u.Id = log.UserId
                where log.UserId = @userId and log.Category = @category
            ";

            return Current.DB.Query<AuditLogRecord, User, AuditLogRecord>(sql, (ch, user) =>
            {
                ch.User = user;
                return ch;
            }, new { userId, category });
        }

        public void SaveAuditRecord(AuditCategory category, AuditEvent @event, int userId, string details)
        {
            Current.DB.AuditLog.Insert(new
            {
                category,
                @event,
                userId,
                details,
                CreatedUtc = DateTime.UtcNow
            });
        }

        public void SaveCatalogAuditRecord(AuditEvent @event, int userId)
        {
            SaveAuditRecord(AuditCategory.Catalog, @event, userId, null);
        }

        public void SaveCatalogAuditRecord(AuditEvent @event, int userId, string details)
        {
            SaveAuditRecord(AuditCategory.Catalog, @event, userId, details);
        }

        public void SaveAssetAuditRecord(AuditEvent @event, int userId)
        {
            SaveAuditRecord(AuditCategory.Asset, @event, userId, null);
        }

        public void SaveAssetAuditRecord(AuditEvent @event, int userId, string details)
        {
            SaveAuditRecord(AuditCategory.Asset, @event, userId, details);
        }

        public void SaveManufacturerAuditRecord(AuditEvent @event, int userId)
        {
            SaveAuditRecord(AuditCategory.Manufacturer, @event, userId, null);
        }

        public void SaveManufacturerAuditRecord(AuditEvent @event, int userId, string details)
        {
            SaveAuditRecord(AuditCategory.Manufacturer, @event, userId, details);
        }

        public void SaveQuoteAuditRecord(AuditEvent @event, int userId)
        {
            SaveAuditRecord(AuditCategory.Quote, @event, userId, null);
        }

        public void SaveQuoteAuditRecord(AuditEvent @event, int userId, string details)
        {
            SaveAuditRecord(AuditCategory.Quote, @event, userId, details);
        }

        public void SaveUserAuditRecord(AuditEvent @event, int userId)
        {
            SaveAuditRecord(AuditCategory.User, @event, userId, null);
        }

        public void SaveUserAuditRecord(AuditEvent @event, int userId, string details)
        {
            SaveAuditRecord(AuditCategory.User, @event, userId, details);
        }
    }
}