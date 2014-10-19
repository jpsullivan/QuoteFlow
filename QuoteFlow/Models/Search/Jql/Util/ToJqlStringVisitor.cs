using System.Collections.Generic;
using System.Text;
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
			IList<IOperand> operands = multiValue.Values;
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
		    if (singleValueOperand.LongValue != null)
			{
				return singleValueOperand.LongValue.ToString();
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

		public Result Visit(TerminalClause clause)
		{
			return BuildJqlString(clause);
		}

		public override Result Visit(IWasClause clause)
		{
			return BuildJqlString(clause);
		}

		public override Result Visit(IChangedClause clause)
		{
			return BuildJqlString(clause);
		}

		public override string Visit(IHistoryPredicate predicate)
		{
			//depending on actual implementation of HistoryPredicate we should print them differently
			if (predicate is TerminalHistoryPredicate)
			{
				return visit((TerminalHistoryPredicate) predicate);
			}
		    if (predicate is AndHistoryPredicate)
		    {
		        return visit((AndHistoryPredicate) predicate);
		    }

		    // default fallback for unknown implementation
			return predicate.DisplayString;
		}

		private string Visit(TerminalHistoryPredicate predicate)
		{
			var sb = new StringBuilder();
			sb.Append(predicate.Operator.DisplayString);
			sb.Append(" ");
			sb.Append(predicate.Operand.accept(this));
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

			for (IEnumerator<IClause> clauseIterator = andClause.Clauses.GetEnumerator(); clauseIterator.MoveNext();)
			{
				IClause clause = clauseIterator.Current;
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

				if (clauseIterator.hasNext())
				{
					sb.Append(" ").Append(clauseName).Append(" ");
				}
			}
			return new Result(sb.ToString(), clausePrecedence);
		}

		private Result BuildJqlString(ITerminalClause clause)
		{
			var builder = new StringBuilder(Support.EncodeFieldName(clause.Name));
			clause.Property.@foreach(new EffectAnonymousInnerClassHelper(this, builder));
			builder.Append(" ").Append(clause.Operator.DisplayString);
			builder.Append(" ").Append(clause.Operand.Accept(this));
			if (clause is IWasClause)
			{
				IHistoryPredicate predicate = ((IWasClause) clause).Predicate;
				if (predicate != null)
				{
					builder.Append(" ").Append(predicate.Accept(this));
				}
			}

			return new Result(builder.ToString(), ClausePrecedence.TERMINAL);
		}

		private class EffectAnonymousInnerClassHelper : Effect<Property>
		{
			private readonly ToJqlStringVisitor outerInstance;

			private StringBuilder builder;

			public EffectAnonymousInnerClassHelper(ToJqlStringVisitor outerInstance, StringBuilder builder)
			{
				this.outerInstance = outerInstance;
				this.builder = builder;
			}

			public override void Apply(Property property)
			{
				builder.Append("[").Append(outerInstance.Support.EncodeFieldName(property.KeysAsString)).Append("].").Append(outerInstance.support.encodeFieldName(property.ObjectReferencesAsString));
			}
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

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: private Result(final String jql, final com.atlassian.query.clause.ClausePrecedence precedence)
			internal Result(string jql, ClausePrecedence precedence)
			{
				Jql = jql;
				Precedence = precedence;
			}

			public override string ToString()
			{
				return ToStringBuilder.reflectionToString(this, ToStringStyle.SHORT_PREFIX_STYLE);
			}
		}
	}
}