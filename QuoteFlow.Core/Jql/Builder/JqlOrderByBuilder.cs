using System;
using System.Collections.Generic;
using System.Linq;
using Ninject;
using QuoteFlow.Api.Asset.Search;
using QuoteFlow.Api.Asset.Search.Constants;
using QuoteFlow.Api.Asset.Search.Managers;
using QuoteFlow.Api.Infrastructure.Extensions;
using QuoteFlow.Api.Infrastructure.IoC;
using QuoteFlow.Api.Jql.Query;
using QuoteFlow.Api.Jql.Query.Order;
using QuoteFlow.Core.Asset.Search.Managers;
using QuoteFlow.Core.DependencyResolution;

namespace QuoteFlow.Core.Jql.Builder
{
    /// <summary>
    /// Used to create <seealso cref="OrderBy"/> clauses to be included in {@link
    /// com.atlassian.query.Query}'s.
    /// 
    /// The OrderBy portion of a JQL query is made up of zero of more order clauses. Each clause composes of a field
    /// and either a <see cref="SortOrder.ASC"/> or a <see cref="SortOrder.DESC"/>
    /// sort order.
    /// 
    /// The order of sorting is from the first search clause in the list to the last. For example
    /// {@code builder.status(SortOrder.DESC).component(SortOrder.ASC).BuildOrderBy()} will produce the Order By statement
    /// {@code Order By status DESC component ASC} which will first sort the result by status descending, and then by component
    /// ascending.
    /// </summary>
    public class JqlOrderByBuilder
    {
        private List<SearchSort> _searchSorts;
        private readonly JqlQueryBuilder _parentBuilder;

        public JqlOrderByBuilder(JqlQueryBuilder parentBuilder)
        {
            _parentBuilder = parentBuilder;
            _searchSorts = new List<SearchSort>();
        }

        /// <summary>
        /// Override any sorts that may have been setup in the builder with the provided list of sorts.
        /// </summary>
        /// <param name="newSorts"> the new sorts to include in the builder, must not be null. </param>
        /// <returns>This builder.</returns>
        public virtual JqlOrderByBuilder SetSorts(ICollection<SearchSort> newSorts)
        {
            if (newSorts == null)
            {
                throw new ArgumentNullException("newSorts");
            }

            _searchSorts = new List<SearchSort>(newSorts);
            return this;
        }

        /// <summary>
        /// Creates a builder who's state will be a mutable copy of the passed in order by.
        /// </summary>
        /// <param name="existingOrderBy"> the template which defines the state the builder will be in once this method returns. </param>
        /// <returns>A builder who's state will be a mutable copy of the passed in order by.</returns>
        public virtual JqlOrderByBuilder SetSorts(IOrderBy existingOrderBy)
        {
            if (existingOrderBy != null)
            {
                _searchSorts = existingOrderBy.SearchSorts;
            }

            return this;
        }

        /// 
        /// <summary>
        /// Call this method to build a <seealso cref="QuoteFlow.Api.Jql.Query"/> using the current builder. When <seealso cref="#EndOrderBy()"/> is not null, this
        /// equates to calling {@code EndOrderBy().BuildQuery()}. When {@code EndOrderBy()} is null, this equates to calling
        /// {@code new QueryImpl(null, BuildOrderBy(), null)}.
        /// </summary>
        /// <returns>The newly generated query.</returns>
        public virtual IQuery BuildQuery()
        {
            if (_parentBuilder != null)
            {
                // Create the query from our configured data
                return _parentBuilder.BuildQuery();
            }
            return new Api.Jql.Query.Query(null, BuildOrderBy(), null);
        }

        /// <summary>
        /// NOTE: Calling this method does not change the state of the builder, 
        /// there are no limitations on the number of times this method can be invoked. 
        /// </summary>
        /// <returns>the <seealso cref="OrderBy"/> that is defined by the state of the builder.</returns>
        public virtual IOrderBy BuildOrderBy()
        {
            return new OrderBy(_searchSorts.ToList());
        }

        /// <summary>
        /// Call this to return to the parent JqlQueryBuilder.
        /// </summary>
        /// <returns> the query builder who created this order by builder. May be null if there is no associated <seealso cref="JqlQueryBuilder"/>. </returns>
        public virtual JqlQueryBuilder EndOrderBy()
        {
            return _parentBuilder;
        }

        /// <summary>
        /// Reset the builder to its empty state.
        /// </summary>
        /// <returns>The builder in its empty state.</returns>
        public virtual JqlOrderByBuilder Clear()
        {
            _searchSorts = new List<SearchSort>();
            return this;
        }

