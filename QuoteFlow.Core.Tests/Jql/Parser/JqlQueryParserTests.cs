using System;
using System.Collections.Generic;
using System.Linq;
using Antlr.Runtime;
using QuoteFlow.Api.Jql.Parser;
using QuoteFlow.Api.Jql.Query;
using QuoteFlow.Api.Jql.Query.Clause;
using QuoteFlow.Api.Jql.Query.Operand;
using QuoteFlow.Api.Jql.Query.Order;
using QuoteFlow.Core.Jql.Parser;
using Xunit;
using Xunit.Extensions;

namespace QuoteFlow.Core.Tests.Jql.Parser
{
    /// <summary>
    /// Tests for the <see cref="JqlQueryParser"/>.
    /// </summary>
    public class JqlQueryParserTests
    {
        // Set of characters that are reserved but not currently used within JQL.
        private string _illegalCharsString = "{}*/%+$#@?;";

        private List<string> _currentWords = new List<string>()
        {
            "empty",
            "null",
            "and",
            "or",
            "not",
            "in",
            "is",
            "cf",
            "issue.property",
            "order",
            "by",
            "desc",
            "asc",
            "on",
            "before",
            "to",
            "after",
            "from",
            "was",
            "changed"
        };

        [Theory]
        [InlineData("priority = \"qwerty\"")]
        [InlineData("priority=\"qwerty\"")]
        [InlineData("priority=qwerty")]
        [InlineData("  priority=qwerty  ")]
        [InlineData("priority=     qwerty order      by priority, other")]
        public void WhiteSpace_IsIgnored_DuringParsing(string jql)
        {
            var expectedClause = new TerminalClause("priority", Operator.EQUALS, "qwerty");
            Evaluate(jql, expectedClause);
        }

        private static readonly List<TerminalClause> MinusClauses = new List<TerminalClause>
        {
            new TerminalClause("key", Operator.EQUALS, "one-1"), // 0
            new TerminalClause("key", Operator.IN, new MultiValueOperand(new SingleValueOperand("one-1"), new SingleValueOperand(-1))), // 1
            new TerminalClause("key", Operator.IN, new MultiValueOperand(new SingleValueOperand("one-1"), new SingleValueOperand("1-1"))), // 2
            new TerminalClause("-78a", Operator.EQUALS, new SingleValueOperand("a")), // 3
            new TerminalClause("numberfield", Operator.GREATER_THAN_EQUALS, -29202), // 4
            new TerminalClause("numberfield", Operator.GREATER_THAN_EQUALS, "-29202-"), // 5
            new TerminalClause("numberfield", Operator.GREATER_THAN_EQUALS, "w-88") // 6
        };
        
        [Theory]
        [InlineData("key = one-1", 0)]
        [InlineData("key in (one-1, -1)", 1)]
        [InlineData("key in (one-1, 1-1)", 2)]
        [InlineData("-78a = a", 3)]
        [InlineData("numberfield >= -29202", 4)]
        [InlineData("numberfield >= -29202-", 5)]
        [InlineData("numberfield >= w-88 ", 6)]
        public void CheckingForMinus_AfterRemoving_FromTheReservedCharsList(string jql, int index)
        {
            // it'd be REALLY nice if we could just pipe this to the inline data.
            var expectedClause = MinusClauses.ElementAt(index);
            Evaluate(jql, expectedClause);
        }

        private static readonly List<TerminalClause> NewLineClauses = new List<TerminalClause>()
        {
            new TerminalClause("newline", Operator.EQUALS, "hello\nworld"), // 0
            new TerminalClause("newline", Operator.EQUALS, "hello\nworld"), // 1
            new TerminalClause("newline", Operator.EQUALS, "hello\r"), // 2
            new TerminalClause("newline", Operator.EQUALS, "hello\r"), // 3
            new TerminalClause("newline", Operator.EQUALS, "\r"), // 4
            new TerminalClause("newline", Operator.EQUALS, "\r"), // 5
            new TerminalClause("new\nline", Operator.EQUALS, "b"), // 6
            new TerminalClause("new\nline", Operator.EQUALS, "b"), // 7
            new TerminalClause("newline", Operator.EQUALS, new FunctionOperand("fun\rc")), // 8
            new TerminalClause("newline", Operator.EQUALS, new FunctionOperand("fun\rc")) // 9
        };

