using System;
using System.Collections.Generic;
using QuoteFlow.Api.Models;
using QuoteFlow.Api.Util;
using QuoteFlow.Core.Asset.Editor;

namespace QuoteFlow.Core.Asset.Viewer
{
    /// <summary>
    /// Contains all the information needed to render the view asset 
    /// page with inline edit, including: fields, asset details, pager information,
    /// and web panels.
    /// </summary>
    public class AssetFields : EditFields
    {
        public IAsset Asset { get; set; }
        public AssetPager Pager { get; set; }
        public long ReadTime { get; set; }

        public AssetFields()
        {
        }

        public AssetFields(string qfToken, IErrorCollection errorCollection)
            : base(qfToken, errorCollection)
        {
        }

        public AssetFields(
                string xsrfToken,
                IErrorCollection errorCollection,
                List<string> fields,
                IAsset asset,
                AssetPager pager) : base(fields, xsrfToken, errorCollection)
        {
            Asset = asset;
            Pager = pager;
        }
    }
}