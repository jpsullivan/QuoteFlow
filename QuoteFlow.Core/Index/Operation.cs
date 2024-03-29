﻿using QuoteFlow.Api.Lucene.Index;
using QuoteFlow.Core.Lucene.Index;

namespace QuoteFlow.Core.Index
{
    /// <summary>
    /// An operation that is performed on an Index. See <see cref="Operation"/> for
    /// factory methods.
    /// Note: this is not an interface to prevent clients implementing it. All
    /// clients need to now is that they have an <see cref="Operations"/> that will do a
    /// create/delete/update/whatever, not how it is implemented.
    /// </summary>
    public abstract class Operation
    {
        public Operation()
        {
        }

        public abstract void Perform(IWriter writer);

        public abstract UpdateMode Mode();
    }
}