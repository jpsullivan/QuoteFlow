using System;
using System.Collections.Generic;
using Dapper;
using QuoteFlow.Api.Models;

namespace QuoteFlow.Api.Auditing
{
    public class AuditLogRecord
    {
        public AuditLogRecord()
        {
        }

        public AuditLogRecord(int id, AuditCategory category, AuditEvent @event, int userId, string details, DateTime createdUtc)
        {
            Id = id;
            Category = category;
            Event = @event;
            UserId = userId;
            RawDetails = details;
            CreatedUtc = createdUtc;
        }

        /// <summary>
        /// The record ID.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The event category that this change could be classified as.
        /// </summary>
        public AuditCategory Category { get; set; }

        /// <summary>
        /// The resolved event type based on its event value.
        /// </summary>
        public AuditEvent Event { get; set; }

        /// <summary>
        /// The user who performed the logged action.
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// The resolved <see cref="User"/> based on the UserId.
        /// </summary>
        public User User { get; set; }

        /// <summary>
        /// The ID of whatever has been created/updated/deleted.
        /// For example, creating a new Catalog would populate this with
        /// the new catalog ID.
        /// </summary>
        public int? ActorId { get; set; }

        /// <summary>
        /// The ID of whatever the "parent" member to the actor is.
        /// For example, modifying an asset would populate this with
        /// whatever the assets catalog ID is.
        /// </summary>
        public int? ParentActorId { get; set; }

        /// <summary>
        /// A JSON-serialized representation of any extra details that this
        /// record may provide. Useful for giving before/after details of a change.
        /// e.g. OldCustomerName = "company abc", NewCustomerName = "company 123"
        /// </summary>
        public string RawDetails { get; set; }

        /// <summary>
        /// Key-value representation of the <see cref="RawDetails"/> property.
        /// </summary>
        [IgnoreProperty(true)]
        public IDictionary<string, string> Details { get; set; } 

        /// <summary>
        /// When this record was created.
        /// </summary>
        public DateTime CreatedUtc { get; set; }
    }
}