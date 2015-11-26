using System.Collections.Generic;
using System.Collections.Immutable;

namespace QuoteFlow.Api.Search
{
    /// <summary>
	/// Optional parameters to restrict a user search.
	/// 
	/// This allows you to include or exclude active and inactive users and allow 
	/// or disallow empty search queries.
	/// </summary>
    public class UserSearchParams
    {
        public static readonly UserSearchParams ActiveUsersIgnoreEmptyQuery = new UserSearchParams(false, true, false);
        public static readonly UserSearchParams ActiveUsersAllowEmptyQuery = new UserSearchParams(true, true, false);

        public UserSearchParams(bool allowEmptyQuery, bool includeActive, bool includeInactive, bool canMatchEmail = false, ISet<int?> catalogIds = null)
        {
            AllowEmptyQuery = allowEmptyQuery;
            IncludeActive = includeActive;
            IncludeInactive = includeInactive;
            CanMatchEmail = canMatchEmail;
            CatalogIds = catalogIds;
        }

        public virtual bool AllowEmptyQuery { get; }

        public virtual bool IncludeActive { get; }

        public virtual bool IncludeInactive { get; }

        /// <summary>
        /// Indicate whether the search would apply the query to email address as well.
        /// </summary>
        public bool CanMatchEmail { get; }

        private ISet<int?> CatalogIds { get; }

        public static Builder builder(UserSearchParams prototype)
        {
            return new Builder(prototype);
        }

        public class Builder
        {
            private bool _allowEmptyQuery;
            private bool _includeActive = true;
            private bool _includeInactive;
            private bool _canMatchEmail;
            private ISet<int?> _catalogIds;

            public Builder()
            {
            }

            internal Builder(UserSearchParams prototype)
            {
                _allowEmptyQuery = prototype.AllowEmptyQuery;
                _includeActive = prototype.IncludeActive;
                _includeInactive = prototype.IncludeInactive;
                _canMatchEmail = prototype.CanMatchEmail;
                _catalogIds = prototype.CatalogIds;
            }

            public virtual UserSearchParams Build()
            {
                return new UserSearchParams(_allowEmptyQuery, _includeActive, _includeInactive, _canMatchEmail, _catalogIds);
            }

            public virtual Builder AllowEmptyQuery(bool allowEmptyQuery)
            {
                _allowEmptyQuery = allowEmptyQuery;
                return this;
            }

            public virtual Builder IncludeActive(bool includeActive)
            {
                _includeActive = includeActive;
                return this;
            }

            public virtual Builder IncludeInactive(bool includeInactive)
            {
                _includeInactive = includeInactive;
                return this;
            }

            public virtual Builder CanMatchEmail(bool canMatchEmail)
            {
                _canMatchEmail = canMatchEmail;
                return this;
            }

            public virtual Builder FilterByCatalogIds(ICollection<int?> catalogIds)
            {
                _catalogIds = catalogIds == null ? null : ImmutableHashSet.CreateRange(catalogIds);
                return this;
            }
        }
    }
}