        [Theory]
        [InlineData("newline = \"hello\nworld\"", 0)]
        [InlineData("newline = \"hello\\nworld\"", 1)]
        [InlineData("newline = 'hello\r'", 2)]
        [InlineData("newline = 'hello\\r'", 3)]
        [InlineData("newline = '\r'", 4)]
        [InlineData("newline = '\\r'", 5)]
        [InlineData("'new\nline' = 'b'", 6)]
        [InlineData("'new\\nline' = 'b'", 7)]
        [InlineData("'newline' = 'fun\rc'()", 8)]
        [InlineData("'newline' = 'fun\\rc'()", 9)]
        public void Ensure_NewLine_IsAccepted(string jql, int index)
        {
            var expectedClause = NewLineClauses.ElementAt(index);
            Evaluate(jql, expectedClause);
        }

        private static readonly List<TerminalClause> OtherOperators = new List<TerminalClause>
        {
            new TerminalClause("coolness", Operator.GREATER_THAN_EQUALS, "awesome"), // 0
            new TerminalClause("coolness", Operator.GREATER_THAN, "awesome"), // 1
            new TerminalClause("coolness", Operator.LESS_THAN, "awesome"), // 2
            new TerminalClause("coolness", Operator.LESS_THAN_EQUALS, "awesome"), // 3
            //new TerminalClause("coolness", Operator.NOT_EQUALS, "awesome"), new OrderBy(new SearchSort("coolness", SortOrder.DESC)) // 4
        };
        
        [Theory]
        [InlineData("coolness >= awesome", 0)]
        [InlineData("coolness > awesome", 1)]
        [InlineData("coolness < awesome", 2)]
        [InlineData("coolness <= awesome", 3)]
        //[InlineData("coolness        !=       awesome order     by     coolness desc", 4)]
        public void TestsFor_OtherOperators(string jql, int index)
        {
            var expectedClause = OtherOperators.ElementAt(index);
            Evaluate(jql, expectedClause);
        }

        private static readonly List<TerminalClause> InOperators = new List<TerminalClause>()
        {
            new TerminalClause("language", Operator.IN, new MultiValueOperand("java", "c", "python2")), // 0
            new TerminalClause("languagein", Operator.IN, new MultiValueOperand("java", "c", "python2")), // 1
            new TerminalClause("inlanguage", Operator.IN, new MultiValueOperand("java", "c", "python2")), // 2
            new TerminalClause("pri", Operator.IN, new MultiValueOperand("java", "c", "python2")), // 3
            new TerminalClause("pri", Operator.IN, new MultiValueOperand("java")), // 4
            new TerminalClause("pri", Operator.IN, new MultiValueOperand("java")), // 5
            new TerminalClause("pri", Operator.IN, new MultiValueOperand("java")) // 6
        };

        [Theory]
        [InlineData("language in (java, c, \"python2\")", 0)]
        [InlineData("languagein   IN    (   java, c     , \"python2\")", 1)]
        [InlineData("inlanguage in (java, c, \"python2\")", 2)]
        [InlineData("pri in (java,c,\"python2\")", 3)]
        [InlineData("pri in(java)", 4)]
        [InlineData("pri In(java)", 5)]
        [InlineData("pri iN(java)", 6)]
        public void In_Operator_Tests(string jql, int index)
        {
            var expectedClause = InOperators.ElementAt(index);
            Evaluate(jql, expectedClause);
        }

