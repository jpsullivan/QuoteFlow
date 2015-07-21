using System;
using System.Collections.Generic;
using System.Linq;
using Lucene.Net.Index;
using Lucene.Net.Search;
using QuoteFlow.Api.Asset.Index;
using QuoteFlow.Api.Jql.Operand;
using QuoteFlow.Api.Jql.Operator;
using QuoteFlow.Api.Jql.Query;
using QuoteFlow.Api.Jql.Query.History;
using QuoteFlow.Api.Jql.Query.Operand;
using QuoteFlow.Api.Jql.Resolver;
using QuoteFlow.Api.Jql.Util;
using QuoteFlow.Api.Models;
using QuoteFlow.Core.Jql.Query.History;

namespace QuoteFlow.Core.Jql.Query
{
	public class HistoryPredicateQueryFactory
	{
		private readonly PredicateOperandResolver _predicateOperandResolver;
		private readonly IJqlDateSupport _jqlDateSupport;
		private readonly ChangeHistoryFieldIdResolver _changeHistoryFieldIdResolver;

	    private static readonly DateTime MaxDate = DateTime.MaxValue;
		private static readonly DateTime MinDate = new DateTime();
		private static readonly global::Lucene.Net.Search.Query FalseQuery = new BooleanQuery();

		/// <summary>
		/// 
		/// </summary>
        /// <param name="predicateOperandResolver">Resolves <see cref="IHistoryPredicate"/> values.</param>
        /// <param name="jqlDateSupport">Parses SQL dates.</param>
        /// <param name="changeHistoryFieldIdResolver">Resolves historic values to their corresponding IDs.</param>
		public HistoryPredicateQueryFactory(PredicateOperandResolver predicateOperandResolver, IJqlDateSupport jqlDateSupport, ChangeHistoryFieldIdResolver changeHistoryFieldIdResolver)
		{
			_predicateOperandResolver = predicateOperandResolver;
			_jqlDateSupport = jqlDateSupport;
			_changeHistoryFieldIdResolver = changeHistoryFieldIdResolver;
		}

		public virtual BooleanQuery MakePredicateQuery(User searcher, string field, IHistoryPredicate historyPredicate, bool isChangedSearch)
		{
			var predicateQuery = new BooleanQuery();
			var terminalPredicates = new List<TerminalHistoryPredicate>();
			if (historyPredicate is AndHistoryPredicate)
			{
				foreach (IHistoryPredicate predicate in ((AndHistoryPredicate) historyPredicate).Predicates)
				{
					terminalPredicates.Add((TerminalHistoryPredicate) predicate);
				}
			}
			else
			{
				terminalPredicates.Add((TerminalHistoryPredicate) historyPredicate);
			}
			foreach (var predicate in terminalPredicates)
			{
				MakeBooleanQuery(searcher, field, predicate, predicateQuery, isChangedSearch);
			}
			return predicateQuery;
		}

		private void MakeBooleanQuery(User searcher, string field, TerminalHistoryPredicate predicate, BooleanQuery predicateQuery, bool isChangedSearch)
		{
		    if (predicate == null)
		    {
		        throw new ArgumentNullException(nameof(predicate));
		    }

		    if (predicateQuery == null)
		    {
		        throw new ArgumentNullException(nameof(predicateQuery));
		    }

			var @operator = predicate.Operator;
			var operand = predicate.Operand;
			var operandValues = GetValuesForOperatorAndOperand(searcher, field, @operator, operand).ToList();
			if (!operandValues.Any())
			{
				return;
			}

			if (Operator.BY == @operator)
			{
				MakeByQuery(predicateQuery, operandValues);
			}
			if (Operator.TO == @operator)
			{
				MakeChangedQuery(field, predicateQuery, operandValues, true);
			}
			if (Operator.FROM == @operator)
			{
				MakeChangedQuery(field, predicateQuery, operandValues, false);
			}
			if (OperatorClasses.ChangeHistoryDatePredicates.Contains(@operator))
			{
				if (Operator.DURING == @operator)
				{
					MakeDuringQuery(predicateQuery, operandValues, field, isChangedSearch);
				}
				else
				{
					if (Operator.ON == @operator)
					{
						MakeOnQuery(predicateQuery, operandValues, field, isChangedSearch);
					}
					else
					{
						MakeBeforEorAfterQuery(@operator, predicateQuery, operandValues, field, isChangedSearch);
					}
				}
			}
		}

