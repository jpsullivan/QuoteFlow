﻿using Ninject;
using QuoteFlow.Api.Jql.Query;
using QuoteFlow.Api.Jql.Query.Clause;
using QuoteFlow.Api.Jql.Query.Order;
using QuoteFlow.Core.DependencyResolution;

namespace QuoteFlow.Core.Jql.Builder
{
    /// <summary>
    /// Used to build <see cref="IQuery"/>'s that can be used to perform issue searching in JIRA.
    /// <p/>
    /// This gives you access to a <see cref="IJqlClauseBuilder"/> which can be used to build up the
    /// where clause of the JQL and also a <see cref="JqlOrderByBuilder"/> which can be used to build
    /// up the order by clause of the JQL.
    /// <p/>
    /// This object can also be used as a factory for <see cref="IJqlClauseBuilder"/> and
    /// <see cref="JqlOrderByBuilder"/> instances.
    /// </summary>
    public class JqlQueryBuilder
    {
        private readonly JqlOrderByBuilder _jqlOrderByBuilder;
        private readonly IJqlClauseBuilder _jqlClauseBuilder;

        /// <summary>
        /// 
        /// </summary>
        /// <returns>A new builder that can be used to build a JQL query.</returns>
        public static JqlQueryBuilder NewBuilder()
        {
            return new JqlQueryBuilder();
        }

        /// <summary>
        /// Creates a new builder that clones the state of the passed in query so that you can use the resulting builder to
        /// create a slightly modified query.
        /// </summary>
        /// <param name="existingQuery">The template to clone, both the where clause and order by clause will be cloned.</param>
        /// <returns>A new builder that clones the state of the passed in query.</returns>
        public static JqlQueryBuilder NewBuilder(IQuery existingQuery)
        {
            return new JqlQueryBuilder(existingQuery);
        }

        /// <summary>
        /// Build a new <see cref="JqlClauseBuilder"/>. The returned builder will have no associated
        /// <see cref="JqlQueryBuilder"/>.
        /// </summary>
        /// <returns>The new clause builder.</returns>
        public static IJqlClauseBuilder NewClauseBuilder()
        {
            return CreateClauseBuilder(null, null);
        }

        /// <summary>
        /// Build a new <see cref="IJqlClauseBuilder"/> and initialise it with the passed clause.
        /// The returned builder will have no associated <see cref="JqlQueryBuilder"/>.
        /// </summary>
        /// <param name="copy"> the claue to Add to the new builder. Can be null. </param>
        /// <returns> the new clause builder. </returns>
        public static IJqlClauseBuilder NewClauseBuilder(IClause copy)
        {
            return CreateClauseBuilder(null, copy);
        }

        /// <summary>
        /// Build a new <see cref="IJqlClauseBuilder"/> and initialise it with the clause from the
        /// passed query. The returned builder will have no associated <see cref="JqlQueryBuilder"/>.
        /// </summary>
        /// <param name="query">The query whose where clause will be copied into the new builder. Can be null.</param>
        /// <returns>The new clause builder.</returns>
        public static IJqlClauseBuilder NewClauseBuilder(IQuery query)
        {
            return CreateClauseBuilder(null, query?.WhereClause);
        }

        /// <summary>
        /// Build a new <see cref="JqlOrderByBuilder"/>. The returned builder will have no associated
        /// <see cref="JqlQueryBuilder"/>.
        /// </summary>
        /// <returns> the new clause builder. </returns>
        public static JqlOrderByBuilder NewOrderByBuilder()
        {
            return new JqlOrderByBuilder(null);
        }

        /// <summary>
        /// Build a new <see cref="JqlOrderByBuilder"/> and initialise it with the passed order. The returned builder will have
        /// no associated <see cref="JqlQueryBuilder"/>.
        /// </summary>
        /// <param name="copy"> the order to copy. Can be null. </param>
        /// <returns> the new clause builder. </returns>
        public static JqlOrderByBuilder NewOrderByBuilder(IOrderBy copy)
        {
            return CreateOrderByBuilder(null, copy);
        }

