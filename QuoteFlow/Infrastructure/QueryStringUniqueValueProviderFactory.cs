using System.Globalization;
using System.Collections.Generic;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.ValueProviders;
using System.Web.Http.ValueProviders.Providers;

namespace QuoteFlow.Infrastructure
{
    public class QueryStringUniqueValueProviderFactory : QueryStringValueProviderFactory
    {
        private const string RequestLocalStorageKey = "{755EDBD6-46CD-4B44-8162-31D8CF111155}";

        public override IValueProvider GetValueProvider(HttpActionContext actionContext)
        {
            object provider;
            var storage = actionContext.Request.Properties;

            // Only parse the query string once-per request
            if (!storage.TryGetValue(RequestLocalStorageKey, out provider))
            {
                provider = new QueryStringUniqueValueProvider(actionContext, CultureInfo.InvariantCulture);
                storage[RequestLocalStorageKey] = provider;
            }

            return (IValueProvider)provider;
        }
    }


    public class QueryStringUniqueValueProvider : NameValuePairsValueProvider
    {
        public QueryStringUniqueValueProvider(HttpActionContext actionContext, CultureInfo culture)
            : base(GetUniqueQueryNameValuePairs(actionContext), culture)
        {
        }


        private static IEnumerable<KeyValuePair<string, string>> GetUniqueQueryNameValuePairs(HttpActionContext actionContext)
        {
            var pairs = actionContext.ControllerContext.Request.GetQueryNameValuePairs();
            var returnedKeys = new HashSet<string>();
            foreach (var pair in pairs)
            {
                if (returnedKeys.Contains(pair.Key))
                    continue;
                returnedKeys.Add(pair.Key);
                yield return pair;
            }
        }
    }
}