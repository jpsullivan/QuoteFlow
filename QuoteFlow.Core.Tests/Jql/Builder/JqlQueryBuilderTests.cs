using System;
using QuoteFlow.Api.Jql.Query;
using QuoteFlow.Api.Jql.Query.Clause;
using QuoteFlow.Api.Jql.Query.Order;
using QuoteFlow.Core.Jql.Builder;
using Xunit;

namespace QuoteFlow.Core.Tests.Jql.Builder
{
    public class JqlQueryBuilderTests
    {
        public class TheClearMethod
        {
            [Fact]
            public void TestJqlClearBuilder()
            {
                JqlQueryBuilder builder = JqlQueryBuilder.NewBuilder();
                builder.Where().DescriptionIsEmpty();
                builder.OrderBy().Catalog(SortOrder.None);

                Assert.NotNull(builder.Where().BuildClause());
                Assert.False(OrderBy.NoOrder.Equals(builder.OrderBy().BuildOrderBy()));

                builder.Clear();

                Assert.Null(builder.Where().BuildClause());
                Assert.Equal(OrderBy.NoOrder, builder.OrderBy().BuildOrderBy());
            }
        }

        [Fact]
        public void TestSubBuildersNotNull()
        {
            JqlQueryBuilder builder = JqlQueryBuilder.NewBuilder();
            Assert.NotNull(builder.Where());
            Assert.NotNull(builder.OrderBy());
            Assert.NotNull(builder.BuildQuery());
            Assert.Equal(new Api.Jql.Query.Query(null, new OrderBy(), null), builder.BuildQuery());
        }

        [Fact]
        public void TestCloneFromExisting()
        {
            var existingOrderBy = new OrderBy(new SearchSort("Test", SortOrder.DESC), new SearchSort("Blah", SortOrder.ASC), new SearchSort("Heee"), new SearchSort("ASC", "Haaaa"));
            var existingClause = JqlQueryBuilder.NewBuilder().Where().Summary("test!").And().Sub().Description("desc").Or().Not().DescriptionIsEmpty().Endsub().BuildClause();
            var existingSearchQuery = new Api.Jql.Query.Query(existingClause, existingOrderBy, null);
            Assert.Equal(existingSearchQuery, JqlQueryBuilder.NewBuilder(existingSearchQuery).BuildQuery());
        }

        [Fact]
        public void TestCloneFromExistingNull()
        {
            var existingSearchQuery = new Api.Jql.Query.Query(null, new OrderBy(), null);

            Assert.Equal(existingSearchQuery, JqlQueryBuilder.NewBuilder(null).BuildQuery());
            Assert.Equal(existingSearchQuery, JqlQueryBuilder.NewBuilder(null).Where().And().BuildQuery());
            Assert.Equal(existingSearchQuery, JqlQueryBuilder.NewBuilder(null).Where().Or().BuildQuery());
        }

        [Fact]
        public void TestCloneFromExistingEmpty()
        {
            var existingSearchQuery = new Api.Jql.Query.Query(null, new OrderBy(), null);

            Assert.Equal(existingSearchQuery, JqlQueryBuilder.NewBuilder(existingSearchQuery).BuildQuery());
            Assert.Equal(existingSearchQuery, JqlQueryBuilder.NewBuilder(existingSearchQuery).Where().And().BuildQuery());
            Assert.Equal(existingSearchQuery, JqlQueryBuilder.NewBuilder(existingSearchQuery).Where().Or().BuildQuery());

            // it should be possible to call And()/Or() on an empty query.
            JqlQueryBuilder builder = JqlQueryBuilder.NewBuilder(existingSearchQuery);
            builder.Where().And().Or().Description().Like().String("test");

            var expectedSearchQuery = new Api.Jql.Query.Query(new TerminalClause("description", Operator.LIKE, "test"), new OrderBy(), null);
            Assert.Equal(expectedSearchQuery, builder.BuildQuery());
        }

        [Fact]
        public void TestNewQueryBuilder()
        {
            var builder = JqlQueryBuilder.NewClauseBuilder();
            Assert.NotNull(builder);

            Assert.Same(builder, builder.And());
            Assert.Same(builder, builder.Or());

            // make sure the builder is empty initially.
            Assert.Null(builder.BuildClause());

            // make sure there is no parent.
            Assert.Null(builder.EndWhere());
        }

