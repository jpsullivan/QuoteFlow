using System;
using System.Collections.Generic;
using System.Linq;
using Antlr.Runtime;
using QuoteFlow.Infrastructure.Exceptions.Antlr;
using QuoteFlow.Infrastructure.Helpers;
using QuoteFlow.Models.Search.Jql.Parser;
using QuoteFlow.Models.Search.Jql.Util;

namespace QuoteFlow.Models.Search.Jql
{
    public class JqlLexerTester : Lexer
    {
        private LinkedList<AntlrPosition> Stack { get; set; }

        private void StripAndSet()
        {
            string text = Text;
            text = text.Substring(1, text.Length - 1 - 1);
            text = JqlStringSupport.Decode(text);
            Text = text;
        }

        private void CheckAndSet()
        {
            string text = JqlStringSupport.Decode(Text);
            if (JqlStringSupport.IsReservedString(text))
            {
                var message = JqlParseErrorMessages.ReservedWord(text, state.tokenStartLine, state.tokenStartCharPositionInLine);
                throw new RuntimeRecognitionException(message);
            }
            Text = text;
        }

        public override void EmitErrorMessage(string msg)
        {
            base.EmitErrorMessage(msg);
        }

        /// <summary>
        /// START HACK 
        /// We need to get the "lexer" to fail when it detects errors. At the moment ANTLR just drops the input
        /// up until the error and tries again. This can leave antlr actually parsing JQL strings that are not
        /// valid. For example, the string "priority = \kbjb" will actually be parsed as "priority = bjb". 
        /// To stop this we throw a RuntimeRecognitionRuntimeException which the DefaultJqlParser is careful to catch. 
        /// Throwing the RecognitionException  will not work as JqlLexer.nextToken catches this exception and tries 
        /// again (which will cause an infinite loop).
        /// 
        /// Antlr (check up to 3.1.3) does not seem to be able to handle the "catch" clause on lexer rules. It throws a RuntimeException
        /// when trying to process the grammar. To get around this we have hacked the error reporting using a "stack" to push
        /// on the rule we currently have an error in. We can use this information to produce a pretty good error message when
        /// the lexer tries to recover though is does make for some pretty strange logic.
        /// </summary>
        /// <param name="re"></param>
        public override void Recover(RecognitionException re)
        {
            var handler = new LexerErrorHelper(input, PeekPosition());
            handler.HandleError(re);
        }

        private void Recover()
        {
            var e = new MismatchedSetException(null, input);
            Recover(e);
        }

        private void PushPosition(int tokenType)
        {
            Stack.AddLast(new AntlrPosition(tokenType, input));
        }

        private void PopPosition()
        {
            if (Stack.Count != 0) Stack.RemoveFirst();
        }

        private AntlrPosition PeekPosition()
        {
            return Stack.Count == 0 ? null : Stack.ElementAt(0);
        }

        public override void mTokens()
        {
        }
    }
}