﻿using System;
using System.Web.Mvc;

namespace QuoteFlow.Api.Infrastructure.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public sealed class HintAttribute : Attribute, IMetadataAware
    {
        private readonly object _typeId = new Object();

        public HintAttribute(string hint)
        {
            Hint = hint;
        }

        public string Hint { get; private set; }

        public override object TypeId
        {
            get { return _typeId; }
        }

        public void OnMetadataCreated(ModelMetadata metadata)
        {
            if (metadata == null)
            {
                throw new ArgumentNullException(nameof(metadata));
            }
            metadata.AdditionalValues["Hint"] = Hint;
        }
    }
}