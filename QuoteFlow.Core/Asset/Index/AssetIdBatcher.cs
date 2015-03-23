using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using QuoteFlow.Api.Asset;
using QuoteFlow.Api.Models;

namespace QuoteFlow.Core.Asset.Index
{
    public class AssetIdBatcher : IAssetsBatcher
    {
        public IAssetFactory AssetFactory { get; protected set; }
        public int BatchSize { get; protected set; }
        public ISpy Spy { get; protected set; }
        
        private readonly List<string> _orderBy = new List<string> { "id DESC" };
        
        /// <summary>
        /// Contains the max "id" used to get the next batch.
        /// </summary>
        private int _maxIdNextBatch;

        public AssetIdBatcher(IAssetFactory assetFactory, int batchSize, ISpy spy)
        {
            AssetFactory = assetFactory;
            BatchSize = batchSize;
            Spy = spy;
            _maxIdNextBatch = SelectMaxId();
        }

        public IEnumerator<IEnumerable<Api.Models.Asset>> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// SELECTs the max asset id from the Asset table.
        /// </summary>
        /// <returns>The asset id of the newest asset, or -1 if there are no assets.</returns>
        private int SelectMaxId()
        {
            var maxId = Current.DB.Query<int>("SELECT MAX(Id) FROM Assets").First();
            if (maxId == 0)
            {
                return -1;
            }

            return maxId;
        }

        // Allows us to watch the assets as they get iterated over
        public interface ISpy
        {
            void Spy(IAsset next);
        }
    }
}