        /// <summary>
        /// Add a search sort with the fieldName and the specified sort to the order by. This is a convienience method that
        /// trys to lookup the primary JQL clause name for the provided field name.
        /// 
        /// If we are unable to find the associated clause name then the sort will be added with the provided field name and
        /// will likely not pass JQL validation.
        /// </summary>
        /// <param name="fieldName">The field name used to lookup the JQL clause name via {@link SearchHandlerManager#getJqlClauseNames(String)} method.</param>
        /// <param name="order">The order, ASC, or DESC.</param>
        /// <param name="makePrimarySort">If true this will be added to the beginning of the sorts, otherwise it will be added to the end.</param>
        /// <returns>The current builder.</returns>
        public virtual JqlOrderByBuilder AddSortForFieldName(string fieldName, SortOrder order, bool makePrimarySort)
        {
            if (fieldName == null)
            {
                throw new ArgumentNullException("fieldName");
            }

            ISearchHandlerManager searchHandlerManager = Container.Kernel.TryGet<SearchHandlerManager>();
            ICollection<ClauseNames> clauseNames = searchHandlerManager.GetJqlClauseNames(fieldName);
            string sortName = fieldName;
            if (clauseNames.Count > 0)
            {
                var nameEnumerator = clauseNames.GetEnumerator();
                if (nameEnumerator.MoveNext())
                {
                    sortName = nameEnumerator.Current.PrimaryName;
                }
            }
            else
            {
                //log.debug("Unable to find a JQL clause name for field name '" + fieldName + "', adding sort anyway.");
            }

            return Add(sortName, order, makePrimarySort);
        }

        /// <summary>
        /// Add a search sort with the jqlClauseName and the specified sort to the order by. No validation is done in this
        /// builder so you must make sure you create valid sorts.
        /// </summary>
        /// <param name="jqlClauseName">The JQL clause name to sort by.</param>
        /// <param name="order">The order, ASC, or DESC.</param>
        /// <param name="makePrimarySort">If true this will be added to the beginning of the sorts, otherwise it will be added to the end.</param>
        /// <returns>The current builder.</returns>
        public virtual JqlOrderByBuilder Add(string jqlClauseName, SortOrder order, bool makePrimarySort)
        {
            if (jqlClauseName.IsNullOrWhiteSpace())
            {
                throw new ArgumentNullException("jqlClauseName");
            }

            if (makePrimarySort)
            {
                _searchSorts.Insert(0, new SearchSort(jqlClauseName, order));
            }
            else
            {
                _searchSorts.Add(new SearchSort(jqlClauseName, order));
            }
            return this;
        }

        /// <summary>
        /// Add a search sort with the jqlClauseName and the specified sort to the end of the sort list in the order by. No
        /// validation is done in this builder so you must make sure you create valid sorts.
        /// <p/>
        /// This is the same as calling <seealso cref="#Add(String, com.atlassian.query.order.SortOrder, boolean)"/> with false.
        /// </summary>
        /// <param name="jqlClauseName"> the JQL clause name to sort by. </param>
        /// <param name="order"> the order, ASC, or DESC. </param>
        /// <returns> the current builder. </returns>
        public virtual JqlOrderByBuilder Add(string jqlClauseName, SortOrder order)
        {
            return Add(jqlClauseName, order, false);
        }

        /// <summary>
        /// Add a search sort with the jqlClauseName and use the claues default sort to the end of the sort list in the order
        /// by. No validation is done in this builder so you must make sure you create valid sorts.
        /// <p/>
        /// This is the same as calling <seealso cref="#Add(String, com.atlassian.query.order.SortOrder, boolean)"/> with null and
        /// false.
        /// </summary>
        /// <param name="jqlClauseName"> the JQL clause name to sort by. </param>
        /// <returns> the current builder. </returns>
        public virtual JqlOrderByBuilder Add(string jqlClauseName)
        {
            return Add(jqlClauseName, SortOrder.None, false);
        }

//        public virtual JqlOrderByBuilder priority(SortOrder order, bool makePrimarySort)
//        {
//            return Add(SystemSearchConstants.ForPriority().JqlClauseNames.PrimaryName, order, makePrimarySort);
//        }
//
//        public virtual JqlOrderByBuilder priority(SortOrder order)
//        {
//            return priority(order, false);
//        }
//
//        public virtual JqlOrderByBuilder currentEstimate(SortOrder order)
//        {
//            return currentEstimate(order, false);
//        }
//
//        public virtual JqlOrderByBuilder currentEstimate(SortOrder order, bool makePrimarySort)
//        {
//            return Add(SystemSearchConstants.forCurrentEstimate().JqlClauseNames.PrimaryName, order, makePrimarySort);
//        }
//
//        public virtual JqlOrderByBuilder originalEstimate(SortOrder order)
//        {
//            return originalEstimate(order, false);
//        }
//
//        public virtual JqlOrderByBuilder originalEstimate(SortOrder order, bool makePrimarySort)
//        {
//            return Add(SystemSearchConstants.forOriginalEstimate().JqlClauseNames.PrimaryName, order, makePrimarySort);
//        }
//
//        public virtual JqlOrderByBuilder votes(SortOrder order)
//        {
//            return votes(order, false);
//        }
//
//        public virtual JqlOrderByBuilder votes(SortOrder order, bool makePrimarySort)
//        {
//            return Add(SystemSearchConstants.forVotes().JqlClauseNames.PrimaryName, order, makePrimarySort);
//        }

//        public virtual JqlOrderByBuilder watches(SortOrder order)
//        {
//            return watches(order, false);
//        }
//
//        public virtual JqlOrderByBuilder watches(SortOrder order, bool makePrimarySort)
//        {
//            return Add(SystemSearchConstants.forWatches().JqlClauseNames.PrimaryName, order, makePrimarySort);
//        }

