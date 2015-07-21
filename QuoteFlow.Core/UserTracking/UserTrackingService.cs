using System;
using System.Collections.Generic;
using System.Linq;
using QuoteFlow.Api.UserTracking;

namespace QuoteFlow.Core.UserTracking
{
    public class UserTrackingService : IUserTrackingService
    {
        public IEnumerable<RecentLink> GetRecentLinks(int userId)
        {
            const string sql = @"
                select * from RecentlyVisitedLinks where UserId = @userId
                order by VisitedUtc DESC
            ";
            return Current.DB.Query<RecentLink>(sql, new {userId});
        }

        public IEnumerable<RecentLink> GetRecentLinks(int userId, PageType type)
        {
            const string sql = @"
                select * from RecentlyVisitedLinks where UserId = @userId and PageType = @type
                order by VisitedUtc DESC
            ";
            return Current.DB.Query<RecentLink>(sql, new { userId, type });
        }

        public void UpdateRecentLinks(int userId, PageType type, int pageId, string pageName)
        {
            if (pageName == null) throw new ArgumentNullException(nameof(pageName));

            var recentPages = GetRecentLinks(userId, type).ToList();
            var newPage = new RecentLink
            {
                UserId = userId,
                PageType = type,
                PageId = pageId,
                PageName = pageName,
                VisitedUtc = DateTime.UtcNow
            };

            // remove this page if it has already been visited to prevent duplicates
            foreach (var recentPage in recentPages.Where(recentPage => recentPage.Equals(newPage)))
            {
                DeleteLink(recentPage.Id);
            }

            // we only want a max of 5 tracked pages of a specific type
            if (recentPages.Count() == 5)
            {
                DeleteOldestLink(userId, type);
            }

            Current.DB.RecentlyVisitedLinks.Insert(newPage);
        }

        public void DeleteOldestLink(int userId, PageType type)
        {
            var oldest = GetRecentLinks(userId, type).Last();
            Current.DB.RecentlyVisitedLinks.Delete(oldest.Id);
        }

        public void DeleteLink(int id)
        {
            Current.DB.RecentlyVisitedLinks.Delete(id);
        }
    }
}