using System;
using System.Collections.Generic;
using System.Linq;
using QuoteFlow.Models.Search.Jql.Operand;
using QuoteFlow.Models.Search.Jql.Query;
using QuoteFlow.Models.Search.Jql.Query.Clause;
using QuoteFlow.Services.Interfaces;

namespace QuoteFlow.Models.Search.Jql.Context
{
    /// <summary>
    /// Generates a <seealso cref="ClauseContext"/> based on the manufacturer values and 
    /// the catalog they are visible in.
    /// </summary>
    public class ManufacturerClauseContextFactory : IClauseContextFactory
    {
        public IJqlOperandResolver JqlOperandResolver { get; protected set; }
        public IManufacturerService ManufacturerService { get; protected set; }

        public ManufacturerClauseContextFactory(IJqlOperandResolver jqlOperandResolver, IManufacturerService manufacturerService)
        {
            JqlOperandResolver = jqlOperandResolver;
            ManufacturerService = manufacturerService;
        }

        public IClauseContext GetClauseContext(User searcher, ITerminalClause terminalClause)
        {
            Operator @operator = terminalClause.Operator;
			if (!handlesOperator(@operator))
			{
				return ClauseContext.CreateGlobalClauseContext();
			}

			var values = JqlOperandResolver.GetValues(searcher, terminalClause.Operand, terminalClause);
			ISet<int> manufacturerIds = new HashSet<int>();
			if (values != null)
			{
				foreach (QueryLiteral value in values)
				{
					// if we have an empty literal, the Global context will not impact on any existing contexts, so do nothing
					if (!value.IsEmpty)
					{
					    foreach (var id in GetIds(value))
					    {
					        manufacturerIds.Add(id);
					    }
					}
				}
			}

			if (manufacturerIds.Any() && isNegationOperator(@operator))
			{
			    var ids = ManufacturerService.GetManufacturers(0).Select(m => m.Id);
			    ISet<int> allManufacturerIds = new HashSet<int>(ids);
			    foreach (var id in manufacturerIds)
			    {
			        allManufacturerIds.Remove(id);
			    }
				manufacturerIds = allManufacturerIds;
			}

			if (!manufacturerIds.Any())
			{
				return ClauseContext.CreateGlobalClauseContext();
			}

			ISet<ICatalogManufacturerContext> contexts = new HashSet<ICatalogManufacturerContext>();
			foreach (int manufacturerId in manufacturerIds)
			{
				contexts.Add(new CatalogManufacturerContext(AllCatalogsContext.Instance, new ManufacturerContext(manufacturerId)));
			}

			return new ClauseContext(contexts);
        }

        private bool isNegationOperator(Operator @operator)
		{
			return OperatorClasses.NegativeEqualityOperators.Contains(@operator);
		}

		private bool handlesOperator(Operator @operator)
		{
			return OperatorClasses.EqualityOperatorsWithEmpty.Contains(@operator);
		}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value">The query literal; must not be null or the empty literal.</param>
        /// <returns>A list of ids of issue types represented by this literal; never null.</returns>
		internal virtual IEnumerable<int> GetIds(QueryLiteral value)
        {
            if (value.StringValue != null)
			{
				//return resolver.getIndexedValues(value.StringValue);
                ManufacturerService.GetManufacturers(0).Select(m => m.Id);
			}
            if (value.IntValue != null)
            {
                //return resolver.getIndexedValues(value.IntValue);
                return ManufacturerService.GetManufacturers(0).Select(m => m.Id);
            }
            throw new Exception("Invalid query literal.");
        }
    }
}