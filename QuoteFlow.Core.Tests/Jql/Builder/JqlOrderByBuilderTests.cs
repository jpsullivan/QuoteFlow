using System.Linq;
using QuoteFlow.Api.Asset.Search.Constants;
using QuoteFlow.Api.Jql.Query;
using QuoteFlow.Api.Jql.Query.Clause;
using QuoteFlow.Api.Jql.Query.Order;
using QuoteFlow.Core.Jql.Builder;
using Xunit;

namespace QuoteFlow.Core.Tests.Jql.Builder
{
    public class JqlOrderByBuilderTests
    {
        private JqlOrderByBuilder _orderByBuilder;
        private IJqlClauseBuilder _whereClauseBuilder;
        private JqlQueryBuilder _parentBuilder;

        public JqlOrderByBuilderTests()
        {
            _parentBuilder = JqlQueryBuilder.NewBuilder();
            _whereClauseBuilder = _parentBuilder.Where();
            _orderByBuilder = _parentBuilder.OrderBy();
        }

        [Fact]
        public void TestClear()
        {
            _orderByBuilder.Add("test");
            Assert.Equal(new OrderBy(new SearchSort("test")), _orderByBuilder.BuildOrderBy());

            _orderByBuilder.Clear();
            Assert.Equal(OrderBy.NoOrder, _orderByBuilder.BuildOrderBy());

            _orderByBuilder.Add("test").Add("test2");
            Assert.Equal(new OrderBy(new SearchSort("test"), new SearchSort("test2")), _orderByBuilder.BuildOrderBy());
            
            Assert.Equal(OrderBy.NoOrder, _orderByBuilder.Clear().BuildOrderBy());
        }

        [Fact]
        public void TestCloneFromExisting()
        {
            var existingOrderBy = new OrderBy(
                new SearchSort("Test", SortOrder.DESC),
                new SearchSort("Blah", SortOrder.ASC), 
                new SearchSort("Heee"), 
                new SearchSort("ASC", "Haaaa"));

            _orderByBuilder.SetSorts(existingOrderBy);
            Assert.Equal(existingOrderBy, _orderByBuilder.BuildOrderBy());
        }

        [Fact]
        public void TestEndOrderBy()
        {
            Assert.Equal(_parentBuilder, _orderByBuilder.EndOrderBy());
        }

        [Fact]
        public void TestSetSorts()
        {
            var existingOrderBy = new OrderBy(
                new SearchSort("Test", SortOrder.DESC),
                new SearchSort("Blah", SortOrder.ASC),
                new SearchSort("Heee"),
                new SearchSort("ASC", "Haaaa"));

            _orderByBuilder.Description(SortOrder.ASC);
            _orderByBuilder.SetSorts(existingOrderBy.SearchSorts);
            Assert.Equal(existingOrderBy, _orderByBuilder.BuildOrderBy());
        }

        [Fact]
        public void TestGetQuery()
        {
            _whereClauseBuilder.Catalog("MSA");
            _orderByBuilder.Description(SortOrder.ASC).Add("Test", SortOrder.DESC);

            var expectedWhere = new TerminalClause("catalog", Operator.EQUALS, "MSA");
            var expectedOrderBy = new OrderBy(new SearchSort("description", SortOrder.ASC), new SearchSort("Test", SortOrder.DESC));
            var expectedQuery = new Api.Jql.Query.Query(expectedWhere, expectedOrderBy, null);
            Assert.Equal(expectedQuery, _orderByBuilder.BuildQuery());
        }

        [Fact]
        public void TestGetQueryNoParent()
        {
            var builder = new JqlOrderByBuilder(null);
            builder.Description(SortOrder.ASC).Add("Test", SortOrder.DESC);

            var expectedOrderBy = new OrderBy(new SearchSort("description", SortOrder.ASC), new SearchSort("Test", SortOrder.DESC));
            var expectedQuery = new Api.Jql.Query.Query(null, expectedOrderBy, null);
            Assert.Equal(expectedQuery, builder.BuildQuery());
        }

