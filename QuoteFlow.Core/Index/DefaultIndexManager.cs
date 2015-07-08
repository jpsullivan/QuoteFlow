using System;
using System.IO;
using Lucene.Net.Index;
using Lucene.Net.Search;
using QuoteFlow.Core.Lucene.Index;

namespace QuoteFlow.Core.Index
{
    public class DefaultIndexManager : IIndexManager
    {
        private readonly IIndexConfiguration _configuration;
        private readonly IEngine _actor;
        private readonly IDisposableIndex _index;

        public DefaultIndexManager(IIndexConfiguration configuration, IEngine engine, IDisposableIndex index)
        {
            if (configuration == null) throw new ArgumentNullException("configuration");
            if (engine == null) throw new ArgumentNullException("engine");
            if (index == null) throw new ArgumentNullException("index");

            _configuration = configuration;
            _actor = engine;
            _index = index;
        }

        public IIndex Index
        {
            get { return _index; }
        }

        public IndexSearcher OpenSearcher()
        {
            return _actor.GetSearcher();
        }

        public bool IndexCreated
        {
            get
            {
                try
                {
                    return IndexReader.IndexExists(_configuration.Directory);
                }
                catch (IOException ex)
                {
                    throw ex;
                }
            }
        }

        public void DeleteIndexDirectory()
        {
            _actor.Clean();
        }

        public void Dispose()
        {
            _index.Dispose();
        }
    }
}