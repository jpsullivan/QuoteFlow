using System;
using System.Collections;
using System.Collections.Generic;

namespace QuoteFlow.Api.Asset.Fields
{
    /// <summary>
    /// Interface for fields in QuoteFlow which are able to be placed on "screens" - once they are on the screen they have an "order".
    /// More generally, <see cref="OrderableField"/>s can be viewed and edited.
    /// </summary>
    public interface IOrderableField : IField, ISearchableField
    {
        /// <summary>
        /// Populate the fieldValueHolder with a value that should be shown by default when the issue
        /// has not been created yet.
        /// </summary>
        /// <param name="fieldValuesHolder">The fieldValuesHolder Map to be populated.</param>
        /// <param name="asset">The asset.</param>
        void PopulateDefaults(IDictionary<string, object> fieldValuesHolder, Models.Asset asset);

        /// <summary>
        /// Checks to see if the (web) parameters contains a relevant value with which to populate the issue
        /// </summary>
        /// <param name="parameters">Map of HTTP request parameters ("Action parameters").</param>
        bool HasParam(IDictionary<string, String[]> parameters);

        /// <summary>
        /// Populate the fieldValuesHolder with a value from (web) parameters.
        /// </summary>
        /// <param name="fieldValuesHolder">The fieldValuesHolder Map to be populated.</param>
        /// <param name="parameters">Map of HTTP parameters.</param>
        void PopulateFromParams(IDictionary<string, object> fieldValuesHolder, IDictionary<string, String[]> parameters);

        /// <summary>
        /// Used to initialise the fieldValuesHolder from the current value of the asset. Used, for example, when
        /// showing the Edit Asset screen to show the issue's current values.
        /// </summary>
        /// <param name="fieldValuesHolder">The fieldValuesHolder Map to be populated.</param>
        /// <param name="asset">The asset.</param>
        void PopulateFromAsset(IDictionary<string, object> fieldValuesHolder, Models.Asset asset);

        /// <summary>
        /// Determines if the field has a value for the given issue.
        /// </summary>
        /// <param name="asset">The asset.</param>
        /// <returns>True if the given Asset has a value for this field.</returns>
        bool HasValue(Models.Asset asset);

        /// <summary>
        /// Get a field value from the map of parameters passed.
        /// The params map may contain other parameters that are not relevant to this custom field.
        /// </summary>
        /// <param name="fieldParams">The map of field parameters.</param>
        /// <returns> Value for this field from the map of parameters. </returns>
        /// <exception cref="FieldValidationException"> if there is a problem with Field Validation. </exception>
        object GetValueFromParams(IDictionary fieldParams);

        /// <summary>
        /// Used to convert from a user friendly string value and put the result into the fieldValuesHolder.
        /// This method is useful for places like Jelly where the field value can be a name (e.g. issue type name) and not a
        /// regular id that is used in the web pages.
        /// </summary>
        /// <param name="fieldValuesHolder">Map of field Values.</param>
        /// <param name="stringValue">User friendly string value.</param>
        /// <param name="asset">the asset.</param>
        /// <exception cref="FieldValidationException">If cannot convert to a value from the given string.</exception>
        void PopulateParamsFromString(IDictionary<string, object> fieldValuesHolder, string stringValue, Models.Asset asset);
    }
}