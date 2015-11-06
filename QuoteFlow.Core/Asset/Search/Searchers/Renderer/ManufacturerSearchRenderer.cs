using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QuoteFlow.Api.Asset.Fields.Option;
using QuoteFlow.Api.Asset.Index;
using QuoteFlow.Api.Asset.Search;
using QuoteFlow.Api.Asset.Search.Constants;
using QuoteFlow.Api.Asset.Search.Searchers.Renderer;
using QuoteFlow.Api.Asset.Transport;
using QuoteFlow.Api.Jql.Query;
using QuoteFlow.Api.Models;
using QuoteFlow.Api.Services;
using QuoteFlow.Core.Asset.Fields.Option;
using QuoteFlow.Core.Asset.Search.Searchers.Util;

namespace QuoteFlow.Core.Asset.Search.Searchers.Renderer
{
    public class ManufacturerSearchRenderer : AbstractSearchRenderer, ISearchRenderer
    {
        public ICatalogService CatalogService { get; protected set; }
        public IManufacturerService ManufacturerService { get; protected set; }

        public ManufacturerSearchRenderer(ICatalogService catalogService, IManufacturerService manufacturerService,
            SimpleFieldSearchConstants searchConstants, string searcherNameKey) 
            : base(searchConstants, searcherNameKey)
        {
            CatalogService = catalogService;
            ManufacturerService = manufacturerService;
        }

        public void AddEditParameters(IFieldValuesHolder fieldValuesHolder, ISearchContext searchContext, User user, IDictionary<string, object> templateParameters)
        {
            var options = new List<string>();
            object selectedOptions;
            if (fieldValuesHolder.TryGetValue(DocumentConstants.ManufacturerId, out selectedOptions))
            {
                // check if this is a HashSet and crudely cast it to a list
                var set = selectedOptions as HashSet<string>;
                if (set != null)
                {
                    var casted = set;
                    options = casted.ToList();
                }
                else
                {
                    // attempt to cast the selectedOptions string array into a list
                    var casted = (string[]) selectedOptions;
                    options = casted.ToList();
                }
            }

            templateParameters.Add("selectedOptions", options);

            var validOptions = GetVisibleOptions(user, searchContext).ToList();
            templateParameters.Add("optionCSSClasses", GetOptionCssClasses(validOptions));

            var invalidOptions = GetInvalidOptions(user, options, validOptions);
            SearchContextRenderHelper.AddSearchContextParams(searchContext, templateParameters);

//            var processed = ProcessOptions(validOptions, invalidOptions);
//            foreach (var o in processed)
//            {
//                templateParameters.Add(o.Key, o.Value);
//            }

            templateParameters.Add("visibleManufacturers", validOptions);
            templateParameters.Add("invalidOptions", invalidOptions);
        }

        /// <summary>
        /// Construct edit HTML parameters and add them to a template parameters map.
        /// </summary>
        /// <param name="user">The user performing the search</param>
        /// <param name="searchContext">The search context</param>
        /// <param name="fieldValuesHolder">Contains the values the user has selected</param>
        /// <param name="displayParameters"></param>
        /// <returns></returns>
        public override string GetEditHtml(User user, ISearchContext searchContext, IFieldValuesHolder fieldValuesHolder, IDictionary<string, object> displayParameters)
        {
            var templateParameters = GetDisplayParams(user, searchContext, fieldValuesHolder, displayParameters);
            AddEditParameters(fieldValuesHolder, searchContext, user, templateParameters);
            return RenderEditTemplate("ManufacturerSearcherEdit", templateParameters);
        }

        public override bool IsShown(User user, ISearchContext searchContext)
        {
            return true;
        }

        public override string GetViewHtml(User user, ISearchContext searchContext, IFieldValuesHolder fieldValuesHolder, IDictionary<string, object> displayParameters)
        {
            var templateParameters = GetDisplayParams(user, searchContext, fieldValuesHolder, displayParameters);
            AddViewParameters(fieldValuesHolder, searchContext, user, templateParameters);
            return RenderEditTemplate("ManufacturerSearcherView", templateParameters);
        }

        public override bool IsRelevantForQuery(User user, IQuery query)
        {
            return IsRelevantForQuery(SystemSearchConstants.ForManufacturer().JqlClauseNames, query);
        }

        /// <summary>
        /// Returns all possible options (not just those that are visible).
        /// </summary>
        /// <returns></returns>
        private ICollection<IOption> GetAllOptions()
        {
            var allOptions = new List<IOption>();
            var allManufacturers = ManufacturerService.GetManufacturers(1); // todo organization fix
            allOptions.AddRange(allManufacturers.Select(manufacturer => new AssetConstantOption(manufacturer)));

            return allOptions;
        }