        private static readonly List<TerminalClause> NotInOperators = new List<TerminalClause>
        {
            new TerminalClause("language", Operator.NOT_IN, new MultiValueOperand("java", "c", "python2")), // 0
            new TerminalClause("languagein", Operator.NOT_IN, new MultiValueOperand("java", "c", "python2")), // 1
            new TerminalClause("inlanguage", Operator.NOT_IN, new MultiValueOperand("java", "c", "python2")), // 2
            new TerminalClause("pri", Operator.NOT_IN, new MultiValueOperand("java", "c", "python2")), // 3
            new TerminalClause("pri", Operator.NOT_IN, new MultiValueOperand("java")), // 4
            new TerminalClause("pri", Operator.NOT_IN, new MultiValueOperand("java")), // 5
            new TerminalClause("pri", Operator.NOT_IN, new MultiValueOperand("java")) // 6
        };

        [Theory]
        [InlineData("language not in (java, c, \"python2\")", 0)]
        [InlineData("languagein  NOT   IN    (   java, c     , \"python2\")", 1)]
        [InlineData("inlanguage not in (java, c, \"python2\")", 2)]
        [InlineData("pri NOT in (java,c,\"python2\")", 3)]
        [InlineData("pri not in(java)", 4)]
        [InlineData("pri NoT In(java)", 5)]
        [InlineData("pri nOT iN(java)", 6)]
        public void NotIn_Operator_Tests(string jql, int index)
        {
            var expectedClause = NotInOperators.ElementAt(index);
            Evaluate(jql, expectedClause);
        }

        private static readonly List<TerminalClause> LikeOperator = new List<TerminalClause>
        {
            new TerminalClause("pri", Operator.LIKE, new SingleValueOperand("stuff")), // 0
            new TerminalClause("pri", Operator.LIKE, new SingleValueOperand("stuff")), // 1
            new TerminalClause("pri", Operator.LIKE, new SingleValueOperand(12)), // 2
            new TerminalClause("pri", Operator.LIKE, new SingleValueOperand(12)), // 3
            new TerminalClause("pri", Operator.LIKE, new MultiValueOperand(new List<IOperand>() { new SingleValueOperand("stuff"), new SingleValueOperand(12) })) // 4
        };

        [Theory]
        [InlineData("pri ~ stuff", 0)]
        [InlineData("pri~stuff", 1)]
        [InlineData("pri ~ 12", 2)]
        [InlineData("pri~12", 3)]
        [InlineData("pri ~ (\"stuff\", 12)", 4)]
        public void Like_Operator_Tests(string jql, int index)
        {
            var expectedClause = LikeOperator.ElementAt(index);
            Evaluate(jql, expectedClause);
        }

        private static readonly List<TerminalClause> NotLikeOperator = new List<TerminalClause>
        {
            new TerminalClause("pri", Operator.NOT_LIKE, new SingleValueOperand("stuff")), // 0
            new TerminalClause("pri", Operator.NOT_LIKE, new SingleValueOperand("stuff")), // 1
            new TerminalClause("pri", Operator.NOT_LIKE, new SingleValueOperand(12)), // 2
            new TerminalClause("pri", Operator.NOT_LIKE, new SingleValueOperand(12)), // 3
            new TerminalClause("pri", Operator.NOT_LIKE, new MultiValueOperand(new List<IOperand>() { new SingleValueOperand("stuff"), new SingleValueOperand(12) })) // 4
        };

        [Theory]
        [InlineData("pri !~ stuff", 0)]
        [InlineData("pri!~stuff", 1)]
        [InlineData("pri !~ 12", 2)]
        [InlineData("pri!~12", 3)]
        [InlineData("pri !~ (\"stuff\", 12)", 4)]
        public void NotLike_Operator_Tests(string jql, int index)
        {
            var expectedClause = NotLikeOperator.ElementAt(index);
            Evaluate(jql, expectedClause);
        }

        private static readonly List<TerminalClause> IsOperator = new List<TerminalClause>
        {
            new TerminalClause("pri", Operator.IS, new SingleValueOperand("stuff")), // 0
            new TerminalClause("pri", Operator.IS, new SingleValueOperand("stuff")), // 1
            new TerminalClause("pri", Operator.IS, new EmptyOperand()) // 2
        };

