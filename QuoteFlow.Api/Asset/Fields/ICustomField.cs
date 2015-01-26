using System;
using System.Collections.Generic;
using QuoteFlow.Api.Asset.Search;
using QuoteFlow.Api.Models;
using Wintellect.PowerCollections;

namespace QuoteFlow.Api.Asset.Fields
{
    /// <summary>
    /// Custom Field interface.
    /// <p/>
    /// Typically one obtains a CustomField using <seealso cref="CustomFieldManager"/>,
    /// eg. <see cref="CustomFieldManager.GetCustomFieldObjectByName(String)"/>
    /// <p/>
    /// To create or update an instance of a CustomField for an issue use <seealso cref="CreateValue(Asset,Object)"/>
    /// or <see cref="OrderableField.UpdateValue(FieldLayoutItem,ModifiedValue,AssetChangeHolder)"/>.
    /// </summary>
    public interface ICustomField : INavigableField, IOrderableField
    {
        /// <summary>
        /// Determines if this custom field is within the scope of the given project, and list of Issue Types.
        /// </summary>
        /// <param name="catalog">The project.</param>
        /// <param name="assetTypeIds">A list of AssetType ids.</param>
        /// <returns> {@code true} if this custom field is within the given scope. </returns>
        bool IsInScope(Catalog catalog, IEnumerable<string> assetTypeIds);

        /// <summary>
        /// Determines whether this custom field is in scope.
        /// </summary>
        /// <param name="searchContext"> search context </param>
        /// <returns> true if this field is in scope </returns>
        bool IsInScope(ISearchContext searchContext);

        /// <summary>
        /// Determines if this custom field is within the scope of the given project, and list of Issue Types.
        /// 
        /// If the project is null, then it is treated as <tt>any project</tt>.
        /// If the issueTypeIds list is null or an empty list, then it is treated as <tt>any issue type</tt>.
        /// 
        /// If the passed project is <tt>any project</tt>, this method will search in all the <seealso cref="FieldConfigScheme"/>
        /// of this custom field, ignoring the projects that they apply to (since the given project is <tt>any</tt>)
        /// and looking for at least one of them that applies to at least one of the given issue type ids.
        /// 
        /// If the passed list of issue types is <tt>any issue type</tt>, this method will search for at least one <seealso cref="FieldConfigScheme"/>
        /// that applies to the given project, ignoring the issue types that it applies to (since the given issue type ids are <tt>any</tt>).
        /// 
        /// If both the project and issue types are <tt>any</tt>, the question being asked is "is this custom field
        /// in the scope of any project and any issue type?", which will always be true.
        /// </summary>
        bool IsInScopeForSearch(Catalog catalog, IEnumerable<string> issueTypeIds);

        /// <summary>
        /// This method compares the values of this custom field in two given issues.
        /// Returns a negative integer, zero, or a positive integer as the value of first issue is less than, equal to,
        /// or greater than the value of the second issue.
        /// If either of given issues is null a IllegalArgumentException is thrown.
        /// </summary>
        /// <param name="issue1"> issue to compare </param>
        /// <param name="issue2"> issue to compare </param>
        /// <returns>
        /// A negative integer, zero, or a positive integer as the value of first issue is less than, 
        /// equal to, or greater than the value of the second issue 
        /// </returns>
        int Compare(Models.Asset asset1, Models.Asset asset2);

//        /// <summary>
//        /// Get the custom field string values that are relevant to this particular custom field
//        /// </summary>
//        /// <param name="customFieldValuesHolder"> containing all params </param>
//        /// <returns> a <seealso cref="CustomFieldParams"/> of <seealso cref="String"/> objects </returns>
//        ICustomFieldParams GetCustomFieldValues(IDictionary customFieldValuesHolder);

        /// <summary>
        /// Retrieves and returns the Object representing the this CustomField value for the given issue.
        /// See <seealso cref="CustomFieldType#getValueFromIssue(CustomField,Issue)"/>.
        /// This is only used to communicate with the 'view' JSP. Multiselects will return a list, dates a date, etc.
        /// </summary>
        /// <param name="issue"> issue to retrieve the value from </param>
        /// <returns> Object representing the this CustomField value for the given issue </returns>
        /// <seealso cref= #getValueFromParams(java.util.Map) </seealso>
        object GetValue(Models.Asset issue);

        /// <summary>
        /// Removes this custom field and returns a set of issue IDs of all issues that are affected by removal of this
        /// custom field.
        /// </summary>
        /// <returns> a set of issue IDs of affected issues </returns>
        Set<int?> Remove();

//        /// <summary>
//        /// Returns options for this custom field if it is
//        /// of <seealso cref="com.atlassian.jira.issue.customfields.MultipleCustomFieldType"/> type. Otherwise returns null.
//        /// <p/>
//        /// As this is just used by the view layer, it can be a list of objects
//        /// </summary>
//        /// <param name="key">             not used </param>
//        /// <param name="jiraContextNode"> JIRA context node </param>
//        /// <returns> options for this custom field if it is of <seealso cref="com.atlassian.jira.issue.customfields.MultipleCustomFieldType"/> type, null otherwise </returns>
//        Options getOptions(string key, JiraContextNode jiraContextNode);

        /// <summary>
        /// Sets the name of this custom field.
        /// </summary>
        /// <param name="name"> name to set </param>
        string Name { set; get; }

        /// <summary>
        /// Returns the 1i8n'ed description of this custom field. To render views for the custom field description, prefer
        /// <seealso cref="#getDescriptionProperty()"/>.
        /// </summary>
        /// <returns> the description of this custom field </returns>
        string Description { get; set; }

        /// <summary>
        /// Returns the description of this custom field by reading <seealso cref="#ENTITY_DESCRIPTION"/> of the underlying generic value.
        /// </summary>
        /// <returns> the description of this custom field </returns>
        string UntranslatedDescription { get; }

        /// <summary>
        /// Returns the title of this custom field.
        /// </summary>
        /// <returns> the title of this custom field </returns>
        string FieldName { get; }

        /// <summary>
        /// Returns the name of this custom field by reading <seealso cref="#ENTITY_NAME"/> of the underlying generic value.
        /// </summary>
        /// <returns> the name of this custom field </returns>
        string UntranslatedName { get; }

        /// <summary>
        /// Returns true if all configuration schemes returned by <seealso cref="#getConfigurationSchemes()"/> are enabled.
        /// </summary>
        /// <returns> true if all configuration schemes are enabled, false otherwise </returns>
        bool Enabled { get; }

//        /// <summary>
//        /// Looks up the <seealso cref="ICustomFieldType"/>. It can return null if the custom
//        /// field type cannot be found in the <seealso cref="CustomFieldManager"/>.
//        /// </summary>
//        /// <returns> custom field type </returns>
//        ICustomFieldType CustomFieldType { get; }
    }
}