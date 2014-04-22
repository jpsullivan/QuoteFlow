﻿using System.IO;
using QuoteFlow.Services.Interfaces;

namespace QuoteFlow.Services
{
    public class CloudFileReference : IFileReference
    {
        private Stream _stream;
        private string _contentId;

        public string ContentId
        {
            get { return _contentId; }
        }

        private CloudFileReference(Stream stream, string contentId)
        {
            _contentId = contentId;
            _stream = stream;
        }

        public Stream OpenRead()
        {
            return _stream;
        }

        public static CloudFileReference NotModified(string contentId)
        {
            return new CloudFileReference(null, contentId);
        }

        public static CloudFileReference Modified(ISimpleCloudBlob blob, Stream data)
        {
            return new CloudFileReference(data, blob.ETag);
        }
    }
}
