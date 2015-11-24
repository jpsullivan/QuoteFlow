using System;
using System.Collections.Generic;
using QuoteFlow.Api.Asset.Fields;

namespace QuoteFlow.Api.OrderBy
{
    public class NameComparer : IComparer<IField>
    {
        public int Compare(IField x, IField y)
        {
            if (x == null)
            {
                throw new ArgumentException("The first parameter is null");
            }

            if (y == null)
            {
                throw new ArgumentException("The second parameter is null");
            }

            string name1 = x.NameKey;
            string name2 = y.NameKey;
            return string.Compare(name1, name2, StringComparison.Ordinal);
        }
    }

}