        /// <summary>
        /// Build a new <see cref="JqlOrderByBuilder"/> and initialise it with the order from the passed query. The returned builder will have
        /// no associated <see cref="JqlQueryBuilder"/>.
        /// </summary>
        /// <param name="query"> the query whose order will be copied into the new builder. Can be null. </param>
        /// <returns> the new clause builder. </returns>
        public static JqlOrderByBuilder NewOrderByBuilder(IQuery query)
        {
            return CreateOrderByBuilder(null, query?.OrderByClause);
        }

        private JqlQueryBuilder()
        {
            _jqlOrderByBuilder = CreateOrderByBuilder(this, null);
            _jqlClauseBuilder = CreateClauseBuilder(this, null);
        }

        private JqlQueryBuilder(IQuery existingQuery)
        {
            IClause exisitingClause = null;
            IOrderBy exisitngOrderBy = null;
            
            if (existingQuery != null)
            {
                exisitingClause = existingQuery.WhereClause;
                exisitngOrderBy = existingQuery.OrderByClause;
            }

            _jqlClauseBuilder = CreateClauseBuilder(this, exisitingClause);
            _jqlOrderByBuilder = CreateOrderByBuilder(this, exisitngOrderBy);
        }

        /// <summary>
        /// Creates an <see cref="JqlOrderByBuilder"/> that can be used to modify the order by
        /// statements of the <see cref="JqlQueryBuilder"/> instance.
        /// </summary>
        /// <returns>An OrderBy builder associated with the <see cref="JqlQueryBuilder"/> instance.</returns>
        public virtual JqlOrderByBuilder OrderBy()
        {
            return _jqlOrderByBuilder;
        }

        /// <summary>
        /// Creates an <see cref="IJqlClauseBuilder"/> which is used to modify the where clause portion
        /// of the <see cref="JqlQueryBuilder"/> instance.
        /// </summary>
        /// <returns>A WhereClause builder associated with the <see cref="JqlQueryBuilder"/> instance.</returns>
        public virtual IJqlClauseBuilder Where()
        {
            return _jqlClauseBuilder;
        }

        /// <summary>
        /// This will find the root of the clause tree and build a <see cref="IQuery"/> whos where clause will
        /// return the generated clause and Order By clause will return the generated search order.
        /// <p/>
        /// NOTE: Calling this method does not change the state of the builder, there are no limitations on the number of
        /// times this method can be invoked.
        /// </summary>
        /// <returns>A Query whos where clause contains the built clauses and search order contains the built OrderBy clauses.</returns>
        public virtual IQuery BuildQuery()
        {
            // Create the query from our configured data
            IClause whereClause = _jqlClauseBuilder.BuildClause();
            IOrderBy orderByClause = _jqlOrderByBuilder.BuildOrderBy();
            return new Api.Jql.Query.Query(whereClause, orderByClause, null);
        }

        /// <summary>
        /// Reset the builder to its empty state.
        /// </summary>
        /// <returns> this builder with its state cleared. </returns>
        public virtual JqlQueryBuilder Clear()
        {
            Where().Clear();
            OrderBy().Clear();

            return this;
        }

        private static IJqlClauseBuilder CreateClauseBuilder(JqlQueryBuilder parent, IClause copy)
        {

            var jqlClauseBuilder = Container.Kernel.TryGet<IJqlClauseBuilderFactory>().NewJqlClauseBuilder(parent);
            if (copy != null)
            {
                jqlClauseBuilder.AddClause(copy);
            }
            return jqlClauseBuilder;
        }

        private static JqlOrderByBuilder CreateOrderByBuilder(JqlQueryBuilder parent, IOrderBy copy)
        {
            var builder = new JqlOrderByBuilder(parent);
            if (copy != null)
            {
                builder.SetSorts(copy);
            }
            return builder;
        }
    }

}
