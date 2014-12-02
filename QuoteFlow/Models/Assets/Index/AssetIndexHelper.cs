using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using QuoteFlow.Services.Interfaces;

namespace QuoteFlow.Models.Assets.Index
{
    public class AssetIndexHelper
    {
        private readonly IAssetService issueManager;
		private readonly IAssetIndexer issueIndexer;
		private readonly IAssetFactory issueFactory;

        public AssetIndexHelper(IAssetService issueManager, IAssetIndexer issueIndexer, IAssetFactory issueFactory)
		{
			this.issueManager = issueManager;
			this.issueIndexer = issueIndexer;
			this.issueFactory = issueFactory;
		}

    }
}