		private void MakeChangedQuery(string fieldName, BooleanQuery predicateQuery, IEnumerable<QueryLiteral> operandValues, bool isUpperBounds)
		{
			var changedQuery = new BooleanQuery();
			foreach (var literal in operandValues)
			{
				ICollection<string> ids = _changeHistoryFieldIdResolver.ResolveIdsForField(fieldName, literal, literal.IsEmpty);
				if (ids == null || ids.Count == 0)
				{
					string value = (literal.IntValue != null) ? literal.IntValue.ToString() : literal.StringValue;
					// If we can't match the id to a current valid value then we just search with the literal. It may have been
					// a valid value once upon a time, Of course we may still find nothing matches
					string documentField = isUpperBounds ? DocumentConstants.ChangeTo : DocumentConstants.ChangeFrom;
					changedQuery.Add(CreateTermQuery(fieldName, documentField, EncodeProtocol(value)), Occur.SHOULD);
				}
				else
				{
					foreach (var id in ids)
					{
						string documentField = isUpperBounds ? DocumentConstants.NewValue : DocumentConstants.OldValue;
						changedQuery.Add(CreateTermQuery(fieldName, documentField, EncodeProtocolPreservingCase(id)), Occur.SHOULD);
					}
				}
			}
			predicateQuery.Add(changedQuery, Occur.MUST);
		}

		private static TermQuery CreateTermQuery(string fieldName, string documentField, string value)
		{
			return new TermQuery(new Term(string.Format("{0}.{1}", fieldName.ToLower(), documentField), value));
		}

		private void MakeByQuery(BooleanQuery predicateQuery, IEnumerable<QueryLiteral> operandValues)
		{
			var userQuery = new BooleanQuery();
			foreach (var literal in operandValues)
			{
				string userName = literal.IntValue == null ? literal.StringValue : literal.IntValue.ToString();
                userQuery.Add(new TermQuery(new Term(DocumentConstants.ChangeActioner, EncodeProtocolPreservingCase(userName))), Occur.SHOULD);
			}
			predicateQuery.Add(userQuery, Occur.MUST);
		}

		private void MakeDuringQuery(BooleanQuery predicateQuery, IEnumerable<QueryLiteral> operandValues, string field, bool isChangedSearch)
		{
			// make it resilient in the face of bad AST
			if (operandValues.Count() < 2)
			{
				// this got past validation but we need to be resilient in the face of adversity so false it is
				predicateQuery.Add(FalseQuery, Occur.MUST); // << -- or them together
			}
			else
			{
				var bottomBoundDateRange = ConvertToDateRangeWithImpliedPrecision(operandValues.ElementAt(0));
				var upperBoundDateRange = ConvertToDateRangeWithImpliedPrecision(operandValues.ElementAt(1));

				if (bottomBoundDateRange == null || upperBoundDateRange == null)
				{
					// this got past validation but we need to be resilient in the face of adversity so false it is
					predicateQuery.Add(FalseQuery, Occur.MUST); // << -- or them together
				}
				else
				{
					MakeInclusiveQueryBasedOnDates(predicateQuery, field, bottomBoundDateRange.LowerDate, upperBoundDateRange.UpperDate, isChangedSearch);
				}
			}
		}

		private void MakeOnQuery(BooleanQuery predicateQuery, IEnumerable<QueryLiteral> operandValues, string field, bool isChangedSearch)
		{
			var query = new BooleanQuery();
			foreach (var literal in operandValues)
			{
				var dateRange = ConvertToDateRangeWithImpliedPrecision(literal);
				if (dateRange == null)
				{
					// this got past validation but we need to be resilient in the face of adversity so false it is
					query.Add(FalseQuery, Occur.MUST); // << -- or them together
					return;
				}
			    var condition = new BooleanQuery();

			    MakeInclusiveQueryBasedOnDates(condition, field, dateRange.LowerDate, dateRange.UpperDate, isChangedSearch);
			    query.Add(condition, Occur.SHOULD); // << -- or them together
			}
			predicateQuery.Add(query, Occur.MUST);
		}

