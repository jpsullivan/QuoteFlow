using System.Collections.Generic;
using System.Text;

namespace QuoteFlow.Models.Search.Jql.Query.History
{
    /// <summary>
    /// Represents a sequence of <seealso cref="HistoryPredicate HistoryPredicates"/> composed with AND operators such that the
    /// resulting predicate is true only if ALL of the composed predicates are true.

    /// </summary>
    public class AndHistoryPredicate : IHistoryPredicate
    {
        public IEnumerable<IHistoryPredicate> Predicates { get; private set; } 

        public AndHistoryPredicate(IEnumerable<IHistoryPredicate> predicates)
        {
            Predicates = new List<IHistoryPredicate>(predicates);
        }

        public virtual string DisplayString
        {
            get
            {
                var sb = new StringBuilder();
                foreach (var predicate in Predicates)
                {
                    sb.Append(predicate.DisplayString).Append(" ");
                }
                return sb.ToString();
            }
        }

        public T Accept<T>(IPredicateVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }
    }
}