using System;
using System.Collections.Generic;
using QuoteFlow.Api.Models;

namespace QuoteFlow.Api.Asset.Comparator
{
    /// <summary>
    /// This comparer tries to compare two users based on their "best name" ie their
    /// full name if possible, otherwise their username.
    /// 
    /// This comparer completely ignores case.
    /// </summary>
    public class UserCachingComparer : IComparer<User>
    {
        public virtual int Compare(User user1, User user2)
        {
            //noinspection ObjectEquality
            if (user1 == user2)
            {
                return 0;
            }
            if (user2 == null)
            {
                return -1;
            }
            if (user1 == null)
            {
                return 1;
            }

            int fullNameComparison = string.Compare(user1.FullName, user2.FullName, StringComparison.OrdinalIgnoreCase);
            if (fullNameComparison == 0) //if full names are the same, we should check the username
            {
                return string.CompareOrdinal(user1.Username, user2.Username);
            }

            return fullNameComparison;
        }
    }
}