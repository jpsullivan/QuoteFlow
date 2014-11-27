using System;
using System.Collections.Generic;
using QuoteFlow.Models.Assets.Search;

namespace QuoteFlow.Models.Assets.Statistics
{
    public class TextFieldSorter : ILuceneFieldSorter<string>
    {
        public string DocumentConstant { get; private set; }

        private static readonly IComparer<string> StringComparer = new ComparatorHelper();
        private sealed class ComparatorHelper : IComparer<string>
        {
            public int Compare(string o1, string o2)
            {
                return String.Compare(o1, o2, StringComparison.Ordinal);
            }
        }

        public TextFieldSorter(string documentConstant)
        {
            DocumentConstant = documentConstant;
        }

        
        public string GetValueFromLuceneField(string documentValue)
        {
            return documentValue;
        }

        public IComparer<string> Comparator { get { return StringComparer; } }
    }
}