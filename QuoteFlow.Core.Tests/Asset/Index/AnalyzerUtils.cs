// todo lucene 4.8
//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using Lucene.Net.Analysis;
//using Lucene.Net.Analysis.Tokenattributes;
//using Xunit;
//
//namespace QuoteFlow.Core.Tests.Asset.Index
//{
//    public class AnalyzerUtils
//    {
//        public static SimpleToken[] TokensFromAnalysis(Analyzer analyzer, string text)
//        {
//            TokenStream stream = analyzer.TokenStream("contents", new StringReader(text));
//            CharTermAttribute charTermAttribute = stream.AddAttribute(typeof(CharTermAttribute));
//            PositionIncrementAttribute incrementAttribute = stream.AddAttribute(typeof(PositionIncrementAttribute));
//            TypeAttribute typeAttribute = stream.AddAttribute(typeof(TypeAttribute));
//            OffsetAttribute offsetAttribute = stream.AddAttribute(typeof(OffsetAttribute));
//
//            IList<SimpleToken> tokenList = new List<SimpleToken>();
//            while (stream.IncrementToken())
//            {
//                if (charTermAttribute == null)
//                {
//                    break;
//                }
//
//                tokenList.Add(new SimpleToken(charTermAttribute.ToString(), incrementAttribute.PositionIncrement, typeAttribute.type(), offsetAttribute.startOffset(), offsetAttribute.endOffset()));
//            }
//
//            return tokenList.ToArray();
//        }
//
//        public static void DisplayTokensWithPositions(Analyzer analyzer, string text)
//        {
//            SimpleToken[] tokens = TokensFromAnalysis(analyzer, text);
//
//            int position = 0;
//
//            foreach (SimpleToken token in tokens)
//            {
//                int increment = token.increment;
//
//                if (increment > 0)
//                {
//                    position = position + increment;
//                    Console.WriteLine();
//                    Console.Write(position + ": ");
//                }
//
//                Console.Write("[" + token.term + "] ");
//            }
//            Console.WriteLine();
//        }
//
//        public static void DisplayTokensWithFullDetails(Analyzer analyzer, string text)
//        {
//            SimpleToken[] tokens = TokensFromAnalysis(analyzer, text);
//
//            int position = 0;
//
//            foreach (SimpleToken token in tokens)
//            {
//                int increment = token.increment;
//
//                if (increment > 0)
//                {
//                    position = position + increment;
//                    Console.WriteLine();
//                    Console.Write(position + ": ");
//                }
//
//                Console.Write("[{0}|{1}] ", token.term, token.type);
//            }
//            Console.WriteLine();
//        }
//
//        public static void AssertTokensEqual(Token[] tokens, string[] strings)
//        {
//            Assert.Equal(strings.Length, tokens.Length);
//
//            for (int i = 0; i < tokens.Length; i++)
//            {
//                Assert.Equal(strings[i], tokens[i].Term);
//            }
//        }
//
//        public static void DisplayTokens(Analyzer analyzer, string text)
//        {
//            SimpleToken[] tokens = TokensFromAnalysis(analyzer, text);
//
//            foreach (SimpleToken token in tokens)
//            {
//                Console.Write("[{0}] ", token.term);
//            }
//        }
//
//        private class SimpleToken
//        {
//            internal readonly string term;
//            internal readonly int increment;
//            internal readonly string type;
//            internal readonly int start;
//            internal readonly int end;
//
//            public SimpleToken(string term, int increment, string type, int start, int end)
//            {
//                this.term = term;
//                this.increment = increment;
//                this.type = type;
//                this.start = start;
//                this.end = end;
//            }
//        }
//    }
//
//}