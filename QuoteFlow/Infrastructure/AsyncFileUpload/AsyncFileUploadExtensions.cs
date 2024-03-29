﻿using System;
using QuoteFlow.Api.Services;

namespace QuoteFlow.Infrastructure.AsyncFileUpload
{
    public static class AsyncFileUploadExtensions
    {
        public static void SetProgress(this ICacheService cacheService, string username, AsyncFileUploadProgress progress)
        {
            string cacheKey = GetUpdateCacheKey(username);
            cacheService.SetItem(cacheKey, progress, TimeSpan.FromHours(1));
        }

        public static AsyncFileUploadProgress GetProgress(this ICacheService cacheService, string username)
        {
            string cacheKey = GetUpdateCacheKey(username);
            return cacheService.GetItem(cacheKey) as AsyncFileUploadProgress;
        }

        public static void RemoveProgress(this ICacheService cacheService, string username)
        {
            string cacheKey = GetUpdateCacheKey(username);
            cacheService.RemoveItem(cacheKey);
        }

        private static string GetUpdateCacheKey(string username)
        {
            return "upload-" + username;
        }
    }
}