using System;
using QuoteFlow.Api.Models;

namespace QuoteFlow.Core.Search
{
    /// <summary>
    /// Matcher to compare User parts (username, Full Name and email) with a 
    /// query string and return true any part matches.
    /// </summary>
    public class UserMatcherPredicate
    {
        private readonly bool _canMatchAddresses;
        private readonly string _emailQuery;
        private readonly string _query;

        /// <param name="query">
        ///     The query to compare. _query can not be null.  Empty string will return true for all, so don't pass
        ///     one in.
        /// </param>
        /// <param name="canMatchAddresses"> Whether email should be searched </param>
        public UserMatcherPredicate(string query, bool canMatchAddresses) : this(query, "", canMatchAddresses)
        {
        }

        public UserMatcherPredicate(string nameQuery, string emailQuery, bool canMatchAddresses)
        {
            if (nameQuery == null) throw new ArgumentNullException(nameof(nameQuery));
            if (emailQuery == null) throw new ArgumentNullException(nameof(emailQuery));

            _query = nameQuery.ToLower();
            _emailQuery = emailQuery.ToLower();
            _canMatchAddresses = canMatchAddresses;
        }

        public bool Invoke(User user)
        {
            // NOTE - we don't test against blank or null strings here. Do that once before the code that calls this method.
            var separateEmailQuery = !string.IsNullOrEmpty(_emailQuery);

            var usernameMatched = false;
            // 1. Try the username
            var userPart = user.Username;
            if (!string.IsNullOrEmpty(userPart) && StartsWithCaseInsensitive(userPart, _query))
            {
                if (separateEmailQuery && _canMatchAddresses)
                {
                    // still need to check emailQuery against email
                    usernameMatched = true;
                }
                else
                {
                    return true;
                }
            }

            // 2. If allowed, try the User's email address
            //    at this point, either username not matched, or username matched but we need to match email separately
            if (_canMatchAddresses)
            {
                userPart = user.EmailAddress;
                if (!string.IsNullOrEmpty(userPart) &&
                    StartsWithCaseInsensitive(userPart, separateEmailQuery ? _emailQuery : _query))
                {
                    if (!separateEmailQuery || usernameMatched)
                    {
                        // email matched using name query or email matched using email query and username already matched
                        return true;
                    }
                    // email separately matched but username not match, we want to try display name
                }
                else
                {
                    if (separateEmailQuery)
                    {
                        return false; // when emailQuery is explicitly specified, it must match
                    }
                }
            }

            // 3. at this point, email matching is not required, or matched, but username not matched, just need to check display name
            userPart = user.FullName;
            if (!string.IsNullOrEmpty(userPart))
            {
                // 3a. Go for a quick win with the start of the first name...
                if (StartsWithCaseInsensitive(userPart, _query))
                {
                    return true;
                }

                // 3b. No? It was worth a try. Walk every word in the name, skip first token, we know it failed
                var tokens = userPart.Split(' ');
                for (var i = 1; i < tokens.Length; i++)
                {
                    var token = tokens[i];
                    if (StartsWithCaseInsensitive(token, _query))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private static bool StartsWithCaseInsensitive(string userPart, string query)
        {
            // We try and delay any lower case conversion in this class to as little and as later as possible.
            if (query.Length == 0)
            {
                return true;
            }
            if (query.Length > userPart.Length)
            {
                return false;
            }

            // > 90% of searches will typically miss on the first character.
            if (userPart.Substring(0, 1).ToLower()[0] == query[0])
            {
                return userPart.Substring(0, query.Length).ToLower().Equals(query);
            }
            return false;
        }
    }
}