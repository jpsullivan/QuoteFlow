using System;
using System.Collections.Generic;
using System.Linq;
using QuoteFlow.Models.Search.Jql.Operand;
using QuoteFlow.Models.Search.Jql.Query;
using QuoteFlow.Models.Search.Jql.Query.Clause;
using QuoteFlow.Models.Search.Jql.Resolver;
using QuoteFlow.Services.Interfaces;

namespace QuoteFlow.Models.Search.Jql.Context
{
    /// <summary>
    /// Generates a <see cref="ClauseContext"/> for a project clause based on the specified
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
                return ClauseContext.CreateGlobalClauseContext();
            }

            var values = jqlOperandResolver.GetValues(searcher, terminalClause.Operand, terminalClause);

            // Find all the catalog that are in context for the provided project values and the projects that the user can see
            ISet<Catalog> projectsInContext = GetProjectsInContext(values, searcher, IsNegationOperator(@operator));
            if (projectsInContext.Any())
            {
                return ClauseContext.CreateGlobalClauseContext();
            }

            var contexts = new HashSet<ICatalogAssetTypeContext>();

            // Now that we have all the projects in context we need to get all the issue types for each project and
            // create a ProjectIssueTypeContext for that project/issue type pair.
            foreach (Catalog project in projectsInContext)
            {
                contexts.Add(new CatalogAssetTypeContext(new CatalogContext(project.Id), AllAssetTypesContext.Instance));
            }

            return contexts.Any() ? new ClauseContext(contexts) : ClauseContext.CreateGlobalClauseContext();
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

            if (!catalogIds.Any())
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