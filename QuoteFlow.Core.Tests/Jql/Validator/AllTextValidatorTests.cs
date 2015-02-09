using Moq;
using QuoteFlow.Api.Jql.Query;
using QuoteFlow.Api.Jql.Query.Clause;
using QuoteFlow.Api.Jql.Validator;
using QuoteFlow.Api.Models;
using QuoteFlow.Api.Util;
using QuoteFlow.Core.Jql.Validator;
using Xunit;

namespace QuoteFlow.Core.Tests.Jql.Validator
{
    public class AllTextValidatorTests
    {
        private static readonly User Anonymous = null;

        [Fact]
        public void ValidateBadOperator()
        {
            var clause = new TerminalClause("text", Operator.LIKE, "test");
            var messageSet = new MessageSet();
            messageSet.AddErrorMessage("something");

            var supportedOperatorsValidator = new Mock<SupportedOperatorsValidator>();
            supportedOperatorsValidator.Setup(x => x.Validate(Anonymous, clause)).Returns(messageSet);

            var commentValidator = new CommentValidator(MockJqlOperandResolver.CreateSimpleSupport());
            var validator = new AllTextValidator(commentValidator, supportedOperatorsValidator.Object);

            var result = validator.Validate(Anonymous, clause);
            Assert.Equal(messageSet, result);
        }

        [Fact]
        public void DelegateCalled()
        {
            var clause = new TerminalClause("text", Operator.LIKE, "test");
            var messageSet = new MessageSet();

            var supportedOperatorsValidator = new Mock<SupportedOperatorsValidator>();
            supportedOperatorsValidator.Setup(x => x.Validate(Anonymous, clause)).Returns(messageSet);

            var commentValidator = new CommentValidatorHelper(MockJqlOperandResolver.CreateSimpleSupport(), messageSet);
            var validator = new AllTextValidator(commentValidator, supportedOperatorsValidator.Object);

            var result = validator.Validate(Anonymous, clause);
            Assert.Equal(messageSet, result);
            Assert.True(commentValidator.ValidatedCalled);
        }

        private class CommentValidatorHelper : CommentValidator
        {
            private IMessageSet messageSet;
            public bool ValidatedCalled { get; set; }

            public CommentValidatorHelper(MockJqlOperandResolver operandResolver, IMessageSet messageSet) : base(operandResolver)
            {
                this.messageSet = messageSet;
            }

            public override IMessageSet Validate(User searcher, ITerminalClause terminalClause)
            {
                ValidatedCalled = true;
                return messageSet;
            }
        }

    }
}