		private static DateTime AddOneUnit(DateTime lowerDate)
		{
		    return lowerDate.AddSeconds(1);
		}

		private void MakeBeforEorAfterQuery(Operator @operator, BooleanQuery predicateQuery, IList<QueryLiteral> operandValues, string field, bool isChangedSearch)
		{
			var literal = operandValues[0];

			var dateRange = ConvertToDateRangeWithImpliedPrecision(literal);

			if (dateRange == null)
			{
				// this got past validation but we need to be resilient in the face of adversity so false it is
				predicateQuery.Add(FalseQuery, Occur.MUST); // << -- or them together
			}
			else if (Operator.BEFORE == @operator)
			{
				MakeExclusiveQueryBasedOnDates(predicateQuery, field, MinDate, dateRange.LowerDate, isChangedSearch);
			}
			else if (Operator.AFTER == @operator)
			{
				MakeExclusiveQueryBasedOnDates(predicateQuery, field, AddOneUnit(dateRange.UpperDate), MaxDate, isChangedSearch);
			}
		}

		public virtual void MakeExclusiveQueryBasedOnDates(BooleanQuery bq, string field, DateTime fromDate, DateTime toDate, bool isChangedSearch)
		{
			MakeTermQuery(bq, field, fromDate,toDate,false, isChangedSearch);
		}

		public virtual void MakeInclusiveQueryBasedOnDates(BooleanQuery bq, string field, DateTime fromDate, DateTime toDate, bool isChangedSearch)
		{
			MakeTermQuery(bq, field, fromDate,toDate,true, isChangedSearch);
		}

		public virtual void MakeTermQuery(BooleanQuery bq, string field, DateTime fromDate, DateTime toDate, bool inclusiveSearch, bool isChangedSearch)
		{
			//Chnaged searches work slightly diiferently to WAs searches - in this case the change itrself must happen in the date range
			if (fromDate != null && toDate != null)
			{
				//
				// when we are in inclsuive mode, we need to add one miniumum unit of time so that we form
				// a proper range for Lucence to MISS any records that are updated 1 second AFTER the lower range (ch_nextchangedate) of the
				// one we are looking for.
				//
				fromDate = (inclusiveSearch ? AddOneUnit(fromDate) : fromDate);
				string searchStart = _jqlDateSupport.GetIndexedValue(fromDate);
				string searchEnd = _jqlDateSupport.GetIndexedValue(toDate);
				// startSearch <= NEXT_CHANGE_DATE  AND  CHANGE_DATE <= searchEND
				if (isChangedSearch)
				{
					bq.Add(new TermRangeQuery(DocumentConstants.ChangeDate, searchStart, searchEnd, true, inclusiveSearch), Occur.MUST);
				}
				else
				{
					bq.Add(new TermRangeQuery(field + "." + DocumentConstants.NextChangeDate, searchStart, null, inclusiveSearch, true), Occur.MUST);
					bq.Add(new TermRangeQuery(DocumentConstants.ChangeDate, null, searchEnd, true, inclusiveSearch), Occur.MUST);
				}
			}
		}

		private DateRange ConvertToDateRangeWithImpliedPrecision(QueryLiteral literal)
		{
		    return literal == null
		        ? null
		        : literal.IntValue != null
		            ? _jqlDateSupport.ConvertToDateRange(literal.IntValue)
		            : _jqlDateSupport.ConvertToDateRangeWithImpliedPrecision(literal.StringValue);
		}

		private static string EncodeProtocol(string changeItem)
		{
			return DocumentConstants.ChangeHistoryProtocol + (changeItem == null ? "" : changeItem.ToLower());
		}

		private static string EncodeProtocolPreservingCase(string changeItem)
		{
			return DocumentConstants.ChangeHistoryProtocol + (changeItem ?? "");
		}

		private IEnumerable<QueryLiteral> GetValuesForOperatorAndOperand(User searcher, string field, Operator @operator, IOperand operand)
		{
		    if (operand is EmptyOperand && Operator.BY == @operator)
			{
			    return new List<QueryLiteral> {new QueryLiteral(operand, "")};
			}
		    return _predicateOperandResolver.GetValues(searcher, field, operand);
		}
	}
}