        public virtual JqlOrderByBuilder AssetId(SortOrder order)
        {
            return AssetId(order, false);
        }

        public virtual JqlOrderByBuilder AssetId(SortOrder order, bool makePrimarySort)
        {
            return Add(SystemSearchConstants.ForAssetId().JqlClauseNames.PrimaryName, order, makePrimarySort);
        }

        public virtual JqlOrderByBuilder Catalog(SortOrder order)
        {
            return Catalog(order, false);
        }

        public virtual JqlOrderByBuilder Catalog(SortOrder order, bool makePrimarySort)
        {
            return Add(SystemSearchConstants.ForCatalog().JqlClauseNames.PrimaryName, order, makePrimarySort);
        }

        public virtual JqlOrderByBuilder Manufacturer(SortOrder order)
        {
            return Manufacturer(order, false);
        }

        public virtual JqlOrderByBuilder Manufacturer(SortOrder order, bool makePrimarySort)
        {
            return Add(SystemSearchConstants.ForManufacturer().JqlClauseNames.PrimaryName, order, makePrimarySort);
        }

        public virtual JqlOrderByBuilder CreatedDate(SortOrder order)
        {
            return CreatedDate(order, false);
        }

        public virtual JqlOrderByBuilder CreatedDate(SortOrder order, bool makePrimarySort)
        {
            return Add(SystemSearchConstants.ForCreatedDate().JqlClauseNames.PrimaryName, order, makePrimarySort);
        }

//        public virtual JqlOrderByBuilder dueDate(SortOrder order)
//        {
//            return dueDate(order, false);
//        }
//
//        public virtual JqlOrderByBuilder dueDate(SortOrder order, bool makePrimarySort)
//        {
//            return Add(SystemSearchConstants.forDueDate().JqlClauseNames.PrimaryName, order, makePrimarySort);
//        }

//        public virtual JqlOrderByBuilder lastViewedDate(SortOrder order)
//        {
//            return lastViewedDate(order, false);
//        }
//
//        public virtual JqlOrderByBuilder lastViewedDate(SortOrder order, bool makePrimarySort)
//        {
//            return Add(SystemSearchConstants.forLastViewedDate().JqlClauseNames.PrimaryName, order, makePrimarySort);
//        }

        public virtual JqlOrderByBuilder UpdatedDate(SortOrder order)
        {
            return UpdatedDate(order, false);
        }

        public virtual JqlOrderByBuilder UpdatedDate(SortOrder order, bool makePrimarySort)
        {
            return Add(SystemSearchConstants.ForUpdatedDate().JqlClauseNames.PrimaryName, order, makePrimarySort);
        }

        public virtual JqlOrderByBuilder Summary(SortOrder order)
        {
            return Summary(order, false);
        }