        /// <param name="user"> The user performing the search. </param>
        /// <param name="selectedOptions"> Options the user has selected. </param>
        /// <param name="validOptions"> All options that may be selected. </param>
        /// <returns> Invalid options that the user has selected. </returns>
        private ICollection<IOption> GetInvalidOptions(User user, ICollection<string> selectedOptions, ICollection<IOption> validOptions)
        {
            ICollection<IOption> invalidOptions = new List<IOption>();

            if (selectedOptions != null)
            {
                foreach (var option in GetAllOptions())
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
        /// <param name="options">The manufacturer options.</param>
        /// <returns>A map of options to CSS classes.</returns>
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

        /// <param name="searchContext">The search context.</param>
        /// <param name="user">The user performing the search.</param>
        /// <returns>All manufacturer options visible in the given search context.</returns>
        private IEnumerable<IOption> GetOptionsInSearchContext(ISearchContext searchContext, User user)
        {
            var options = new List<IOption>();
            var manufacturers = GetCatalogManufacturers(GetCatalogsInSearchContext(searchContext, user));

            foreach (var manufacturer in manufacturers)
            {
                options.Add(new AssetConstantOption(manufacturer));
            }

            return options;
        }

        private IEnumerable<IOption> GetVisibleOptions(User user, ISearchContext searchContext)
        {
            var options = new List<IOption>();
            var optionsInSearchContext = GetOptionsInSearchContext(searchContext, user);

            // Add the "All Standard Manufacturers" option and the standard types.
            options.AddRange(optionsInSearchContext);

            return options;
        }

        private IEnumerable<Manufacturer> GetCatalogManufacturers(IEnumerable<Catalog> catalogs)
        {
            var manufacturers = new HashSet<Manufacturer>();
            foreach (var catalog in catalogs)
            {
                var catalogManufacturers = CatalogService.GetManufacturers(catalog.Id);
                foreach (var manufacturer in catalogManufacturers)
                {
                    manufacturers.Add(manufacturer);
                }
            }

            var sorted = manufacturers.OrderBy(m => m.Name);
            return sorted;
        } 

        /// <summary>
        /// todo: actually do something useful here
        /// </summary>
        /// <param name="searchContext"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        private IEnumerable<Catalog> GetCatalogsInSearchContext(ISearchContext searchContext, User user)
        {
            var allCatalogs = CatalogService.GetCatalogs(1).ToList(); // todo org fix
            var catalogs = new List<Catalog>(allCatalogs.Count);
            foreach (var catalog in allCatalogs)
            {
                var isVisibleCatalog = searchContext.IsForAnyCatalogs() ||
                                       searchContext.CatalogIds.Contains(catalog.Id);
                if (isVisibleCatalog)
                {
                    catalogs.Add(catalog);
                }
            }

            return catalogs;
        }

        private IDictionary<string, object> ProcessOptions(IEnumerable<IOption> validOptions, IEnumerable<IOption> invalidOptions)
        {
            IDictionary<string, object> result = new Dictionary<string, object>();
            //var options = new SortedSet<IOption>();
            var options = new List<IOption>();
            
            foreach (var validOption in validOptions)
            {
                options.Add(validOption);
            }

            foreach (var invalidOption in invalidOptions)
            {
                options.Add(invalidOption);
            }
             
            // We need to actually remove special options from the collection as
            // STANDARD_OPTIONS_PREDICATE matches them (they'd be in two groups).
//            result["specialOptions"] = CollectionUtils.select(options, SPECIAL_OPTION_PREDICATE);
//            options = CollectionUtils.selectRejected(options, SPECIAL_OPTION_PREDICATE);

            result.Add("standardOptions", options);

            //result["standardOptions"] = CollectionUtils.select(options, IssueConstantOption.STANDARD_OPTIONS_PREDICATE);
            //result["subtaskOptions"] = CollectionUtils.select(options, IssueConstantOption.SUB_TASK_OPTIONS_PREDICATE);

            return result;
        }

        /// <summary>
        /// Construct view HTML parameters and add them to a template parameters map.
        /// </summary>
        /// <param name="fieldValuesHolder">Contains the values the user has selected.</param>
        /// <param name="searchContext">The search context.</param>
        /// <param name="user">The user performing the search/</param>
        /// <param name="templateParameters">The template parameters.</param>
        private void AddViewParameters(IFieldValuesHolder fieldValuesHolder, ISearchContext searchContext, User user, IDictionary<string, object> templateParameters)
        {
            var manufacturers = new List<string>();
            var manufacturerIds = new List<int>();
            var invalidManufacturers = new List<string>();

            object ids;
            if (fieldValuesHolder.TryGetValue(DocumentConstants.ManufacturerId, out ids))
            {
                // check if this is a HashSet and crudely cast it to a list
                var set = ids as HashSet<string>;
                if (set != null)
                {
                    var casted = set;
                    manufacturerIds = casted.Select(id => Convert.ToInt32(id)).ToList();
                }
                else
                {
                    // attempt to cast the selectedOptions string array into a list
                    var casted = (string[]) ids;
                    manufacturerIds = casted.Select(id => Convert.ToInt32(id)).ToList();
                }
            }

            // The IDs of all valid options in the search context
            var validOptionIds = GetVisibleOptions(user, searchContext).Select(o => o.Id).ToList();

            if (!manufacturerIds.Any())
            {
                return;
            }

            var mfgs = manufacturerIds.Select(id => ManufacturerService.GetManufacturer(id)).ToList();
            foreach (var manufacturer in mfgs)
            {
                manufacturers.Add(manufacturer.Name);
                if (!validOptionIds.Contains(manufacturer.Id.ToString()))
                {
                    invalidManufacturers.Add(manufacturer.Name);
                }
            }

            templateParameters.Add("invalidManufacturers", invalidManufacturers);
            templateParameters.Add("selectedManufacturers", manufacturers);
            SearchContextRenderHelper.AddSearchContextParams(searchContext, templateParameters);
        }
    }
}