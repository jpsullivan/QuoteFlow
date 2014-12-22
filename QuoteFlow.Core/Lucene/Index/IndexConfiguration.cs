using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lucene.Net.Analysis;
using QuoteFlow.Api.Lucene.Index;

namespace QuoteFlow.Core.Lucene.Index
{
    public class IndexConfiguration : IIndexConfiguration
    {
        public Directory Directory
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public Analyzer Analyzer
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }
    }
}
