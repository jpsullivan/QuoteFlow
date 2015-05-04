using System.Collections;
using System.Collections.Generic;

namespace QuoteFlow.Api.Asset.Transport
{
    /// <summary>
    /// This is a field params with Lists as the value
    /// </summary>
    public interface ICollectionParams : IFieldParams
    {
        /// <summary>
        /// Return all values of all keys, flattened into a single collection.
        /// Use <see cref="#getValuesForNullKey()"/> instead if, for example, you just need the values of the custom field.
        /// </summary>
        ICollection AllValues { get; }

        /// <summary>
        /// Return the values of the custom field.
        /// <p/>
        /// The values associated with the null key represents the values of the custom field.
        /// For example, the user selected in a single user picker, or the list of users selected in a multiple user picker.
        /// <p/>
        /// Note that unlike <see cref="#getAllValues()"/>, this method does not return values associated with other non-null keys.
        /// </summary>
        ICollection ValuesForNullKey { get; }

        /// <summary>
        /// Return the values associated with the given {@code key} in the parameters.
        /// <p/>
        /// Depending on the type of field, additional keys might be introduced in addition to the null key.
        /// JIRA might also add additional keys into the parameters.
        /// For example, issue id and project id might be passed into the parameters under separate keys during custom field validation.
        /// </summary>
        ICollection<string> GetValuesForKey(string key);

        /// <summary>
        /// Put the values in.
        /// </summary>
        /// <param name="key"> for mapping </param>
        /// <param name="value"> a Collection of Strings. </param>
        void Add(string key, ICollection<string> value);
    }
}