        [Theory]
        [InlineData("pri IS stuff", 0)]
        [InlineData("pri is stuff", 1)]
        [InlineData("pri IS EMPTY", 2)]
        public void Is_Operator_Tests(string jql, int index)
        {
            var expectedClause = IsOperator.ElementAt(index);
            Evaluate(jql, expectedClause);
        }

        private static readonly List<TerminalClause> IsNotOperator = new List<TerminalClause>
        {
            new TerminalClause("pri", Operator.IS_NOT, new SingleValueOperand("stuff")), // 0
            new TerminalClause("pri", Operator.IS_NOT, new SingleValueOperand("stuff")), // 1
            new TerminalClause("pri", Operator.IS_NOT, new SingleValueOperand("stuff")), // 2
            new TerminalClause("pri", Operator.IS_NOT, new SingleValueOperand("stuff")) // 3
        };

        [Theory]
        [InlineData("pri IS NOT stuff", 0)]
        [InlineData("pri IS not stuff", 1)]
        [InlineData("pri is Not stuff", 2)]
        [InlineData("pri is not stuff", 3)]
        public void IsNot_Operator_Tests(string jql, int index)
        {
            var expectedClause = IsNotOperator.ElementAt(index);
            Evaluate(jql, expectedClause);
        }

        [Fact]
        public void NestedBehavior_Of_InClause_WorksCorrectly()
        {
            var jql = "pri iN((java), duke)";
            var nested = new List<IOperand> {new MultiValueOperand("java"), new SingleValueOperand("duke")};
            Evaluate(jql, new TerminalClause("pri", Operator.IN, new MultiValueOperand(nested)));
        }

        private static readonly List<TerminalClause> NumberClauses = new List<TerminalClause>
        {
            new TerminalClause("priority", Operator.EQUALS, 12345), // 0
            new TerminalClause("priority", Operator.EQUALS, -12345), // 1
            new TerminalClause("priority", Operator.EQUALS, "12a345"), // 2
            new TerminalClause("priority", Operator.EQUALS, "12345a") // 3
        };

        [Theory]
        [InlineData("priority = 12345", 0)]
        [InlineData("priority = -12345", 1)]
        [InlineData("priority = \"12a345\"", 2)]
        [InlineData("priority = 12345a", 3)]
        public void EnsureNumbers_Are_ReturnedCorrectly(string jql, int index)
        {
            var expectedClause = NumberClauses.ElementAt(index);
            Evaluate(jql, expectedClause);
        }

        [Fact]
        public void EnsureQuotedNumber_IsReturned_AsString()
        {
            var jql = "priority = \"12345\"";
            var expectedClause = new TerminalClause("priority", Operator.EQUALS, "12345");
            Evaluate(jql, expectedClause);
        }

        [Fact]
        public void InvalidNumber_Should_ReturnAsString()
        {
            var jql = "priority=\"12a345\"";
            var expectedClause = new TerminalClause("priority", Operator.EQUALS, "12a345");
            Evaluate(jql, expectedClause);
        }

        [Fact]
        public void DotSeperatedValues_ShouldBe_RHV()
        {
            string jql = "version= 1.2.3";
            var expectedClause = new TerminalClause("version", Operator.EQUALS, "1.2.3");
            Evaluate(jql, expectedClause);
        }

        private static readonly List<TerminalClause> EmptyOperand = new List<TerminalClause>
        {
            new TerminalClause("testfield", Operator.EQUALS, new EmptyOperand()), // 0
            new TerminalClause("testfield", Operator.EQUALS, new EmptyOperand()), // 1
            new TerminalClause("testfield", Operator.EQUALS, new EmptyOperand()), // 2
            new TerminalClause("testfield", Operator.EQUALS, new EmptyOperand()), // 3
            new TerminalClause("testfield", Operator.EQUALS, "null"), // 4
            new TerminalClause("testfield", Operator.EQUALS, "NULL"), // 5
            new TerminalClause("testfield", Operator.EQUALS, "EMPTY"), // 6
            new TerminalClause("testfield", Operator.EQUALS, "empty") // 7
        };

