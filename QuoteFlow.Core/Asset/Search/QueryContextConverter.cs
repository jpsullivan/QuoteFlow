using System.Collections.Generic;
using QuoteFlow.Api.Jql.Query;
using QuoteFlow.Core.Jql.Context;
using Wintellect.PowerCollections;

namespace QuoteFlow.Core.Asset.Search
{
    /// <summary>
    /// A utlility class for converting a <see cref="QueryContext"/> into a <seea cref="SearchContext"/>.
    /// This conversion only makes sense if the <see cref="IQuery"/> the QueryContext was generated from fits the simple navigator.
    /// </summary>
    public class QueryContextConverter
    {
        /// <summary>
        /// Converts a <see cref="SearchContext"/> representation into
        /// the <see cref="QueryContext"/> of a search context.
        /// 
        /// As search contexts represented by <see cref="QueryContext"/>s is a super set of those
        /// represented by <see cref="SearchContext"/>, this coversion will always be valid and
        /// never return null.
        /// </summary>
        /// <param name="searchContext">The context to convert into a <see cref="QueryContext"/> </param>
        /// <returns>The context represented by a <see cref="QueryContext"/>. Never Null. </returns>
        public virtual QueryContext getQueryContext(SearchContext searchContext)
        {
            Set<ProjectIssueTypeContext> contexts = new HashSet<ProjectIssueTypeContext>();
            if (searchContext.ForAnyProjects && searchContext.ForAnyIssueTypes)
            {
                return new QueryContextImpl(ClauseContextImpl.createGlobalClauseContext());
            }
            else if (searchContext.ForAnyProjects)
            {
                foreach (string typeId in searchContext.IssueTypeIds)
                {
                    contexts.add(new ProjectIssueTypeContextImpl(AllProjectsContext.INSTANCE, new IssueTypeContextImpl(typeId)));
                }
            }
            else if (searchContext.ForAnyIssueTypes)
            {
                foreach (long? projId in searchContext.ProjectIds)
                {
                    contexts.add(new ProjectIssueTypeContextImpl(new ProjectContextImpl(projId), AllIssueTypesContext.INSTANCE));
                }
            }
            else
            {
                foreach (long? projId in searchContext.ProjectIds)
                {
                    foreach (string typeId in searchContext.IssueTypeIds)
                    {
                        contexts.add(new ProjectIssueTypeContextImpl(new ProjectContextImpl(projId), new IssueTypeContextImpl(typeId)));
                    }
                }
            }
            return new QueryContextImpl(new ClauseContextImpl(contexts));
        }

    }
}
