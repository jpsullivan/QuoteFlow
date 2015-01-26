using System;
using System.Collections.Generic;
using QuoteFlow.Api.Asset;
using QuoteFlow.Api.Jql.Resolver;

namespace QuoteFlow.Core.Jql.Resolver
{
    /// <summary>
    /// Resolves index info with a lucene field using the id of the domain object T to get the
    /// indexed values from a <see cref="INameResolver{T}"/>
    /// </summary>
    public class AssetConstantInfoResolver<T> : IIndexInfoResolver<T> where T : IAssetConstant
    {
        private readonly INameResolver<T> resolver;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="resolver">The name resolver to look up the id if necessary.</param>
        public AssetConstantInfoResolver(INameResolver<T> resolver)
        {
            if (resolver == null)
            {
                throw new ArgumentNullException("resolver");
            }

            this.resolver = resolver;
        }

        private static int? GetValueAsInt(string singleValueOperand)
        {
            try
            {
                return Convert.ToInt32(singleValueOperand);
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public List<string> GetIndexedValues(string rawValue)
        {
            if (rawValue == null)
            {
                throw new ArgumentNullException("rawValue");
            }

            // our id is our index value
            var ids = new List<string>();

            IList<string> idsForName = resolver.GetIdsFromName(rawValue);
            ids.AddRange(idsForName);

            // search by id
            int? valueAsLong = GetValueAsInt(rawValue);
            if (valueAsLong != null && resolver.IdExists((int) valueAsLong))
            {
                ids.Add(rawValue);
            }

            return ids;
        }

        public List<string> GetIndexedValues(int? rawValue)
        {
            if (rawValue == null)
            {
                throw new ArgumentNullException("rawValue");
            }

            if (resolver.IdExists((int) rawValue))
            {
                return new List<string> { rawValue.ToString() };
            }
            return resolver.GetIdsFromName(rawValue.ToString());
        }

        public string GetIndexedValue(T indexedObject)
        {
            if (indexedObject == null)
            {
                throw new ArgumentNullException("indexedObject");
            }

            return indexedObject.Id;
        }
    }
}