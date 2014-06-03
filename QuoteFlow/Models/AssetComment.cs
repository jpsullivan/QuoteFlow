using System;

namespace QuoteFlow.Models
{
    public class AssetComment
    {
        public AssetComment() { }

        public AssetComment(string comment, int assetId, int userId)
        {
            Comment = comment;
            CreatorId = userId;
            AssetId = assetId;
            CreatedUtc = DateTime.UtcNow;
        }

        public int Id { get; set; }
        public string Comment { get; set; }
        public int CreatorId { get; set; }
        public int AssetId { get; set; }
        public DateTime CreatedUtc { get; set; }
    }
}