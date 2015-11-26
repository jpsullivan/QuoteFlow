using System;
using System.Collections.Generic;
using QuoteFlow.Api.Jql.Resolver;
using QuoteFlow.Api.Models;

namespace QuoteFlow.Core.Jql.Resolver
{
    /// <summary>
    /// Index resolver that can find the index values for users.
    /// </summary>
    public class UserIndexInfoResolver : IIndexInfoResolver<User>
    {
        private readonly INameResolver<User> _userResolver;

        public UserIndexInfoResolver(INameResolver<User> userResolver)
        {
            _userResolver = userResolver;
        }

        public List<string> GetIndexedValues(string rawValue)
        {
            if (rawValue == null)
            {
                throw new ArgumentNullException(nameof(rawValue));
            }

            return _userResolver.GetIdsFromName(rawValue);
        }

        public List<string> GetIndexedValues(int? rawValue)
        {
            if (rawValue == null)
            {
                throw new ArgumentNullException(nameof(rawValue));
            }

            return GetIndexedValues(rawValue.ToString());
        }

        public string GetIndexedValue(User indexedObject)
        {
            return indexedObject.Id.ToString();
        }
    }
}