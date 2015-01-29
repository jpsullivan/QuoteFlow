﻿using System;
using System.Collections.Generic;
using Lucene.Net.Search;
using QuoteFlow.Api.Asset.Index;
using QuoteFlow.Api.Lucene.Index;
using QuoteFlow.Api.Models;
using WebBackgrounder;

namespace QuoteFlow.Core.Asset.Index
{
    public class AssetIndexer : IAssetIndexer
    {
        public IIndexResult IndexAssets(IEnumerable<IAsset> assets, Job context)
        {
            throw new NotImplementedException();
        }

        public IIndexResult DeIndexAssets(IEnumerable<IAsset> assets, Job context)
        {
            throw new NotImplementedException();
        }

        public IIndexResult ReIndexAssets(IEnumerable<IAsset> assets, Job context, bool reIndexComments, bool reIndexChangeHistory, bool conditionalUpdate)
        {
            throw new NotImplementedException();
        }

        public IIndexResult ReIndexComments(ICollection<AssetComment> comments, Job context)
        {
            throw new NotImplementedException();
        }

        public IIndexResult IndexAssetsBatchMode(IEnumerable<IAsset> assets, Job context)
        {
            throw new NotImplementedException();
        }

        public IIndexResult Optimize()
        {
            throw new NotImplementedException();
        }

        public void DeleteIndexes()
        {
            throw new NotImplementedException();
        }

        public void Shutdown()
        {
            throw new NotImplementedException();
        }

        public IndexSearcher OpenAssetSearcher()
        {
            throw new NotImplementedException();
        }

        public IndexSearcher OpenCommentSearcher()
        {
            throw new NotImplementedException();
        }

        public IndexSearcher OpenChangeHistorySearcher()
        {
            throw new NotImplementedException();
        }

        public IList<string> IndexPaths { get; private set; }
        public string IndexRootPath { get; private set; }
    }
}