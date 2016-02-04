using System;
using QuoteFlow.Api.Jql;
using QuoteFlow.Api.Jql.Context;
using QuoteFlow.Api.Jql.Permission;
using QuoteFlow.Api.Jql.Query;
using QuoteFlow.Api.Jql.Validator;

namespace QuoteFlow.Core.Jql
{
    /// <summary>
    /// A container for all the objects needed to process a Jql clause.
    /// </summary>
    public class ClauseHandler : IClauseHandler
    {
        public IClauseInformation Information { get; private set; }
        public IClauseQueryFactory Factory { get; private set; }
        public IClauseValidator Validator { get; private set; }
        public IClausePermissionHandler PermissionHandler { get; private set; }
        public IClauseContextFactory ClauseContextFactory { get; private set; }

        public ClauseHandler(IClauseInformation information, IClauseQueryFactory factory, IClauseValidator validator, IClausePermissionHandler permissionHandler, IClauseContextFactory clauseContextFactory)
        {
            if (information == null) throw new ArgumentNullException(nameof(information));
            if (factory == null) throw new ArgumentNullException(nameof(factory));
            if (validator == null) throw new ArgumentNullException(nameof(validator));
            if (permissionHandler == null) throw new ArgumentNullException(nameof(permissionHandler));
            if (clauseContextFactory == null) throw new ArgumentNullException(nameof(clauseContextFactory));

            Information = information;
            Factory = factory;
            Validator = validator;
            PermissionHandler = permissionHandler;
            ClauseContextFactory = clauseContextFactory;
        }

        protected bool Equals(ClauseHandler other)
        {
            return Equals(Information, other.Information) && Equals(Factory, other.Factory) &&
                   Equals(Validator, other.Validator) && Equals(PermissionHandler, other.PermissionHandler) &&
                   Equals(ClauseContextFactory, other.ClauseContextFactory);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ClauseHandler) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (Information != null ? Information.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (Factory != null ? Factory.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (Validator != null ? Validator.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (PermissionHandler != null ? PermissionHandler.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (ClauseContextFactory != null ? ClauseContextFactory.GetHashCode() : 0);
                return hashCode;
            }
        }
    }
}