using System;
using System.Collections.Generic;
using QuoteFlow.Api.Asset.Comparator;
using QuoteFlow.Api.Asset.Search;
using QuoteFlow.Api.Asset.Search.Constants;
using QuoteFlow.Api.Jql.Query;
using QuoteFlow.Api.Jql.Query.Clause;
using QuoteFlow.Api.Jql.Query.Operand;
using QuoteFlow.Api.Jql.Resolver;
using QuoteFlow.Api.Models;
using QuoteFlow.Api.Services;

namespace QuoteFlow.Api.Asset.Statistics
{
    public class UserStatisticsMapper : IStatisticsMapper<User>
    {
        private readonly string _clauseName;
        private readonly string _emptyIndexValue;
        private readonly IUserService _userService;

        public UserStatisticsMapper(UserFieldSearchConstantsWithEmpty searchConstants, IUserService userService)
            : this(searchConstants.JqlClauseNames.PrimaryName, searchConstants.EmptyIndexValue, searchConstants.IndexField, userService)
        {
        }

        public UserStatisticsMapper(string clauseName, string emptyIndexValue, string indexedField, IUserService userService)
        {
            if (string.IsNullOrEmpty(clauseName))
            {
                throw new ArgumentException(nameof(clauseName));
            }

            if (string.IsNullOrEmpty(indexedField))
            {
                throw new ArgumentException(nameof(indexedField));
            }

            _clauseName = clauseName;
            DocumentConstant = indexedField;
            _emptyIndexValue = emptyIndexValue;
            _userService = userService;
        }

        protected string ClauseName { get; }

        public string DocumentConstant { get; }

        ITerminalClause GetUserClause(string name)
        {
            return new TerminalClause(_clauseName, Operator.EQUALS, name);
        }

        ITerminalClause GetEmptyUserClause()
        {
            return new TerminalClause(_clauseName, Operator.IS, EmptyOperand.Empty);
        }

        public User GetValueFromLuceneField(string documentValue)
        {
            if ((_emptyIndexValue != null) && _emptyIndexValue.Equals(documentValue))
            {
                return null;
            }
            if (documentValue == null)
            {
                return null;
            }

            return _userService.GetUser(documentValue);
        }

        public IComparer<User> Comparator => new UserCachingComparer();

        public bool IsValidValue(User value)
        {
            return true;
        }

        public bool FieldAlwaysPartOfAnAsset => true;

        public SearchRequest GetSearchUrlSuffix(User value, SearchRequest searchRequest)
        {
            throw new System.NotImplementedException();
        }

        protected bool Equals(UserStatisticsMapper other)
        {
            return string.Equals(_clauseName, other._clauseName) &&
                   string.Equals(DocumentConstant, other.DocumentConstant);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((UserStatisticsMapper) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((_clauseName != null ? _clauseName.GetHashCode() : 0)*397) ^ (DocumentConstant != null ? DocumentConstant.GetHashCode() : 0);
            }
        }
    }
}