        public virtual JqlOrderByBuilder Summary(SortOrder order, bool makePrimarySort)
        {
            return Add(SystemSearchConstants.ForSummary().JqlClauseNames.PrimaryName, order, makePrimarySort);
        }

//        public virtual JqlOrderByBuilder resolution(SortOrder order)
//        {
//            return resolution(order, false);
//        }
//
//        public virtual JqlOrderByBuilder resolution(SortOrder order, bool makePrimarySort)
//        {
//            return Add(SystemSearchConstants.forResolution().JqlClauseNames.PrimaryName, order, makePrimarySort);
//        }

//        public virtual JqlOrderByBuilder status(SortOrder order)
//        {
//            return status(order, false);
//        }
//
//        public virtual JqlOrderByBuilder status(SortOrder order, bool makePrimarySort)
//        {
//            return Add(SystemSearchConstants.forStatus().JqlClauseNames.PrimaryName, order, makePrimarySort);
//        }
//
//        public virtual JqlOrderByBuilder component(SortOrder order)
//        {
//            return component(order, false);
//        }
//
//        public virtual JqlOrderByBuilder component(SortOrder order, bool makePrimarySort)
//        {
//            return Add(SystemSearchConstants.forComponent().JqlClauseNames.PrimaryName, order, makePrimarySort);
//        }
//
//        public virtual JqlOrderByBuilder affectedVersion(SortOrder order)
//        {
//            return affectedVersion(order, false);
//        }
//
//        public virtual JqlOrderByBuilder affectedVersion(SortOrder order, bool makePrimarySort)
//        {
//            return Add(SystemSearchConstants.forAffectedVersion().JqlClauseNames.PrimaryName, order, makePrimarySort);
//        }
//
//        public virtual JqlOrderByBuilder fixForVersion(SortOrder order)
//        {
//            return fixForVersion(order, false);
//        }
//
//        public virtual JqlOrderByBuilder fixForVersion(SortOrder order, bool makePrimarySort)
//        {
//            return Add(SystemSearchConstants.forFixForVersion().JqlClauseNames.PrimaryName, order, makePrimarySort);
//        }

        public virtual JqlOrderByBuilder Description(SortOrder order)
        {
            return Description(order, false);
        }

        public virtual JqlOrderByBuilder Description(SortOrder order, bool makePrimarySort)
        {
            return Add(SystemSearchConstants.ForDescription().JqlClauseNames.PrimaryName, order, makePrimarySort);
        }

//        public virtual JqlOrderByBuilder environment(SortOrder order)
//        {
//            return environment(order, false);
//        }
//
//        public virtual JqlOrderByBuilder environment(SortOrder order, bool makePrimarySort)
//        {
//            return Add(SystemSearchConstants.forEnvironment().JqlClauseNames.PrimaryName, order, makePrimarySort);
//        }
//
//        public virtual JqlOrderByBuilder resolutionDate(SortOrder order)
//        {
//            return resolutionDate(order, false);
//        }
//
//        public virtual JqlOrderByBuilder resolutionDate(SortOrder order, bool makePrimarySort)
//        {
//            return Add(SystemSearchConstants.forResolutionDate().JqlClauseNames.PrimaryName, order, makePrimarySort);
//        }
//
//        public virtual JqlOrderByBuilder reporter(SortOrder order)
//        {
//            return reporter(order, false);
//        }
//
//        public virtual JqlOrderByBuilder reporter(SortOrder order, bool makePrimarySort)
//        {
//            return Add(SystemSearchConstants.forReporter().JqlClauseNames.PrimaryName, order, makePrimarySort);
//        }
//
//        public virtual JqlOrderByBuilder assignee(SortOrder order)
//        {
//            return assignee(order, false);
//        }
//
//        public virtual JqlOrderByBuilder assignee(SortOrder order, bool makePrimarySort)
//        {
//            return Add(SystemSearchConstants.forAssignee().JqlClauseNames.PrimaryName, order, makePrimarySort);
//        }
//
//        public virtual JqlOrderByBuilder workRatio(SortOrder order)
//        {
//            return workRatio(order, false);
//        }
//
//        public virtual JqlOrderByBuilder workRatio(SortOrder order, bool makePrimarySort)
//        {
//            return Add(SystemSearchConstants.forWorkRatio().JqlClauseNames.PrimaryName, order, makePrimarySort);
//        }
//
//        public virtual JqlOrderByBuilder timeSpent(SortOrder order)
//        {
//            return timeSpent(order, false);
//        }
//
//        public virtual JqlOrderByBuilder timeSpent(SortOrder order, bool makePrimarySort)
//        {
//            return Add(SystemSearchConstants.forTimeSpent().JqlClauseNames.PrimaryName, order, makePrimarySort);
//        }
//
//        public virtual JqlOrderByBuilder securityLevel(SortOrder order)
//        {
//            return securityLevel(order, false);
//        }
//
//        public virtual JqlOrderByBuilder securityLevel(SortOrder order, bool makePrimarySort)
//        {
//            return Add(SystemSearchConstants.forSecurityLevel().JqlClauseNames.PrimaryName, order, makePrimarySort);
//        }
    }

}