        [Fact]
        public void TestNewQueryBuilderClause()
        {
            var builder = JqlQueryBuilder.NewClauseBuilder((IClause) null);
            Assert.NotNull(builder);

            Assert.Same(builder, builder.And());
            Assert.Same(builder, builder.Or());

            // make sure the builder is empty initially.
            Assert.Null(builder.BuildClause());

            // make sure there is no parent.
            Assert.Null(builder.EndWhere());

            IClause clause = new TerminalClause("test", "cool");
            builder = JqlQueryBuilder.NewClauseBuilder(clause);

            Assert.Equal(clause, builder.BuildClause());
            Assert.Equal(new AndClause(clause, clause), builder.And().AddClause(clause).BuildClause());
            Assert.Null(builder.EndWhere());
        }

        [Fact]
        public void TestNewQueryBuilderQuery()
        {
            var builder = JqlQueryBuilder.NewClauseBuilder((IQuery) null);
            Assert.NotNull(builder);

            // make sure the builder is empty initially.
            Assert.Null(builder.BuildClause());

            // make sure there is no parent.
            Assert.Null(builder.EndWhere());

            var query = new Api.Jql.Query.Query(null);

            builder = JqlQueryBuilder.NewClauseBuilder(query);
            Assert.NotNull(builder);
            Assert.Null(builder.BuildClause());
            Assert.Null(builder.EndWhere());

            IClause clause = new TerminalClause("test", "cool");
            query = new Api.Jql.Query.Query(clause);

            builder = JqlQueryBuilder.NewClauseBuilder(query);
            Assert.NotNull(builder);
            Assert.Null(builder.EndWhere());

            Assert.Equal(clause, builder.BuildClause());
            Assert.Equal(new AndClause(clause, clause), builder.And().AddClause(clause).BuildClause());
            Assert.Null(builder.EndWhere());
        }

        [Fact]
        public void TestNewOrderByBuilder()
        {
            var builder = JqlQueryBuilder.NewOrderByBuilder();
            Assert.NotNull(builder);

            // make sure the builder is empty initially.
            Assert.Equal(OrderBy.NoOrder, builder.BuildOrderBy());

            // make sure there is no parent.
            Assert.Null(builder.EndOrderBy());
        }

        [Fact]
        public void TestNewOrderByBuilderClause()
        {
            var builder = JqlQueryBuilder.NewOrderByBuilder((OrderBy)null);
            Assert.NotNull(builder);

            // make sure the builder is empty initially.
            Assert.Equal(OrderBy.NoOrder, builder.BuildOrderBy());

            //Make sure there is no parent.
            Assert.Null(builder.EndOrderBy());

            OrderBy orderBy = new OrderBy(new SearchSort("test", SortOrder.ASC));
            builder = JqlQueryBuilder.NewOrderByBuilder(orderBy);

            Assert.Equal(orderBy, builder.BuildOrderBy());
            Assert.Equal(new OrderBy(new SearchSort("test", SortOrder.ASC), new SearchSort("cool")), builder.Add("cool").BuildOrderBy());
            Assert.Null(builder.EndOrderBy());
        }

        [Fact]
        public void TestNewOrderbyBuilderQuery()
        {
            var builder = JqlQueryBuilder.NewOrderByBuilder((IQuery) null);
            Assert.NotNull(builder);

            // make sure the builder is empty initially.
            Assert.Equal(OrderBy.NoOrder, builder.BuildOrderBy());

            // make sure there is no parent.
            Assert.Null(builder.EndOrderBy());

            builder = JqlQueryBuilder.NewOrderByBuilder(new Api.Jql.Query.Query(null, null, null));
            Assert.NotNull(builder);

            // make sure the builder is empty initially.
            Assert.Equal(OrderBy.NoOrder, builder.BuildOrderBy());

            // make sure there is no parent.
            Assert.Null(builder.EndOrderBy());

            OrderBy orderBy = new OrderBy(new SearchSort("test", SortOrder.ASC));
            builder = JqlQueryBuilder.NewOrderByBuilder(new Api.Jql.Query.Query(null, orderBy, null));

            Assert.Equal(orderBy, builder.BuildOrderBy());
            Assert.Equal(new OrderBy(new SearchSort("test", SortOrder.ASC), new SearchSort("cool")), builder.Add("cool").BuildOrderBy());
            Assert.Null(builder.EndOrderBy());
        }
    }
}