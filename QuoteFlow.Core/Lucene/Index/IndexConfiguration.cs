using System;
using System.IO;
using System.Web.Hosting;
using Lucene.Net.Analysis;
using Lucene.Net.Store;
using QuoteFlow.Api.Lucene.Index;
using Directory = Lucene.Net.Store.Directory;
using Version = Lucene.Net.Util.Version;

namespace QuoteFlow.Core.Lucene.Index
{
    public class IndexConfiguration : IIndexConfiguration
    {
        internal static readonly Version LuceneVersion = Version.LUCENE_30;

        private static readonly SingleInstanceLockFactory LuceneLock = new SingleInstanceLockFactory();

        // Factory method for DI/IOC that creates the directory the index is stored in.
        // Used by real website. Bypassed for unit tests.
        private static SimpleFSDirectory _directorySingleton;

        internal static string GetDirectoryLocation()
        {
            // Don't create the directory if it's not already present.
            return _directorySingleton == null ? null : _directorySingleton.Directory.FullName;
        }

        internal static string GetIndexMetadataPath()
        {
            // Don't create the directory if it's not already present.
            string root = _directorySingleton == null ? "." : (_directorySingleton.Directory.FullName ?? ".");
            return Path.Combine(root, "index.metadata");
        }

        internal static Directory GetDirectory(IndexLocation location)
        {
            if (_directorySingleton == null)
            {
                var index = GetIndexLocation(location);
                if (!System.IO.Directory.Exists(index))
                {
                    System.IO.Directory.CreateDirectory(index);
                }

                var directoryInfo = new DirectoryInfo(index);
                _directorySingleton = new SimpleFSDirectory(directoryInfo, LuceneLock);
            }

            return _directorySingleton;
        }

        private static string GetIndexLocation(IndexLocation location)
        {
            switch (location)
            {
                case IndexLocation.Temp:
                    return Path.Combine(Path.GetTempPath(), "QuoteFlow", "Lucene");
                default:
                    return HostingEnvironment.MapPath("~/App_Data/Lucene");
            }
        }

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
