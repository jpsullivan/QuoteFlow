using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuoteFlow.Core.Lucene.Index
{
    internal class DelayCloseSearcher : DelegateSearcher, IDelayDisposable
    {
    }
}