        [Theory]
        [InlineData("testfield = EMPTY", 0)]
        [InlineData("testfield = empty", 1)]
        [InlineData("testfield = NULL", 2)]
        [InlineData("testfield = null", 3)]
        [InlineData("testfield = \"null\"", 4)]
        [InlineData("testfield = \"NULL\"", 5)]
        [InlineData("testfield = \"EMPTY\"", 6)]
        [InlineData("testfield = \"empty\"", 7)]
        public void Empty_Operand_Tests(string jql, int index)
        {
            var expectedClause = EmptyOperand.ElementAt(index);
            Evaluate(jql, expectedClause);
        }

        private static readonly List<TerminalClause> QuotedStrings = new List<TerminalClause>
        {
            new TerminalClause("priority", Operator.EQUALS, "a big string ~ != foo and priority = haha "), // 0
            new TerminalClause("priority", Operator.EQUALS, "") // 1
        };

        [Theory]
        [InlineData("priority = \"a big string ~ != foo and priority = haha \"", 0)]
        [InlineData("priority = \"\"", 1)]
        public void QuotedStrings_WithCharacters_ThatMustBeQuoted_Tests(string jql, int index)
        {
            var expectedClause = QuotedStrings.ElementAt(index);
            Evaluate(jql, expectedClause);
        }

        private static readonly List<TerminalClause> Escaping = new List<TerminalClause>
        {
            new TerminalClause("priority", Operator.EQUALS, "a \n new \r line"), // 0
            new TerminalClause("priority", Operator.EQUALS, "Tab:\t NewLine:\n Carrage Return:\r"), // 1
            new TerminalClause("priority", Operator.EQUALS, "Quote:\" Single:' Back Slash:\\ Space: "), // 2
            new TerminalClause("priority", Operator.EQUALS, "Unicode: \ufeef1 Unicode2: \u6eef"), // 3
            new TerminalClause("priority", Operator.EQUALS, "Escape\" don't"), // 4
            new TerminalClause("priority", Operator.EQUALS, "Escape' don\"t") // 5
        };

        [Theory]
        [InlineData("priority = \"a \\n new \\r line\"", 0)]
        [InlineData("priority = \"Tab:\\t NewLine:\\n Carrage Return:\\r\"", 1)]
        [InlineData("priority = \"Quote:\\\" Single:\\' Back Slash:\\\\ Space:\\ \"", 2)]
        //[InlineData("priority = \"Unicode: \\ufeeF1 Unicode2: \\u6EEF\"", 3)]
        [InlineData("priority = 'Escape\" don\\'t'", 4)]
        [InlineData("priority = \"Escape' don\\\"t\"", 5)]
        public void Escaping_Tests(string jql, int index)
        {
            var expectedClause = Escaping.ElementAt(index);
            Evaluate(jql, expectedClause);
        }

        private static readonly List<TerminalClause> FunctionClauses = new List<TerminalClause>
        {
            new TerminalClause("priority", Operator.EQUALS, new FunctionOperand("higherThan", new List<string>() { "Major" })), // 0
            new TerminalClause("priority", Operator.IN, new FunctionOperand("randomName", new List<string> { "Major", "Minor", "cool", "-65784" })), // 1
            new TerminalClause("priority", Operator.GREATER_THAN_EQUALS, new FunctionOperand("randomName", new List<string>())), // 2
            new TerminalClause("pri", Operator.NOT_IN, new FunctionOperand("fun", "name\u0082e"))
        };

        [Theory]
        [InlineData("priority = higherThan(Major)", 0)]
        [InlineData("priority In     randomName(Major, Minor,      \"cool\", -65784)", 1)]
        [InlineData("priority    >=    randomName()", 2)]
        //[InlineData("pri not in fun(name\u0082e", 3)]
        public void Function_Call_Tests(string jql, int index)
        {
            var expectedClause = FunctionClauses.ElementAt(index);
            Evaluate(jql, expectedClause);
        }