        [Fact]
        public void TestEmptyOrderBy()
        {
            var orderBy = _orderByBuilder.BuildOrderBy();
            Assert.False(orderBy.SearchSorts.Any());
        }

        [Fact]
        public void TestAddStringClause()
        {
            var orderBy = _orderByBuilder.Add("Test", SortOrder.DESC).BuildOrderBy();
            Assert.Equal(1, orderBy.SearchSorts.Count);
            Assert.Equal(new SearchSort("Test", SortOrder.DESC), orderBy.SearchSorts.First());
        }

        [Fact]
        public void TestAddStringClauseNoOrder()
        {
            var orderBy = _orderByBuilder.Add("Test").BuildOrderBy();
            Assert.Equal(1, orderBy.SearchSorts.Count);
            Assert.Equal(new SearchSort("Test"), orderBy.SearchSorts.First());
        }

        [Fact]
        public void TestDescriptionAsc()
        {
            var orderBy = _orderByBuilder.Description(SortOrder.ASC).BuildOrderBy();
            Assert.Equal(1, orderBy.SearchSorts.Count);
            Assert.Equal(
                new SearchSort(SystemSearchConstants.ForDescription().JqlClauseNames.PrimaryName, SortOrder.ASC),
                orderBy.SearchSorts.First());
        }

        [Fact]
        public void TestDescriptionDesc()
        {
            var orderBy = _orderByBuilder.Description(SortOrder.DESC).BuildOrderBy();
            Assert.Equal(1, orderBy.SearchSorts.Count);
            Assert.Equal(
                new SearchSort(SystemSearchConstants.ForDescription().JqlClauseNames.PrimaryName, SortOrder.DESC),
                orderBy.SearchSorts.First());
        }

        [Fact]
        public void TestDescriptionNone()
        {
            var orderBy = _orderByBuilder.Description(SortOrder.None).BuildOrderBy();
            Assert.Equal(1, orderBy.SearchSorts.Count);
            Assert.Equal(
                new SearchSort(SystemSearchConstants.ForDescription().JqlClauseNames.PrimaryName, SortOrder.None),
                orderBy.SearchSorts.First());
        }

        [Fact]
        public void TestDescriptionAddedFirst()
        {
            var orderBy = _orderByBuilder.Catalog(SortOrder.None).Description(SortOrder.None, true).BuildOrderBy();
            Assert.Equal(2, orderBy.SearchSorts.Count);
            Assert.Equal(new SearchSort(SystemSearchConstants.ForDescription().JqlClauseNames.PrimaryName, SortOrder.None),
                orderBy.SearchSorts.First());
        }

        [Fact]
        public void TestAssetIdAsc()
        {
            var orderBy = _orderByBuilder.AssetId(SortOrder.ASC).BuildOrderBy();
            Assert.Equal(1, orderBy.SearchSorts.Count);
            Assert.Equal(
                new SearchSort(SystemSearchConstants.ForAssetId().JqlClauseNames.PrimaryName, SortOrder.ASC),
                orderBy.SearchSorts.First());
        }

        [Fact]
        public void TestAssetIdDesc()
        {
            var orderBy = _orderByBuilder.AssetId(SortOrder.DESC).BuildOrderBy();
            Assert.Equal(1, orderBy.SearchSorts.Count);
            Assert.Equal(
                new SearchSort(SystemSearchConstants.ForAssetId().JqlClauseNames.PrimaryName, SortOrder.DESC),
                orderBy.SearchSorts.First());
        }

        [Fact]
        public void TestAssetIdNone()
        {
            var orderBy = _orderByBuilder.AssetId(SortOrder.None).BuildOrderBy();
            Assert.Equal(1, orderBy.SearchSorts.Count);
            Assert.Equal(
                new SearchSort(SystemSearchConstants.ForAssetId().JqlClauseNames.PrimaryName, SortOrder.None),
                orderBy.SearchSorts.First());
        }

