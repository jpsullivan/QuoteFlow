using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QuoteFlow.Infrastructure.Extensions;
using QuoteFlow.Models.Search.Jql.Query;
using QuoteFlow.Models.Search.Jql.Query.Clause;
using QuoteFlow.Models.Search.Jql.Query.History;
using QuoteFlow.Models.Search.Jql.Query.Operand;

namespace QuoteFlow.Models.Search.Jql.Util
{
	/// <summary>
	/// Turns a JQL search into a valid JQL string. Will perform escaping as necessary during the process.
	/// </summary>
	internal sealed class ToJqlStringVisitor : IOperandVisitor<string>, IClauseVisitor<ToJqlStringVisitor.Result>, IPredicateVisitor<string>
	{
        private JqlStringSupport Support { get; set; }

		internal ToJqlStringVisitor(JqlStringSupport support)
		{
			Support = support;
		}

		internal string ToJqlString(IClause clause)
		{
			return clause.Accept(this).Jql;
		}

		public string Visit(EmptyOperand empty)
		{
			return EmptyOperand.OPERAND_NAME;
		}

		public string Visit(FunctionOperand function)
		{
			var sb = new StringBuilder(Support.EncodeFunctionName(function.Name));
			sb.Append("(");

			IList<string> args = function.Args;
			bool first = true;
			foreach (string arg in args)
			{
				if (!first)
				{
					sb.Append(", ");
				}
				first = false;

				sb.Append(Support.EncodeFunctionArgument(arg));
			}
			sb.Append(")");
			return sb.ToString();
		}

		public string Visit(MultiValueOperand multiValue)
		{
			var sb = new StringBuilder("(");
			var operands = multiValue.Values;
			bool first = true;
			foreach (var operand in operands)
			{
				if (!first)
				{
					sb.Append(", ");
				}
				first = false;

				sb.Append(operand.Accept(this));
			}
			sb.Append(")");
			return sb.ToString();
		}

		public string Visit(SingleValueOperand singleValueOperand)
		{
		    if (singleValueOperand.IntValue != null)
			{
				return singleValueOperand.IntValue.ToString();
			}
		    return Support.EncodeStringValue(singleValueOperand.StringValue);
		}

		public Result Visit(AndClause andClause)
		{
			return VisitMultiClause(andClause, AndClause.And, ClausePrecedence.AND);
		}

		public Result Visit(OrClause orClause)
		{
			return VisitMultiClause(orClause, OrClause.Or, ClausePrecedence.OR);
		}

		public Result Visit(NotClause notClause)
		{
			Result subResult = notClause.SubClause.Accept(this);
			bool brackets = subResult.Precedence.CompareTo(ClausePrecedence.NOT) < 0;
		    string jql = string.Format(brackets ? "{0} ({1})" : "{0} {1}", NotClause.NOT, subResult.Jql);
		    return new Result(jql, ClausePrecedence.NOT);
		}

		public Result Visit(ITerminalClause clause)
		{
			return BuildJqlString(clause);
		}

		public Result Visit(IWasClause clause)
		{
			return BuildJqlString(clause);
		}

		public Result Visit(IChangedClause clause)
		{
			return BuildJqlString(clause);
		}

		public string Visit(IHistoryPredicate predicate)
		{
			//depending on actual implementation of HistoryPredicate we should print them differently
		    var historyPredicate = predicate as TerminalHistoryPredicate;
		    if (historyPredicate != null)
			{
				return Visit(historyPredicate);
			}
		    var andHistoryPredicate = predicate as AndHistoryPredicate;
		    if (andHistoryPredicate != null)
		    {
		        return Visit(andHistoryPredicate);
		    }

		    // default fallback for unknown implementation
			return predicate.DisplayString;
		}

		private string Visit(TerminalHistoryPredicate predicate)
		{
			var sb = new StringBuilder();
			sb.Append(predicate.Operator.GetDisplayAttributeFrom(typeof(Operator)));
			sb.Append(" ");
			sb.Append(predicate.Operand.Accept(this));
			return sb.ToString();
		}

		private string Visit(AndHistoryPredicate predicate)
		{
			var sb = new StringBuilder();
			foreach (IHistoryPredicate p in predicate.Predicates)
			{
				sb.Append(p.Accept(this)).Append(" ");
			}
			return sb.ToString();
		}

		private Result VisitMultiClause(MultiClause andClause, string clauseName, ClausePrecedence clausePrecedence)
		{
			var sb = new StringBuilder();

		    for (int i = 0; i < andClause.Clauses.Count(); i++)
		    {
                IClause clause = andClause.Clauses.ElementAt(i);
                Result clauseResult = clause.Accept(this);

                bool brackets = clauseResult.Precedence.CompareTo(clausePrecedence) < 0;

                if (brackets)
                {
                    sb.Append("(");
                }
                sb.Append(clauseResult.Jql);
                
                if (brackets)
                {
                    sb.Append(")");
                }

		        if (i != andClause.Clauses.Count() - 1)
		        {
                    sb.Append(" ").Append(clauseName).Append(" ");
		        }
		    }

			return new Result(sb.ToString(), clausePrecedence);
		}

		private Result BuildJqlString(ITerminalClause clause)
		{
			var builder = new StringBuilder(Support.EncodeFieldName(clause.Name));

		    foreach (var property in clause.Property)
		    {
                builder.Append("[")
                        .Append(Support.EncodeFieldName(property.KeysAsString()))
                        .Append("].")
                        .Append(Support.EncodeFieldName(property.ObjectReferencesAsString()));
		    }

			builder.Append(" ").Append(clause.Operator.GetDisplayAttributeFrom(typeof(Operator)));
			builder.Append(" ").Append(clause.Operand.Accept(this));
		    
            if (!(clause is IWasClause)) return new Result(builder.ToString(), ClausePrecedence.TERMINAL);
		    
            IHistoryPredicate predicate = ((IWasClause) clause).Predicate;
		    if (predicate != null)
		    {
		        builder.Append(" ").Append(predicate.Accept(this));
		    }

		    return new Result(builder.ToString(), ClausePrecedence.TERMINAL);
		}

		private Result BuildJqlString(IChangedClause clause)
		{
			var builder = new StringBuilder(Support.EncodeFieldName(clause.Field));
			builder.Append(" ").Append("changed");
			if (clause.Predicate != null)
			{
			   builder.Append(" ");
			   builder.Append(clause.Predicate.Accept(this));
			}
			return new Result(builder.ToString(), ClausePrecedence.TERMINAL);
		}

		public sealed class Result
		{
            public string Jql { get; private set; }
            public ClausePrecedence Precedence { get; private set; }

			internal Result(string jql, ClausePrecedence precedence)
			{
				Jql = jql;
				Precedence = precedence;
			}

		}
	}
}