        private static readonly List<IClause> StringBreaks = new List<IClause>
        {
            new AndClause(new TerminalClause("a", Operator.EQUALS, "b"), new TerminalClause("c", Operator.EQUALS, "d")), // 0
            new AndClause(new TerminalClause("a", Operator.EQUALS, "b"), new TerminalClause("c", Operator.EQUALS, "d")), // 1
            new OrClause(new TerminalClause("a", Operator.EQUALS, "b"), new TerminalClause("c", Operator.EQUALS, "d")), // 2
            new OrClause(new TerminalClause("a", Operator.EQUALS, "b"), new TerminalClause("c", Operator.EQUALS, "d")), // 3
            new TerminalClause("a", Operator.LESS_THAN, "b"), // 4
            new TerminalClause("a", Operator.GREATER_THAN, "b"), // 5
            new TerminalClause("a", Operator.LIKE, "b")
        };

        [Theory]
        [InlineData("a=b&c=d", 0)]
        [InlineData("a=b&&c=d", 1)]
        [InlineData("a=b|c=d", 2)]
        [InlineData("a=b||c=d", 3)]
        [InlineData("a<b", 4)]
        [InlineData("a>b", 5)]
        [InlineData("a~b", 6)]
        public void String_Break_Tests(string jql, int index)
        {
            var expectedClause = StringBreaks.ElementAt(index);
            Evaluate(jql, expectedClause);
        }

        private static readonly TerminalClause _priorityEqualsMajor = new TerminalClause("priority", Operator.EQUALS, "major");
        private static readonly TerminalClause _fooGtBarFunc = new TerminalClause("foo", Operator.GREATER_THAN, new FunctionOperand("bar"));
        private static readonly List<IClause> AndOperator = new List<IClause>
        {
            new AndClause(_priorityEqualsMajor, _fooGtBarFunc), // 0
            new AndClause(new TerminalClause("priority", Operator.EQUALS, "majorand"), _fooGtBarFunc), // 1
            new AndClause(_priorityEqualsMajor, _fooGtBarFunc), // 2
            new AndClause(new TerminalClause("priority", Operator.NOT_EQUALS, "major"), _fooGtBarFunc), // 3
            new AndClause(new TerminalClause("priority", Operator.NOT_EQUALS, "major"), _fooGtBarFunc), // 4
            new AndClause(new TerminalClause("priority", Operator.NOT_EQUALS, "andmajor"), _fooGtBarFunc), // 5
        };

        [Theory]
        [InlineData("priority = major and foo > bar()", 0)]
        [InlineData("priority = majorand and foo>bar()", 1)]
        [InlineData("priority = major and foo > bar()", 2)]
        [InlineData("priority != major    and      foo >      bar()", 3)]
        [InlineData("priority != major    &&      foo >      bar()", 4)]
        [InlineData("priority != andmajor    &      foo >      bar()", 5)]
        //[InlineData("priority != andmajor    and      foo >      bar() order by priority     DESC,      foo", 6)]
        public void And_Operator_Tests(string jql, int index)
        {
            var expectedClause = AndOperator.ElementAt(index);
            Evaluate(jql, expectedClause);
        }

        private static readonly TerminalClause _priorityEqualsMinor = new TerminalClause("priority", Operator.EQUALS, "minor");
        private static readonly List<IClause> OrOperator = new List<IClause>
        {
            new OrClause(_priorityEqualsMajor, _fooGtBarFunc), // 0
            new OrClause(_priorityEqualsMajor, _fooGtBarFunc), // 1
            new OrClause(_priorityEqualsMajor, _fooGtBarFunc, _priorityEqualsMinor), // 2
            new OrClause(_priorityEqualsMajor, _fooGtBarFunc, _priorityEqualsMinor), // 3
            new OrClause(_priorityEqualsMajor, _fooGtBarFunc, _priorityEqualsMinor) // 4
        };

