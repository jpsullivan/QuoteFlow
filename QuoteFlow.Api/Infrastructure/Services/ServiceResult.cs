﻿using System;
using QuoteFlow.Api.Util;

namespace QuoteFlow.Api.Infrastructure.Services
{
    /// <summary>
    /// Simple implementation of a validation result.
    /// </summary>
    public class ServiceResult : IServiceResult
    {
        protected ServiceResult(IErrorCollection errorCollection)
        {
            if (errorCollection == null)
            {
                throw new ArgumentNullException(nameof(errorCollection));
            }

            ErrorCollection = errorCollection;
        }

        public bool IsValid()
        {
            return !ErrorCollection.HasAnyErrors();
        }

        public IErrorCollection ErrorCollection { get; private set; }
    }
}