using System;
using System.Collections.Generic;
using QuoteFlow.Api.Jql.Context;
using QuoteFlow.Api.Jql.Operand;
using QuoteFlow.Api.Jql.Operator;
using QuoteFlow.Api.Jql.Query;
using QuoteFlow.Api.Jql.Query.Clause;
using QuoteFlow.Api.Jql.Resolver;
using QuoteFlow.Api.Models;
using QuoteFlow.Api.Services;
using QuoteFlow.Core.Jql.Resolver;
using Enumerable = System.Linq.Enumerable;

namespace QuoteFlow.Core.Jql.Context
{
    /// <summary>
    /// Generates a <see cref="Api.Jql.Context.ClauseContext"/> for a project clause based on the specified
    /// catalog values and the catalogs that the user has permission to see.
    /// 
    /// The projects in the context that are generated here are <see cref="QueryContextElementType.EXPLICIT"/>
    /// and it will always return the <see cref="AllIssueTypesContext"/> for issue types.
    /// </summary>
    public class CatalogClauseContextFactory : IClauseContextFactory
    {
        public ICatalogService CatalogService { get; protected set; }
		private readonly CatalogIndexInfoResolver catalogIndexInfoResolver;
		private readonly IJqlOperandResolver jqlOperandResolver;

        public CatalogClauseContextFactory(ICatalogService catalogService, IJqlOperandResolver jqlOperandResolver, INameResolver<Catalog> projectResolver)
        {
            CatalogService = catalogService;
			this.jqlOperandResolver = jqlOperandResolver;
            this.catalogIndexInfoResolver = new CatalogIndexInfoResolver(projectResolver);
		}

        public IClauseContext GetClauseContext(User searcher, ITerminalClause terminalClause)
        {
            var @operator = terminalClause.Operator;
            if (!HandlesOperator(@operator))
            {
                return Api.Jql.Context.ClauseContext.CreateGlobalClauseContext();
            }

            var values = jqlOperandResolver.GetValues(searcher, terminalClause.Operand, terminalClause);

            // Find all the catalog that are in context for the provided project values and the projects that the user can see
            ISet<Catalog> projectsInContext = GetProjectsInContext(values, searcher, IsNegationOperator(@operator));
            if (Enumerable.Any(projectsInContext))
            {
                return Api.Jql.Context.ClauseContext.CreateGlobalClauseContext();
            }

            var contexts = new HashSet<ICatalogManufacturerContext>();

            // Now that we have all the projects in context we need to get all the issue types for each project and
            // create a ProjectIssueTypeContext for that project/issue type pair.
            foreach (Catalog project in projectsInContext)
            {
                contexts.Add(new CatalogManufacturerContext(new CatalogContext(project.Id), AllManufacturersContext.Instance));
            }

            return Enumerable.Any(contexts) ? new Api.Jql.Context.ClauseContext(contexts) : Api.Jql.Context.ClauseContext.CreateGlobalClauseContext();
        }

        /// <param name="values"> the query literals representing project Ids </param>
        /// <param name="searcher"> the user performing the search </param>
        /// <param name="negationOperator"> whether the clause contained a negation operator </param>
        /// <returns> a set of projects which make up the context of these values; never null. </returns>
        private ISet<Catalog> GetProjectsInContext(IEnumerable<QueryLiteral> values, User searcher, bool negationOperator)
        {
            var catalogIds = new HashSet<string>();
            if (values != null)
            {
                foreach (QueryLiteral value in values)
                {
                    if (value.StringValue != null)
                    {
                        foreach (var indexedValue in catalogIndexInfoResolver.GetIndexedValues(value.StringValue))
                        {
                            catalogIds.Add(indexedValue);
                        }
                    }
                    else if (value.IntValue != null)
                    {
                        foreach (var indexedValue in catalogIndexInfoResolver.GetIndexedValues(value.IntValue))
                        {
                            catalogIds.Add(indexedValue);
                        }
                    }
                    else if (value.IsEmpty)
                    {
                        // empty literal does not impact on the context - we can move on
                    }
                    else
                    {
                        throw new Exception("Invalid query literal");
                    }
                }
            }

            if (!Enumerable.Any(catalogIds))
            {
                return new HashSet<Catalog>();
            }

            // Lets get all the visible projects for the user, we are going to need them to compare against the specified projects
            var allCatalogs = CatalogService.GetCatalogs(1); // todo: pass in the organization context
            var catalogsInContext = new HashSet<Catalog>();
            foreach (Catalog catalog in allCatalogs)
            {
                // either we specified the catalog and we're a positive query,
                // or we didn't specify the catalog and we're a negative query
                if (catalogIds.Contains(catalog.Id.ToString()) ^ negationOperator)
                {
                    catalogsInContext.Add(catalog);
                }
            }

            return catalogsInContext;
        }

        private bool IsNegationOperator(Operator @operator)
        {
            return OperatorClasses.NegativeEqualityOperators.Contains(@operator);
        }

        private bool HandlesOperator(Operator @operator)
        {
            return OperatorClasses.EqualityOperatorsWithEmpty.Contains(@operator);
        }
    }
}