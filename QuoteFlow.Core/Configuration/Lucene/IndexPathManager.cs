using System;
using System.IO;
using System.Web.Hosting;
using Lucene.Net.Store;
using QuoteFlow.Api.Configuration;
using QuoteFlow.Api.Configuration.Lucene;
using Directory = Lucene.Net.Store.Directory;
using Version = Lucene.Net.Util.Version;

namespace QuoteFlow.Core.Configuration.Lucene
{
    public class IndexPathManager : IIndexPathManager
    {
        public static readonly Version LuceneVersion = Version.LUCENE_30;

        private static SingleInstanceLockFactory LuceneLock = new SingleInstanceLockFactory();

        // Factory method for DI/IOC that creates the directory the index is stored in.
        // Used by real website. Bypassed for unit tests.
        private static SimpleFSDirectory _directorySingleton;

        public static Directory GetDirectory(LuceneIndexLocation location)
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

        private static string GetIndexLocation(LuceneIndexLocation location)
        {
            switch (location)
            {
                case LuceneIndexLocation.Temp:
                    return Path.Combine(Path.GetTempPath(), "QuoteFlow", "Lucene");
                default:
                    return HostingEnvironment.MapPath("~/App_Data/Lucene");
            }
        }

        public string IndexRootPath
        {
            get { return GetIndexLocation(LuceneIndexLocation.AppData); }
            set { throw new NotImplementedException(); }
        }

        public string DefaultIndexRootPath
        {
            get
            {
                // Don't create the directory if it's not already present.
                return _directorySingleton?.Directory.FullName;
            }
        }

        public string AssetIndexPath
        {
            get
            {
                // Don't create the directory if it's not already present.
                string root = _directorySingleton == null ? "." : (_directorySingleton.Directory.FullName ?? ".");
                return Path.Combine(root, "assets");
            }
        }

        public string CommentIndexPath
        {
            get
            {
                // Don't create the directory if it's not already present.
                string root = _directorySingleton == null ? "." : (_directorySingleton.Directory.FullName ?? ".");
                return Path.Combine(root, "comments");
            }
        }

        public IndexPathManagerMode Mode => IndexPathManagerMode.Default;
    }
}
