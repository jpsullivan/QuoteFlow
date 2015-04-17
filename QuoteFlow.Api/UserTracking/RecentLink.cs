using System;

namespace QuoteFlow.Api.UserTracking
{
    public class RecentLink
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public PageType PageType { get; set; }
        public int PageId { get; set; }
        public string PageName { get; set; }
        public DateTime VisitedUtc { get; set; }

        protected bool Equals(RecentLink other)
        {
            return PageType == other.PageType && PageId == other.PageId && string.Equals(PageName, other.PageName);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((RecentLink) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (int) PageType;
                hashCode = (hashCode*397) ^ PageId;
                hashCode = (hashCode*397) ^ (PageName != null ? PageName.GetHashCode() : 0);
                return hashCode;
            }
        }
    }
}