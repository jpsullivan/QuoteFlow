using System;

namespace QuoteFlow.Models
{
    public class UserActivity
    {
        public long UserActivityId { get; set; }
        public long UserId { get; set; }
        public string Acitivity { get; set; }
        public DateTime Created { get; set; }
    }
}