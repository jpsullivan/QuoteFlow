﻿using System.ComponentModel.DataAnnotations;

namespace QuoteFlow.Api.Models.ViewModels.Assets
{
    public class NewAssetCommentViewModel
    {
        [Required]
        public string Comment { get; set; }

        public int AssetId { get; set; }
        public string AssetName { get; set; }
    }
}