        [Fact]
        public void TestAssetIdAddedFirst()
        {
            var orderBy = _orderByBuilder.Catalog(SortOrder.None).AssetId(SortOrder.None, true).BuildOrderBy();
            Assert.Equal(2, orderBy.SearchSorts.Count);
            Assert.Equal(new SearchSort(SystemSearchConstants.ForAssetId().JqlClauseNames.PrimaryName, SortOrder.None),
                orderBy.SearchSorts.First());
        }

        [Fact]
        public void TestCatalogAsc()
        {
            var orderBy = _orderByBuilder.Catalog(SortOrder.ASC).BuildOrderBy();
            Assert.Equal(1, orderBy.SearchSorts.Count);
            Assert.Equal(
                new SearchSort(SystemSearchConstants.ForCatalog().JqlClauseNames.PrimaryName, SortOrder.ASC),
                orderBy.SearchSorts.First());
        }

        [Fact]
        public void TestCatalogDesc()
        {
            var orderBy = _orderByBuilder.Catalog(SortOrder.DESC).BuildOrderBy();
            Assert.Equal(1, orderBy.SearchSorts.Count);
            Assert.Equal(
                new SearchSort(SystemSearchConstants.ForCatalog().JqlClauseNames.PrimaryName, SortOrder.DESC),
                orderBy.SearchSorts.First());
        }

        [Fact]
        public void TestCatalogNone()
        {
            var orderBy = _orderByBuilder.Catalog(SortOrder.None).BuildOrderBy();
            Assert.Equal(1, orderBy.SearchSorts.Count);
            Assert.Equal(
                new SearchSort(SystemSearchConstants.ForCatalog().JqlClauseNames.PrimaryName, SortOrder.None),
                orderBy.SearchSorts.First());
        }

        [Fact]
        public void TestCatalogAddedFirst()
        {
            var orderBy = _orderByBuilder.Description(SortOrder.None).Catalog(SortOrder.None, true).BuildOrderBy();
            Assert.Equal(2, orderBy.SearchSorts.Count);
            Assert.Equal(new SearchSort(SystemSearchConstants.ForCatalog().JqlClauseNames.PrimaryName, SortOrder.None),
                orderBy.SearchSorts.First());
        }

        [Fact]
        public void TestManufacturerAsc()
        {
            var orderBy = _orderByBuilder.Manufacturer(SortOrder.ASC).BuildOrderBy();
            Assert.Equal(1, orderBy.SearchSorts.Count);
            Assert.Equal(
                new SearchSort(SystemSearchConstants.ForManufacturer().JqlClauseNames.PrimaryName, SortOrder.ASC),
                orderBy.SearchSorts.First());
        }

        [Fact]
        public void TestManufacturerDesc()
        {
            var orderBy = _orderByBuilder.Manufacturer(SortOrder.DESC).BuildOrderBy();
            Assert.Equal(1, orderBy.SearchSorts.Count);
            Assert.Equal(
                new SearchSort(SystemSearchConstants.ForManufacturer().JqlClauseNames.PrimaryName, SortOrder.DESC),
                orderBy.SearchSorts.First());
        }

        [Fact]
        public void TestManufacturerNone()
        {
            var orderBy = _orderByBuilder.Manufacturer(SortOrder.None).BuildOrderBy();
            Assert.Equal(1, orderBy.SearchSorts.Count);
            Assert.Equal(
                new SearchSort(SystemSearchConstants.ForManufacturer().JqlClauseNames.PrimaryName, SortOrder.None),
                orderBy.SearchSorts.First());
        }

