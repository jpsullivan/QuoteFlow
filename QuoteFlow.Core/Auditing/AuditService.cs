﻿using System;
using System.Collections.Generic;
using QuoteFlow.Api.Auditing;
using QuoteFlow.Api.Auditing.DetailResolvers;
using QuoteFlow.Api.Models;
using StackExchange.Profiling.Helpers.Dapper;

namespace QuoteFlow.Core.Auditing
{
    public class AuditService : IAuditService
    {
        public IEnumerable<AuditLogRecord> GetAuditLogs()
        {
            const string sql = @"
                select * from AuditLog log 
                left join Users u on u.Id = log.UserId
                order by log.CreatedUtc DESC
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
                order by log.CreatedUtc DESC
            ";

            return Current.DB.Query<AuditLogRecord, User, AuditLogRecord>(sql, (ch, user) =>
            {
                ch.User = user;
                return ch;
            }, new { category });
        }

        public IEnumerable<AuditLogRecord> GetAuditLogs(params AuditCategory[] categories)
        {
            const string sql = @"
                select * from AuditLog log 
                left join Users u on u.Id = log.UserId
                where log.Category in @categories
                order by log.CreatedUtc DESC
            ";

            return Current.DB.Query<AuditLogRecord, User, AuditLogRecord>(sql, (ch, user) =>
            {
                ch.User = user;
                return ch;
            }, new { categories });
        }

        public IEnumerable<AuditLogRecord> GetAuditLogs(AuditEvent excludedEvent, params AuditCategory[] categories)
        {
            const string sql = @"
                select * from AuditLog log 
                left join Users u on u.Id = log.UserId
                where log.Category in @categories and Event != @excludedEvent
                order by log.CreatedUtc DESC
            ";

            return Current.DB.Query<AuditLogRecord, User, AuditLogRecord>(sql, (ch, user) =>
            {
                ch.User = user;
                return ch;
            }, new { categories, excludedEvent });
        }

        public IEnumerable<AuditLogRecord> GetAuditLogs(int actorId, int? parentActorId, params AuditCategory[] categories)
        {
            var builder = new SqlBuilder();
            var tmpl = builder.AddTemplate(@"
                select * from AuditLog log 
                left join Users u on u.Id = log.UserId
                /**where**/
                order by log.CreatedUtc DESC"
            );

            if (parentActorId.HasValue)
            {
                builder.Where("log.Category in @categories");
                builder.Where("log.ActorId = @actorId");
                builder.Where("log.ParentActorId = @parentActorId");
            }
            else
            {
                builder.Where("log.Category in @categories");
                builder.Where("log.ActorId = @actorId");
            }

            return Current.DB.Query<AuditLogRecord, User, AuditLogRecord>(tmpl.RawSql, (ch, user) =>
            {
                ch.User = user;
                return ch;
            }, new { categories, actorId, parentActorId });
        }

        public IEnumerable<AuditLogRecord> GetAuditLogs(int userId)
        {
            const string sql = @"
                select * from AuditLog log 
                left join Users u on u.Id = log.UserId
                where log.UserId = @userId
                order by log.CreatedUtc DESC
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
                order by log.CreatedUtc DESC
            ";

            return Current.DB.Query<AuditLogRecord, User, AuditLogRecord>(sql, (ch, user) =>
            {
                ch.User = user;
                return ch;
            }, new { userId, category });
        }

        public void SaveAuditRecord(AuditCategory category, AuditEvent @event, int userId, int? actorId, int? parentActorId, string details)
        {
            Current.DB.AuditLog.Insert(new
            {
                category,
                @event,
                userId,
                actorId,
                parentActorId,
                RawDetails = details,
                CreatedUtc = DateTime.UtcNow
            });
        }

        public IEnumerable<AuditLogRecord> GetCatalogAuditLogs(int catalogId)
        {
            var logs = new List<AuditLogRecord>();
            var allLogs = GetAuditLogs(AuditEvent.AssetImported, AuditCategory.Asset, AuditCategory.Catalog);
            foreach (var log in allLogs)
            {
                if (log.Category == AuditCategory.Catalog)
                {
                    if (log.ActorId == catalogId)
                    {
                        logs.Add(log);
                    }
                }

                if (log.Category == AuditCategory.Asset)
                {
                    if (log.ParentActorId == catalogId)
                    {
                        logs.Add(log);
                    }
                }
            }
            
            return logs;
        }

        public void SaveCatalogAuditRecord(AuditEvent @event, int userId, int catalogId)
        {
            SaveAuditRecord(AuditCategory.Catalog, @event, userId, catalogId, null, null);
        }

        public void SaveCatalogAuditRecord(AuditEvent @event, int userId, int catalogId, string details)
        {
            SaveAuditRecord(AuditCategory.Catalog, @event, userId, catalogId, null, details);
        }

        public void SaveCatalogAuditRecord(AuditEvent @event, int userId, int catalogId, IDetailResolver details)
        {
            SaveAuditRecord(AuditCategory.Catalog, @event, userId, catalogId, null, details.Serialize());
        }

        public IEnumerable<AuditLogRecord> GetAssetAuditLogs(int assetId)
        {
            return GetAuditLogs(assetId, null, AuditCategory.Asset);
        }

        public void SaveAssetAuditRecord(AuditEvent @event, int userId, int assetId, int catalogId)
        {
            SaveAuditRecord(AuditCategory.Asset, @event, userId, assetId, catalogId, null);
        }

        public void SaveAssetAuditRecord(AuditEvent @event, int userId, int assetId, int catalogId, string details)
        {
            SaveAuditRecord(AuditCategory.Asset, @event, userId, assetId, catalogId, details);
        }

        public void SaveAssetAuditRecord(AuditEvent @event, int userId, int assetId, int catalogId, IDetailResolver details)
        {
            SaveAuditRecord(AuditCategory.Asset, @event, userId, assetId, catalogId, details.Serialize());
        }

        public void SaveManufacturerAuditRecord(AuditEvent @event, int userId, int manufacturerId)
        {
            SaveAuditRecord(AuditCategory.Manufacturer, @event, userId, manufacturerId, null, null);
        }

        public void SaveManufacturerAuditRecord(AuditEvent @event, int userId, int manufacturerId, string details)
        {
            SaveAuditRecord(AuditCategory.Manufacturer, @event, userId, manufacturerId, null, details);
        }

        public void SaveQuoteAuditRecord(AuditEvent @event, int userId, int quoteId)
        {
            SaveAuditRecord(AuditCategory.Quote, @event, userId, quoteId, null, null);
        }

        public void SaveQuoteAuditRecord(AuditEvent @event, int userId, int quoteId, string details)
        {
            SaveAuditRecord(AuditCategory.Quote, @event, userId, quoteId, null, details);
        }

        public void SaveUserAuditRecord(AuditEvent @event, int userId, int actorUserId)
        {
            SaveAuditRecord(AuditCategory.User, @event, userId, actorUserId, null, null);
        }

        public void SaveUserAuditRecord(AuditEvent @event, int userId, int actorUserId, string details)
        {
            SaveAuditRecord(AuditCategory.User, @event, userId, actorUserId, null, details);
        }
    }
}