using System.Collections.Generic;
using System.Text;
using QuoteFlow.Api.Jql.Query;
using QuoteFlow.Api.Jql.Query.Clause;
using QuoteFlow.Api.Jql.Query.History;
using QuoteFlow.Api.Jql.Query.Operand;

namespace QuoteFlow.Core.Jql.Query.Clause
{
    /// <summary>
    /// Represents the clause for the "WAS mode" of change history querying. This clause selects issues on the basis of a
    /// previous value of a field being equal to a given value (the 'operand').
    /// </summary>
    public sealed class WasClause : IWasClause
    {
        public string Field { get; set; }
        public IOperand Operand { get; private set; }
        public Operator Operator { get; private set; }
        public IEnumerable<Property> Property { get; private set; }
        public IHistoryPredicate Predicate { get; set; }
        public IEnumerable<IClause> SubClauses { get; set; }

        public string DisplayString { get { return "was";  } }

        public WasClause(string field, Operator @operator, IOperand operand, IHistoryPredicate predicate)
        {
            Field = field;
            Operand = operand;
            Operator = @operator;
            Predicate = predicate;
        }

        public WasClause(IWasClause clause)
        {
            Field = clause.Field;
            Operand = clause.Operand;
            Predicate = clause.Predicate;
            Operator = clause.Operator;
        }

        public string Name { get; private set; }
        public IEnumerable<IClause> Clauses { get; private set; }

        public T Accept<T>(IClauseVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }

        public override string ToString()
        {
            // The '{' brackets in this method are designed to make this method return invalid JQL so that we know when
            // we call this method. This method is only here for debugging and should not be used in production.
            var sb = (new StringBuilder("{")).Append(Name);

            sb.Append(" ").Append("was").Append(" ").Append(Operand.DisplayString);
            if (Predicate != null)
            {
                sb.Append(" ").Append(Predicate.DisplayString);
            }
            sb.Append("}");
            return sb.ToString();
        }

        public override bool Equals(object o)
        {
            if (this == o)
            {
                return true;
            }
            if (o == null || GetType() != o.GetType())
            {
                return false;
            }

            var that = (WasClause) o;

            if (!Field.Equals(that.Field))
            {
                return false;
            }
            if (!Operand.Equals(that.Operand))
            {
                return false;
            }
            if ((Predicate != null) && !Predicate.Equals(that.Predicate))
            {
                return false;
            }
            return true;
        }

        public override int GetHashCode()
        {
            int result = Operand.GetHashCode();
            if (Predicate != null)
            {
                result = 31 * result + (Predicate == null ? 0 : Predicate.GetHashCode());
            }
            result = 31 * result + Field.GetHashCode();
            return result;
        }
    }
}