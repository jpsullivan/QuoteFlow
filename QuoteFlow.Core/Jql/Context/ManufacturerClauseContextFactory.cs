using System;
using System.Collections.Generic;
using System.Linq;
using Ninject;
using QuoteFlow.Api.Jql.Context;
using QuoteFlow.Api.Jql.Operand;
using QuoteFlow.Api.Jql.Operator;
using QuoteFlow.Api.Jql.Query;
using QuoteFlow.Api.Jql.Query.Clause;
using QuoteFlow.Api.Models;
using QuoteFlow.Api.Services;
using QuoteFlow.Core.DependencyResolution;
using QuoteFlow.Core.Jql.Resolver;
using QuoteFlow.Core.Services;

namespace QuoteFlow.Core.Jql.Context
{
    /// <summary>
    /// Generates a <see cref="Api.Jql.Context.ClauseContext"/> based on the manufacturer values and 
    /// the catalog they are visible in.
    /// </summary>
    public class ManufacturerClauseContextFactory : IClauseContextFactory
    {
        public IJqlOperandResolver JqlOperandResolver { get; protected set; }
        public ManufacturerResolver ManufacturerResolver { get; protected set; }
        public IManufacturerService ManufacturerService { get; protected set; }

        // todo: Use the resolver to fetch *indexed* values instead of calling the service each time

        public ManufacturerClauseContextFactory(ManufacturerResolver resolver, IJqlOperandResolver jqlOperandResolver)
        {
            ManufacturerResolver = resolver;
            JqlOperandResolver = jqlOperandResolver;
            ManufacturerService = Container.Kernel.TryGet<ManufacturerService>();
        }

        public IClauseContext GetClauseContext(User searcher, ITerminalClause terminalClause)
        {
            Operator @operator = terminalClause.Operator;
			if (!handlesOperator(@operator))
			{
				return Api.Jql.Context.ClauseContext.CreateGlobalClauseContext();
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
			    var ids = ManufacturerService.GetManufacturers(1).Select(m => m.Id); // todo: replace organization id
			    ISet<int> allManufacturerIds = new HashSet<int>(ids);
			    foreach (var id in manufacturerIds)
			    {
			        allManufacturerIds.Remove(id);
			    }
				manufacturerIds = allManufacturerIds;
			}

			if (!manufacturerIds.Any())
			{
				return Api.Jql.Context.ClauseContext.CreateGlobalClauseContext();
			}

			ISet<ICatalogManufacturerContext> contexts = new HashSet<ICatalogManufacturerContext>();
			foreach (int manufacturerId in manufacturerIds)
			{
				contexts.Add(new CatalogManufacturerContext(AllCatalogsContext.Instance, new ManufacturerContext(manufacturerId)));
			}

			return new Api.Jql.Context.ClauseContext(contexts);
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
                ManufacturerService.GetManufacturers(1).Select(m => m.Id); // todo: replace organization id
			}
            if (value.IntValue != null)
            {
                //return resolver.getIndexedValues(value.IntValue);
                return ManufacturerService.GetManufacturers(1).Select(m => m.Id); // todo: replace organization id
            }
            throw new Exception("Invalid query literal.");
        }
    }
}