        [Theory]
        [InlineData("priority = major or foo > bar()", 0)]
        [InlineData("priority = major or foo > bar()", 1)]
        [InlineData("priority = major or foo > bar() or priority = minor", 2)]
        [InlineData("priority = major || foo > bar() | priority = minor", 3)]
        [InlineData("priority = major or foo > bar() || priority = minor", 4)]
        public void Or_Operator_Tests(string jql, int index)
        {
            var expectedClause = OrOperator.ElementAt(index);
            Evaluate(jql, expectedClause);
        }

        private static readonly List<IClause> NotOperator = new List<IClause>
        {
            new OrClause(new NotClause(_priorityEqualsMajor), _fooGtBarFunc, _priorityEqualsMinor), // 0
            new OrClause(new NotClause(_priorityEqualsMajor), new AndClause(_fooGtBarFunc, _priorityEqualsMinor)), // 1
            new OrClause(new NotClause(_priorityEqualsMajor), new AndClause(new NotClause(_fooGtBarFunc), _priorityEqualsMinor)), // 2
            new AndClause(new NotClause(new OrClause(_priorityEqualsMajor, new NotClause(_fooGtBarFunc))), _priorityEqualsMinor) // 3
        };

        [Theory]
        [InlineData("not priority = major or foo > bar() or priority = minor", 0)]
        [InlineData("not priority = major or foo > bar() AnD priority=\"minor\"", 1)]
        [InlineData("not priority = major or not foo > bar() AnD priority=\"minor\"", 2)]
        [InlineData("not (priority = major or not foo > bar()) AnD priority=\"minor\"", 3)]
        public void Not_Operator_Tests(string jql, int index)
        {
            var expectedClause = NotOperator.ElementAt(index);
            Evaluate(jql, expectedClause);
        }

        [Fact]
        public void ExclamationNOT_Operator_Negates()
        {
            string jql = "! (priority = major or ! foo > bar()) AnD priority=\"minor\"";
            var expectedClause = new AndClause(new NotClause(new OrClause(_priorityEqualsMajor, new NotClause(_fooGtBarFunc))), _priorityEqualsMinor);
            Evaluate(jql, expectedClause);
        }

        private static readonly List<JqlParseErrorMessage> InvalidParsingInputMessages = new List<JqlParseErrorMessage>()
        {
            JqlParseErrorMessages.IllegalCharacter('\f', 2, 1), // 0
            JqlParseErrorMessages.BadOperator(CreateEofToken(1, 3)) // 1
        };

        [Theory]
        [InlineData("Tabs and Spaces string", "  \t  \n\r\f   ", 0)]
        [InlineData("Bare field", "foo", 1)]
        public void Tests_For_Invalid_Parsing_Input(string desc, string jql, int index)
        {
            var expectedParseError = InvalidParsingInputMessages.ElementAt(index);
            EvaluateError(desc, jql, expectedParseError);
        }

        private static void Evaluate(string jql, IClause expectedClause)
        {
            var parser = new JqlQueryParser();
            var query = parser.ParseQuery(jql);
            var whereClause = query.WhereClause;
            Assert.Same(jql, query.QueryString);
            Assert.Equal(expectedClause, whereClause);
            //Assert.Equal(orderBy, query.OrderByClause);
        }

        private static void EvaluateError(string desc, string inputJql, JqlParseErrorMessage parseError)
        {
            var parser = new JqlQueryParser();
            try
            {
                var query = parser.ParseQuery(inputJql);
                Assert.True(false, $"Expected exception on input '{inputJql}' but got query '{query}'");
            }
            catch (JqlParseException ex)
            {
                var actual = ex.ParseErrorMessage;
                Assert.Equal(parseError, actual);
            }
        }

        private static IToken CreateEofToken(int line, int position)
        {
            return CreateToken(null, line, position, TokenTypes.EndOfFile);
        }

        private static IToken CreateToken(string text, int line, int position, int type = 10)
        {
            return new CommonToken(type)
            {
                Line = line,
                CharPositionInLine = position,
                Text = text
            };
        }
    }
}
