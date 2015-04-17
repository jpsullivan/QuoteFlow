using System.Collections.Generic;

namespace QuoteFlow.Api.UserTracking
{
    /// <summary>
    /// The interface for a service that manages recently viewed pages 
    /// of certain types for users.
    /// </summary>
    public interface IUserTrackingService
    {
        /// <summary>
        /// Fetches all of the most recent links for a specific user.
        /// </summary>
        /// <param name="userId">The ID of the user to fetch recently visited pages for</param>
        /// <returns></returns>
        IEnumerable<RecentLink> GetRecentLinks(int userId);

        /// <summary>
        /// Fetches the most recent links for a specific user and page type.
        /// </summary>
        /// <param name="userId">The ID of the user to fetch recently visited pages for</param>
        /// <param name="type">The type of page to filter by</param>
        /// <returns></returns>
        IEnumerable<RecentLink> GetRecentLinks(int userId, PageType type); 

        /// <summary>
        /// Adds a new catalog to the visited list. Pops the oldest recent visit off the
        /// list, pushing this one to the top.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="type"></param>
        /// <param name="pageId"></param>
        /// <param name="pageName"></param>
        void UpdateRecentLinks(int userId, PageType type, int pageId, string pageName);

        /// <summary>
        /// Pops the oldest recently visited page for a single user off the db.
        /// </summary>
        /// <param name="userId">The user that will have their oldest visited link removed</param>
        /// <param name="type">The type of page that is being removed.</param>
        void DeleteOldestLink(int userId, PageType type);

        /// <summary>
        /// Deletes a link.
        /// </summary>
        /// <param name="id">The <see cref="RecentLink"/> ID</param>
        void DeleteLink(int id);
    }
}