        [Fact]
        public void TestManufacturerAddedFirst()
        {
            var orderBy = _orderByBuilder.Description(SortOrder.None).Manufacturer(SortOrder.None, true).BuildOrderBy();
            Assert.Equal(2, orderBy.SearchSorts.Count);
            Assert.Equal(new SearchSort(SystemSearchConstants.ForManufacturer().JqlClauseNames.PrimaryName, SortOrder.None),
                orderBy.SearchSorts.First());
        }

        [Fact]
        public void TestCreatedDateAsc()
        {
            var orderBy = _orderByBuilder.CreatedDate(SortOrder.ASC).BuildOrderBy();
            Assert.Equal(1, orderBy.SearchSorts.Count);
            Assert.Equal(
                new SearchSort(SystemSearchConstants.ForCreatedDate().JqlClauseNames.PrimaryName, SortOrder.ASC),
                orderBy.SearchSorts.First());
        }

        [Fact]
        public void TestCreatedDateDesc()
        {
            var orderBy = _orderByBuilder.CreatedDate(SortOrder.DESC).BuildOrderBy();
            Assert.Equal(1, orderBy.SearchSorts.Count);
            Assert.Equal(
                new SearchSort(SystemSearchConstants.ForCreatedDate().JqlClauseNames.PrimaryName, SortOrder.DESC),
                orderBy.SearchSorts.First());
        }

        [Fact]
        public void TestCreatedDateNone()
        {
            var orderBy = _orderByBuilder.CreatedDate(SortOrder.None).BuildOrderBy();
            Assert.Equal(1, orderBy.SearchSorts.Count);
            Assert.Equal(
                new SearchSort(SystemSearchConstants.ForCreatedDate().JqlClauseNames.PrimaryName, SortOrder.None),
                orderBy.SearchSorts.First());
        }

        [Fact]
        public void TestCreatedDateAddedFirst()
        {
            var orderBy = _orderByBuilder.Description(SortOrder.None).CreatedDate(SortOrder.None, true).BuildOrderBy();
            Assert.Equal(2, orderBy.SearchSorts.Count);
            Assert.Equal(new SearchSort(SystemSearchConstants.ForCreatedDate().JqlClauseNames.PrimaryName, SortOrder.None),
                orderBy.SearchSorts.First());
        }

        [Fact]
        public void TestSummaryAsc()
        {
            var orderBy = _orderByBuilder.Summary(SortOrder.ASC).BuildOrderBy();
            Assert.Equal(1, orderBy.SearchSorts.Count);
            Assert.Equal(
                new SearchSort(SystemSearchConstants.ForSummary().JqlClauseNames.PrimaryName, SortOrder.ASC),
                orderBy.SearchSorts.First());
        }

        [Fact]
        public void TestSummaryDesc()
        {
            var orderBy = _orderByBuilder.Summary(SortOrder.DESC).BuildOrderBy();
            Assert.Equal(1, orderBy.SearchSorts.Count);
            Assert.Equal(
                new SearchSort(SystemSearchConstants.ForSummary().JqlClauseNames.PrimaryName, SortOrder.DESC),
                orderBy.SearchSorts.First());
        }

        [Fact]
        public void TestSummaryNone()
        {
            var orderBy = _orderByBuilder.Summary(SortOrder.None).BuildOrderBy();
            Assert.Equal(1, orderBy.SearchSorts.Count);
            Assert.Equal(
                new SearchSort(SystemSearchConstants.ForSummary().JqlClauseNames.PrimaryName, SortOrder.None),
                orderBy.SearchSorts.First());
        }

        [Fact]
        public void TestSummaryAddedFirst()
        {
            var orderBy = _orderByBuilder.Description(SortOrder.None).Summary(SortOrder.None, true).BuildOrderBy();
            Assert.Equal(2, orderBy.SearchSorts.Count);
            Assert.Equal(new SearchSort(SystemSearchConstants.ForSummary().JqlClauseNames.PrimaryName, SortOrder.None),
                orderBy.SearchSorts.First());
        }
    }
}