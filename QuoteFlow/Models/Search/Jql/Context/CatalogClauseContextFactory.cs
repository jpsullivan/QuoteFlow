using System.Collections.Generic;
using System.Linq;
using QuoteFlow.Models.Search.Jql.Operand;
using QuoteFlow.Models.Search.Jql.Query.Clause;
using QuoteFlow.Models.Search.Jql.Resolver;

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
		private readonly CatalogIndexInfoResolver catalogIndexInfoResolver;
		private readonly IJqlOperandResolver jqlOperandResolver;

        public CatalogClauseContextFactory(IJqlOperandResolver jqlOperandResolver, INameResolver<Catalog> projectResolver)
		{
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
            var projectIds = new HashSet<string>();
            if (values != null)
            {
                foreach (QueryLiteral value in values)
                {
                    if (value.StringValue != null)
                    {
                        projectIds.addAll(projectIndexInfoResolver.getIndexedValues(value.StringValue));
                    }
                    else if (value.LongValue != null)
                    {
                        projectIds.addAll(projectIndexInfoResolver.getIndexedValues(value.LongValue));
                    }
                    else if (value.Empty)
                    {
                        // empty literal does not impact on the context - we can move on
                    }
                    else
                    {
                        throw new IllegalStateException("Invalid query literal");
                    }
                }
            }

            if (projectIds.Empty)
            {
                return Collections.emptySet();
            }

            // Lets get all the visible projects for the user, we are going to need them to compare against the specified projects
            var visibleProjects = permissionManager.getProjectObjects(Permissions.BROWSE, searcher);
            var projectsInContext = new HashSet<Catalog>();
            foreach (Catalog visibleProject in visibleProjects)
            {
                // either we specified the project and we're a positive query,
                // or we didn't specify the project and we're a negative query
                if (projectIds.contains(visibleProject.Id.ToString()) ^ negationOperator)
                {
                    projectsInContext.add(visibleProject);
                }
            }

            return projectsInContext;
        }
    }
}