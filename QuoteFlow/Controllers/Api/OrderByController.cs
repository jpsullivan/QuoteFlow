using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using QuoteFlow.Api.Asset.Fields;
using QuoteFlow.Api.Jql.Parser;
using QuoteFlow.Api.Jql.Query;
using QuoteFlow.Api.Jql.Query.Order;
using QuoteFlow.Api.Jql.Values;
using QuoteFlow.Api.Models.OrderBy;
using QuoteFlow.Api.OrderBy;
using QuoteFlow.Core.Jql.Builder;

namespace QuoteFlow.Controllers.Api
{
    public class OrderByController : ApiController
    {
        public IFieldManager FieldManager { get; protected set; }
        public ISortJqlGenerator SortJqlGenerator { get; protected set; }
        public IJqlQueryParser JqlQueryParser { get; protected set; }
        public IOrderByUtil OrderByUtil { get; protected set; }

        public OrderByController(IFieldManager fieldManager, ISortJqlGenerator sortJqlGenerator, IJqlQueryParser jqlQueryParser, IOrderByUtil orderByUtil)
        {
            FieldManager = fieldManager;
            SortJqlGenerator = sortJqlGenerator;
            JqlQueryParser = jqlQueryParser;
            OrderByUtil = orderByUtil;
        }

        private const int MaxResultsHardLimit = 50;

        [Route("api/orderby/orderbyoptions"), HttpPost]
        public OrderByOptions GetOrderByOptions([FromBody] OrderByRequest request)
        {
            var effectiveMaxResults = Math.Min(request.MaxResults, MaxResultsHardLimit);
            try
            {
                var query = GetQuery(request.Jql);
                var fields = new SuggestedFields(GetSortableColumns());

                // filter by field name prefix from the suggestions
                SuggestedFields matchingFields = fields;
                if (!string.IsNullOrEmpty(request.Query))
                {
                    matchingFields = matchingFields.FilterBy(field => field?.Name != null && field.Name.ToLower().StartsWith(request.Query));
                }

                // remove the current sortBy field from the suggestions
                if (!string.IsNullOrEmpty(request.SortBy))
                {
                    matchingFields = matchingFields.FilterBy(field => field != null && !request.SortBy.Equals(field.Id));
                }

                // return the top N fields that match
                var topMatchingFields = matchingFields.SortBy(new NameComparer()).SelectTop(effectiveMaxResults);

                var orderOptions = BuildOrderOptions(topMatchingFields.Fields, query);
                return new OrderByOptions
                {
                    Fields = orderOptions,
                    TotalCount = fields.Count(),
                    MatchesCount = matchingFields.Count(),
                    MaxResults = effectiveMaxResults
                };

            }
            catch (JqlParseException e)
            {
                throw new InvalidOperationException("Error parsing JQL: " + request.Jql, e);
            }
        }

        private IQuery GetQuery(string jql)
        {
            if (!string.IsNullOrEmpty(jql))
            {
                return JqlQueryParser.ParseQuery(jql);
            }

            // default to an empty query
            return JqlQueryBuilder.NewBuilder().BuildQuery();
        }

        private List<INavigableField> GetSortableColumns()
        {
            try
            {
                var navigableFields = new List<INavigableField>(FieldManager.AllAvailableNavigableFields());
                return navigableFields.FindAll(_fieldIsSortable);
            }
            catch (FieldException ex)
            {
                throw new InvalidOperationException("Error getting all navigable fields.", ex);
            }
        } 

        private IEnumerable<OrderByOption> BuildOrderOptions(IReadOnlyCollection<INavigableField> fields, IQuery query)
        {
            if (fields == null || !fields.Any())
            {
                return new List<OrderByOption>();
            }

            var sortJql = SortJqlGenerator.GenerateColumnSortJql(query, fields);
            var orderOptions = new List<OrderByOption>(fields.Count);
            foreach (var field in fields)
            {
                if (!sortJql.ContainsKey(field.Id))
                {
                    Console.WriteLine("Couldn't generate sort JQL for field '{0}', and query: {1}", field.Id, query);
                }

                orderOptions.Add(new OrderByOption
                {
                    FieldId = field.Id,
                    FieldName = field.Name,
                    SortJql = sortJql[field.Id]
                });
            }

            return orderOptions;
        }

        private readonly Predicate<INavigableField> _fieldIsSortable = field => field?.Sorter != null;
    }
}
