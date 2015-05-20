using System.Collections;
using System.Collections.Generic;
using System.Text;
using QuoteFlow.Api.Asset.Fields.Option;
using QuoteFlow.Api.Asset.Index;
using QuoteFlow.Api.Asset.Search;
using QuoteFlow.Api.Asset.Search.Constants;
using QuoteFlow.Api.Asset.Search.Searchers.Renderer;
using QuoteFlow.Api.Asset.Transport;
using QuoteFlow.Api.Jql.Query;
using QuoteFlow.Api.Models;

namespace QuoteFlow.Core.Asset.Search.Searchers.Renderer
{
    public class ManufacturerSearchRenderer : AbstractSearchRenderer, ISearchRenderer
    {
        public ManufacturerSearchRenderer(SimpleFieldSearchConstants searchConstants, string searcherNameKey) 
            : base(searchConstants, searcherNameKey)
        {
            
        }

        /// <summary>
        /// Construct edit HTML parameters and add them to a template parameters map.
        /// </summary>
        /// <param name="user">The user performing the search</param>
        /// <param name="searchContext">The search context</param>
        /// <param name="fieldValuesHolder">Contains the values the user has selected</param>
        /// <param name="displayParameters"></param>
        /// <returns></returns>
        public override string GetEditHtml(User user, ISearchContext searchContext, IFieldValuesHolder fieldValuesHolder, IDictionary displayParameters)
        {
            object selectedOptions;
            if (fieldValuesHolder.TryGetValue(DocumentConstants.ManufacturerId, out selectedOptions))
            {
                displayParameters.Add("selectedOptions", selectedOptions);

                var validOptions = GetVisibleOptions(user, searchContext);
                displayParameters.Add("optionCSSClasses", GetOptionCssClasses(validOptions));

                var invalidOptions = GetInvalidOptions(user, selectedOptions, validOptions);
                SearchContextRenderHelper.AddSearchContextParams(searchContext, displayParameters);
                
            }

            throw new System.NotImplementedException();
        }

        public override bool IsShown(User user, ISearchContext searchContext)
        {
            throw new System.NotImplementedException();
        }

        public override string GetViewHtml(User user, ISearchContext searchContext, IFieldValuesHolder fieldValuesHolder, IDictionary displayParameters)
        {
            throw new System.NotImplementedException();
        }

        public override bool IsRelevantForQuery(User user, IQuery query)
        {
            throw new System.NotImplementedException();
        }

        /// <param name="i18nHelper"> An i18n helper. </param>
        /// <returns> All possible options (not just those that are visible). </returns>
        private ICollection<IOption> getAllOptions(I18nHelper i18nHelper)
        {
            var allOptions = new List<IOption>();
//            allOptions.Add(new TextOption(ConstantsManager.ALL_STANDARD_ISSUE_TYPES, i18nHelper.getText("common.filters.allstandardissuetypes")));
//            allOptions.Add(new TextOption(ConstantsManager.ALL_SUB_TASK_ISSUE_TYPES, i18nHelper.getText("common.filters.allsubtaskissuetypes")));

            allOptions.AddRange(ConstantsManager.AllIssueTypeObjects);

            return allOptions;
        }

        /// <param name="user"> The user performing the search. </param>
        /// <param name="selectedOptions"> Options the user has selected. </param>
        /// <param name="validOptions"> All options that may be selected. </param>
        /// <returns> Invalid options that the user has selected. </returns>
        private ICollection<IOption> GetInvalidOptions(User user, ICollection selectedOptions, ICollection<IOption> validOptions)
        {
            ICollection<IOption> invalidOptions = new List<IOption>();

            if (selectedOptions != null)
            {
                foreach (IOption option in getAllOptions(getI18n(user)))
                {
                    if (!validOptions.Contains(option) && selectedOptions.Contains(option.Id))
                    {
                        invalidOptions.Add(option);
                    }
                }
            }

            return invalidOptions;
        }

        /// <summary>
        /// Constructs a mapping of issue type options to their CSS classes.
        /// </summary>
        /// <param name="options"> The issue type options. </param>
        /// <returns> A map of options to CSS classes. </returns>
        private IDictionary<IOption, string> GetOptionCssClasses(IEnumerable<IOption> options)
        {
            var optionClassesMap = new Dictionary<IOption, string>();
            foreach (IOption option in options)
            {
//                ICollection<FieldConfigScheme> fieldConfigSchemes = issueTypeSchemeManager.getAllRelatedSchemes(option.Id);

                var cssClasses = new StringBuilder();
//                foreach (FieldConfigScheme fieldConfigScheme in fieldConfigSchemes)
//                {
//                    FieldConfig fieldConfig = fieldConfigScheme.OneAndOnlyConfig;
//                    cssClasses.Append(fieldConfig.Id).Append(" ");
//                }

                optionClassesMap[option] = cssClasses.ToString();
            }

            return optionClassesMap;
        }

        /// <param name="searchContext"> The search context. </param>
        /// <param name="user"> The user performing the search. </param>
        /// <returns> All issue type options visible in the given search context. </returns>
        protected virtual ICollection<IOption> GetOptionsInSearchContext(ISearchContext searchContext, User user)
        {
            ICollection<IOption> options = new HashSet<IOption>();
            ICollection<FieldConfig> fieldConfigs = GetCatalogFieldConfigs(GetCatalogsInSearchContext(searchContext, user));

            foreach (FieldConfig fieldConfig in fieldConfigs)
            {
                OptionSet optionSet = optionSetManager.getOptionsForConfig(fieldConfig);
                options.addAll(optionSet.Options);
            }

            return options;
        }

        private IEnumerable<IOption> GetVisibleOptions(User user, ISearchContext searchContext)
        {
            var options = new List<IOption>();
            var optionsInSearchContext = GetOptionsInSearchContext(searchContext, user);

            // Add the "All Standard Manufacturers" option and the standard types.
            //options.Add(new TextOption(Constants.ALL_STANDARD_ASSET_TYPES, "All Standard Asset Types"));
            options.AddRange(optionsInSearchContext);

            return options;
        }
    }
}