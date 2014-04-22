using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuoteFlow.Models
{
    public class PasswordReset
    {
        public long PasswordResetId { get; set; }
        public long UserId { get; set; }
        public string ResetCode { get; set; }
        public DateTime Updated { get; set; }
        public DateTime Created { get; set; }

        public virtual User User { get; set; }
    }
}