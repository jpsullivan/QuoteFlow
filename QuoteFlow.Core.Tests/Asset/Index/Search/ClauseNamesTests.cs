using System;
using System.Collections.Generic;
using QuoteFlow.Api.Asset.Search;
using Xunit;

namespace QuoteFlow.Core.Tests.Asset.Index.Search
{
    public class ClauseNamesTests
    {
        public class TheCtor
        {
            [Fact]
            public void BadArguments()
            {
                var goodNames = new List<string> {"jack"};
                var nullNames = new List<string> { null };
                var nullNamesInArray = new string[] { null };
                var emptyName = new List<string> {"jill", string.Empty};

                try
                {
                    new ClauseNames(null);
                    Assert.True(false, "Expected exception");
                }
                catch (ArgumentNullException ex)
                {
                }

                try
                {
                    new ClauseNames("  ");
                    Assert.True(false, "Expected exception");
                }
                catch (ArgumentNullException expected)
                {
                }

                try
                {
                    new ClauseNames("dude", (List<string>)null);
                    Assert.True(false, "Should not accept these arguments.");
                }
                catch (ArgumentNullException e)
                {
                    //expected
                }

                try
                {
                    new ClauseNames("dude", (string[])null);
                    Assert.True(false, "Should not accept these arguments.");
                }
                catch (ArgumentNullException e)
                {
                    //expected
                }

                try
                {
                    new ClauseNames("dude", nullNames);
                    Assert.True(false, "Should not accept these arguments.");
                }
                catch (ArgumentNullException e)
                {
                    //expected
                }

                try
                {
                    new ClauseNames("dude", nullNamesInArray);
                    Assert.True(false, "Should not accept these arguments.");
                }
                catch (ArgumentNullException e)
                {
                    //expected
                }

                try
                {
                    new ClauseNames("dude", emptyName);
                    Assert.True(false, "Should not accept these arguments.");
                }
                catch (ArgumentNullException e)
                {
                    //expected
                }

                try
                {
                    new ClauseNames(null, goodNames);
                    Assert.True(false, "Should not accept these arguments.");
                }
                catch (ArgumentNullException e)
                {
                    //expected
                }
            }

            [Fact]
            public void HappyPath()
            {
                var goodNames = new List<string> { "jack" };
                var names = new ClauseNames("dude", goodNames);

                Assert.True(names.Contains("dude"));
                Assert.True(names.Contains("jack"));

                var names2 = new ClauseNames("dude", "jack");
                Assert.Equal(names, names2);
            }
        }
    }
}