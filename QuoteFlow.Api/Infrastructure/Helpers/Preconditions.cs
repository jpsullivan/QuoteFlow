using System;

namespace QuoteFlow.Api.Infrastructure.Helpers
{
    public static class Preconditions
    {
        public static T CheckNotNull<T>(T value) where T : class
        {
            if (value == null)
            {
                throw new ArgumentNullException();
            }
            return value;
        }
    }
}