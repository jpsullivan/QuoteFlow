using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using QuoteFlow.Models.Search.Jql.Query.History;

namespace QuoteFlow.Models.Search.Jql.Query.Clause
{
    /// <summary>
    /// Implementation of ChangedClasue
    /// 
    /// @since v5.0
    /// </summary>
    public class ChangedClause : IChangedClause
    {
        public string Field { get; set; }
        public IHistoryPredicate Predicate { get; set; }
        public IHistoryPredicate HistoryPredicate { get; set; }
        public Operator Operator { get; set; }
        public IEnumerable<IClause> SubClauses { get; set; }

        private const string CHANGED = "CHANGED";

        public ChangedClause(string field, Operator @operator, IHistoryPredicate historyPredicate)
        {
            Field = field;
            Operator = @operator;
            HistoryPredicate = historyPredicate;
        }

        public ChangedClause(IChangedClause clause)
        {
            Field = clause.Field;
            HistoryPredicate = clause.Predicate;
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
            var sb = (new StringBuilder("{")).Append(Field);

            sb.Append(" ").Append("changed");
            if (HistoryPredicate != null)
            {
                sb.Append(" ").Append(HistoryPredicate.DisplayString);
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
            if (o == null || this.GetType() != o.GetType())
            {
                return false;
            }

            var that = (ChangedClause) o;

            if (!Field.Equals(that.Field))
            {
                return false;
            }
            if ((HistoryPredicate != null) && !HistoryPredicate.Equals(that.HistoryPredicate))
            {
                return false;
            }
            return true;
        }

        public override int GetHashCode()
        {
            int result = Field.GetHashCode();
            result = 31 * result + (HistoryPredicate == null ? 0 : HistoryPredicate.GetHashCode());
            return result;
        }
    }
}