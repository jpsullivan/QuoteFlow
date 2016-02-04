using QuoteFlow.Api.Jql;
using QuoteFlow.Api.Jql.Context;
using QuoteFlow.Api.Jql.Permission;
using QuoteFlow.Api.Jql.Query;
using QuoteFlow.Api.Jql.Validator;
using QuoteFlow.Api.Jql.Values;

namespace QuoteFlow.Core.Jql
{
    public class ValuesGeneratingClauseHandler : IClauseHandler, IValueGeneratingClauseHandler
    {
        public IClauseValuesGenerator ValuesGenerator { get; set; }

        public IClauseInformation Information { get; private set; }
        public IClauseQueryFactory Factory { get; private set; }
        public IClauseValidator Validator { get; private set; }
        public IClausePermissionHandler PermissionHandler { get; private set; }
        public IClauseContextFactory ClauseContextFactory { get; private set; }

        public ValuesGeneratingClauseHandler(
            IClauseInformation information, 
            IClauseQueryFactory factory, 
            IClauseValidator validator, 
            IClauseContextFactory clauseContextFactory, 
            IClausePermissionHandler permissionHandler,
            IClauseValuesGenerator valuesGenerator)
        {
            Information = information;
            Factory = factory;
            Validator = validator;
            ClauseContextFactory = clauseContextFactory;
            PermissionHandler = permissionHandler;
            ValuesGenerator = valuesGenerator;
        }

        public IClauseValuesGenerator GetClauseValuesGenerator()
        {
            return ValuesGenerator;
        }

        protected bool Equals(ValuesGeneratingClauseHandler other)
        {
            return Equals(ValuesGenerator, other.ValuesGenerator) && Equals(Information, other.Information) &&
                   Equals(Factory, other.Factory) && Equals(Validator, other.Validator) &&
                   Equals(PermissionHandler, other.PermissionHandler) &&
                   Equals(ClauseContextFactory, other.ClauseContextFactory);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ValuesGeneratingClauseHandler) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (ValuesGenerator != null ? ValuesGenerator.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (Information != null ? Information.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (Factory != null ? Factory.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (Validator != null ? Validator.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (PermissionHandler != null ? PermissionHandler.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (ClauseContextFactory != null ? ClauseContextFactory.GetHashCode() : 0);
                return hashCode;
            }
        }
    }
}