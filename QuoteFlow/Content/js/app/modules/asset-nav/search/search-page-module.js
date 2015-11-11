"use strict";

var _ = require('underscore');

var Brace = require('backbone-brace');
var Utilities = require('../../../components/utilities');

var ColumnPicker = require('../../../components/table/column-picker');
var ContentAddedReason = require('../util/reasons');
var EventTypes = require('../util/types');
var FullScreenLayout = require('./full-screen-controller');
var InlineLayer = require('../../../components/layer/inline-layer');
var SimpleAsset = require('./asset/simple-asset');
var SplitScreenLayout = require('../split-view/layout');
var UrlSerializer = require('../../../util/url-serializer');

/**
 *
 */
var SearchPageModule = Brace.Model.extend({
    namedEvents: ["changeFilterProps"],

    namedAttributes: [
        "currentLayout",
        "layouts",
        "filter",
        "jql",
        "searchId"
    ],

    defaults: {
        filter: null,
        jql: null
    },

    initialize: function (attributes, options) {
        _.extend(this, options);

        this.registerColumnPicker();

        // This is here instead of in defaults, because we use the defaults
        // to reset this module's state (filter and jql) but we don't want to
        // reset the layouts.
        this.set({
            layouts: {}
        });

        this.registerLayout("list-view", {
            label: "List View",
            iconClass: 'icon-view-list',
            view: FullScreenLayout
        });

        this.registerLayout("split-view", {
            label: "Detail View",
            iconClass: 'icon-view-split',
            view: SplitScreenLayout
        });
        this._onFilterChanged();
        this.on("change:filter", this._onFilterChanged, this);

        QuoteFlow.application.vent.on("assetEditor:close", this.returnToSearch, this);
        QuoteFlow.application.vent.on("assetEditor:loadComplete", function (model, props) {
            if (!this.standalone && !props.reason) {
                this.searchResults.selectAssetById(model.getId(), { reason: "assetLoaded" });
                this.searchResults.updateAssetById({ id: model.getId(), action: "rowUpdate" }, { filter: this.getFilter() });

                // Replace URL if issue updated successfully
                if (model.getKey()) {
                    var state = this._getUpdateState({ selectedIssueKey: model.getKey() });
                    if (this._validateNavigate(state)) {
                        this.assetNavRouter.replaceState(state);
                    }
                }
            }
        }, this);

        QuoteFlow.application.vent.on("assetEditor:saveSuccess", function (props) {
            this.searchResults.updateAssetById({ id: props.issueId, action: "inlineEdit" }, { filter: this.getFilter() });
        }, this);

        Utilities.initializeResizeHooks();
    },

    registerColumnPicker: function () {
        this.columnConfig = ColumnPicker.create({ search: this });
    },

    getInactiveLayouts: function () {
        var layouts = [];
        _.each(this.getLayouts(), function (layout, key) {
            if (key !== "split-view") {
                layouts.push(layout);
            }
        }, this);
        return layouts;
    },

    getActiveLayout: function () {
        return this.getLayouts()["split-view"];
    },

    /**
     * Change the page layout.
     * <p/>
     * No-op if the requested layout is already selected.
     *
     * @param {string} key The key of the layout to change to.
     * @param {object} [options]
     * @param {boolean} [options.ajax=true] Whether to POST the user's preferred layout to the server.
     * @param {boolean} [options.render=true] Whether to render the new layout.
     */
    changeLayout: function (key, options) {
        var layout = this.getLayout(key),
            newLayout,
            previousLayout = this.getCurrentLayout();

        QuoteFlow.application.changingLayout = true;

        // JRADEV-20786 - Scroll to top of page before changing layouts.
        jQuery("body, html").scrollTop(0);

        options = _.defaults({}, options, {
            ajax: true,
            render: true
        });

        if (layout) {
            // If the requested layout is already selected, do nothing.
            // if (previousLayout instanceof layout.view) {
            //     return;
            // }

            if (previousLayout && previousLayout.close) {
                previousLayout.close();
                // now unselect the selected issue. the assumption here is that we are switching to
                // a mode that does not have an issue selected by default (i.e. list view).
                this.searchResults.unselectAsset({ replace: true });
            }

            //JIRA.Issues.LayoutPreferenceManager.setPreferredLayoutKey(key, options);

            newLayout = new layout.view({
                fullScreenAsset: this.fullScreenAsset,
                assetContainer: this.assetContainer,
                assetCacheManager: this.assetCacheManager,
                search: this.search,
                searchContainer: this.searchContainer,
                searchHeaderModule: this.searchHeaderModule,
                columnConfig: this.columnConfig
            });

            newLayout.on("close", function () {
                this.searchContainer.find('.navigator-content').addClass("pending");
            }, this);

            newLayout.on("render", function () {
                this.searchContainer.find('.navigator-content').removeClass("pending");
                QuoteFlow.trigger(EventTypes.LAYOUT_RENDERED, [key]);
            }, this);

            options.render && newLayout.render();
            this.setCurrentLayout(newLayout);

            this.standalone = false;
        }
    },

    /**
     * Create an instance of the user's preferred layout and set it as the current layout.
     */
    createLayout: function () {
        if (!this.getCurrentLayout()) {
            this.changeLayout("split-view", { render: false });
            //this.fullScreenAsset.deactivate();
            this.fullScreenAsset.destroy();
        }
    },

    _onFilterChanged: function () {
        var previousFilter = this.previous('filter');
        if (previousFilter) {
            previousFilter.off('change', this.triggerChangeFilterProps, this);
        }

        var currentFilter = this.getFilter();
        if (currentFilter) {
            currentFilter.on('change', this.triggerChangeFilterProps, this);
        }
    },

    /**
     * @param {string} key A layout key.
     * @return {object|null} The layout associated with <tt>key</tt> or <tt>null</tt>.
     */
    getLayout: function (key) {
        return this.getLayouts()[key] || null;
    },

    /**
     * @return {object} an array of all registered layouts, sorted by label.
     */
    getSortedLayouts: function () {
        return _.sortBy(this.getLayouts(), "label");
    },

    /**
     * Associate a layout class with a key.
     *
     * @param {string} key A key used to identify the layout. If the key isn't unique, the old layout is overridden.
     * @param {object} layout The layout class to be associated with <tt>key</tt>; its constructor, not an instance.
     */
    registerLayout: function (key, layout) {
        layout.id = key;
        this.get("layouts")[key] = layout;
    },

    /**
     * Get jql but make sure that any requests to get jql have completed.
     * @return {jQuery.Deferred}
     */
    getJqlDeferred: function () {
        var deferred = jQuery.Deferred();
        var instance = this;
        // I am adding a settimeout to fix the following case and avoid similar ones in the future.
        // I open a searcher, make some changes. Clicking the "Save" button to update the filter, I want to
        // get the jql after the searcher have made their request to the server. Unfortunately because the click
        // event of the "Save" button happens before the searchers make their request, we need to delay a tad.
        _.defer(function () {
            instance.queryModule.searchersReady().always(function () {
                // Similar senario as the one above except in this case the request has returned but the jql hasn't been set.
                _.defer(function () {
                    deferred.resolve(instance.getEffectiveJql());
                });
            });
        });
        return deferred.promise();
    },

    registerIssueSearchManager: function (searchManger) {
        this.assetSearchManager = searchManger;
    },

    registerIssueCacheManager: function (issueCacheManager) {
        this.assetCacheManager = issueCacheManager;
    },

    registerQueryModule: function (queryModule) {
        this.queryModule = queryModule;
        this.queryModule.onJqlChanged(this.queryModuleSearchRequested, this);
        this.queryModule.onJqlError(this.disableLayoutSwitcher, this);
        this.queryModule.onJqlSuccess(this.enableLayoutSwitcher, this);
        this.queryModule.onVerticalResize(QuoteFlow.Interactive.triggerVerticalResize);
        this.queryModule.onQueryTooComplexSwitchToAdvanced(function () {
            QuoteFlow.application.execute("analytics:trigger", "kickass.queryTooComplexSwitchToAdvanced");
        });
        this.queryModule.onBasicModeCriteriaCountWhenSearching(function (data) {
            QuoteFlow.application.execute("analytics:trigger", "kickass.basicModeCriteriaCountWhenSearching", data);
        });
        this.queryModule.onChangedPreferredSearchMode(function (mode) {
            QuoteFlow.application.execute("analytics:trigger", "kickass.switchto" + mode);
        });

//        Shifter.register(SearchShifter({
//            isBasicMode: _.bind(this.queryModule.isBasicMode, this.queryModule),
//            isFullScreenIssue: _.bind(this.isFullScreenIssueVisible, this),
//            searcherCollection: this.queryModule.getSearcherCollection()
//        }).create());
    },

    disableLayoutSwitcher: function () {
        if (this.layoutSwitcher) {
            this.layoutSwitcher.disableLayoutSwitcher();
        }
    },

    enableLayoutSwitcher: function () {
        if (this.layoutSwitcher) {
            this.layoutSwitcher.enableLayoutSwitcher();
        }
    },

    registerLayoutSwitcher: function (layoutSwitcher) {
        this.layoutSwitcher = layoutSwitcher;
    },

    registerFilterModule: function (newFilterModule) {
        if (this.filterModule) {
            this.filterModule.off('filterRemoved');
            this.filterModule.off('filterSelected');
        }

        this.filterModule = newFilterModule;
        this.filterModule.on('filterRemoved', function (props) {
            var currentFilter = this.getFilter();
            if (currentFilter && props.filterId === currentFilter.getId()) {
                this.resetToBlank();
            }
        }, this);

        this.filterModule.on('filterSelected', function (props) {
            this.resetToFilter(props.filterId);
        }, this);
    },

    registerSearch: function (search) {
        this.search = search;
        this.searchResults = this.search.getResults();
        this.searchResults.on("change:resultsId", this._handleSearchResultsChange, this);
        this.searchResults.onStartIndexChange(this._handleSearchResultsChange, this);
        this.searchResults.onSelectedAssetChange(this._handleSearchResultsChange, this);

        var columnConfig = this.columnConfig;

        //TODO This event must be fired before searchResults.on*Change events in order to work
        //Make sure that is a design feature and not a coincidence
        this.on("change:filter", function () {
            //When switch to another filter, clear the columns
            columnConfig.clearFilterConfiguration();
        });

        this.searchResults.onColumnsChange(function (searchResults) {
            var configName = searchResults.getColumnConfig();
            if (configName) { //There is no columnConfig on empty search
                columnConfig.syncColumns(configName, searchResults.getColumns());
            }
        });

        this.searchResults.onColumnConfigChange(function (searchResults) {
            var configName = searchResults.getColumnConfig();
            if (configName) { // there is no columnConfig on empty search
                columnConfig.setCurrentColumnConfig(configName);
                // when the columnConfig changes, always set the columns
                columnConfig.syncColumns(configName, searchResults.getColumns());
            }
        });

        columnConfig.onColumnsSync(function (columnConfigName) {
            search.stableUpdate({
                columnConfig: columnConfigName
            });
        });

        this.searchResults.onSelectedAssetChange(_.bind(function (issue) {
            if (!issue.hasAsset()) {
                QuoteFlow.application.execute("assetEditor:removeAssetMetadata");
            }
        }, this));
    },

    _handleSearchResultsChange: function (model, options) {
        options = options || {};
        options.replace ?
                this.assetNavRouter.replaceState(this.getState()) :
                this.assetNavRouter.pushState(this.getState());
    },

    registerSearchHeaderModule: function (searchHeaderModule) {
        this.searchHeaderModule = searchHeaderModule;
    },

    registerFullScreenAsset: function (fullScreenIssue) {
        this.fullScreenAsset = fullScreenIssue;
    //     this.fullScreenAsset.bindAssetHidden(function () {
    //         QuoteFlow.application.execute("assetEditor:dismiss");
    //         this.updateWindowTitle(this.getFilter());
    //         QuoteFlow.trigger(EventTypes.NEW_CONTENT_ADDED, [this.searchContainer, ContentAddedReason.returnToSearch]);
    //    }, this);
    },

    /**
     * @param {element} options.assetContainer The element in which assets are to be rendered.
     * @param {element} options.searchContainer The element in which search results are to be rendered.
     */
    registerViewContainers: function (options) {
        this.assetContainer = options.assetContainer;
        this.searchContainer = options.searchContainer;
    },

    registerIssueNavRouter: function (assetNavRouter) {
        this.assetNavRouter = assetNavRouter;
    },

    prevAsset: function () {
        if (this._overlayIsVisible()) {
            return false;
        }
        if (QuoteFlow.application.request("assetEditor:canDismissComment") && !this.standalone) {
            this.getCurrentLayout().prevAsset();
            return true;
        }

        return false;
    },

    nextAsset: function () {
        if (this._overlayIsVisible()) {
            return false;
        }
        if (QuoteFlow.application.request("assetEditor:canDismissComment") && !this.standalone) {
            this.getCurrentLayout().nextAsset();
            return true;
        }

        return false;
    },

    /**
     * Is there an issue currently being loaded
     * @return Boolean
     */
    isCurrentlyLoadingAsset: function () {
        return QuoteFlow.application.request("assetEditor:isCurrentlyLoading");
    },

    _overlayIsVisible: function () {
        return AJS.$(".aui-blanket").filter(":visible").length > 0;
    },

    /**
     * Retrieve the ID of the selected asset.
     * If asset search is visible, the ID of the currently highlighted asset is
     * returned; if we're viewing an asset, its ID is returned.
     *
     * @param {AJS.Dialog} [dialog] The dialog requesting this information.
     * @return {number} The ID of the currently selected asset.
     */
    getEffectiveIssueId: function (dialog) {
        return this.getEffectiveAsset().getId();
    },

    /**
     * Update the UI in response to an asset update.
     *
     * @param {object} issueUpdate An asset update object (see <tt>JIRA.Issues.Utils.getUpdateCommandForDialog</tt>).
     * @return {jQuery.Deferred} A deferred that is resolved when the refresh completes.
     */
    updateAsset: function (assetUpdate) {
        var isDelete = assetUpdate.action === JIRA.Issues.Actions.DELETE,
            isFullScreen = this.fullScreenAsset.isVisible();

        if (isDelete) {
            return this._deleteIssue(assetUpdate);
        } else if (isFullScreen) {
            return this.fullScreenAsset.updateAsset(assetUpdate).done(_.bind(function () {
                // If it's not a standalone issue, then we also need to update the search results.
                //
                // Things break if these requests are made in parallel, so force them to be serial.
                !this.standalone && this.searchResults.updateAsset(assetUpdate, { showMessage: false, filter: this.getFilter() });
            }, this));
        } else {
            return this.searchResults.updateAsset(assetUpdate, { filter: this.getFilter() });
        }
    },

    /**
     * Update the UI in response to asset deletion.
     *
     * @param {object} issueUpdate An asset update object (see <tt>JIRA.Issues.Utils.getUpdateCommandForDialog</tt>).
     * @return {jQuery.Deferred} A deferred that is resolved when the update completes.
     * @private
     */
    _deleteIssue: function (assetUpdate) {
        var isFullScreen = this.fullScreenAsset.isVisible(),
            isVisibleAsset = assetUpdate.key == QuoteFlow.application.request("assetEditor:getAssetKey");

        if (!isFullScreen) {
            return this.searchResults.updateAsset(assetUpdate);
        } else if (!isVisibleAsset) {
            return this.fullScreenAsset.updateAsset(assetUpdate);
        } else if (this.standalone) {
            this.resetToBlank();
            JIRA.Issues.showNotification(assetUpdate.message, assetUpdate.key);
            return jQuery.Deferred().resolve().promise();
        } else {
            this.returnToSearch();
            return this.searchResults.updateAsset(assetUpdate);
        }
    },

    /**
     * Retrieve the key of the selected asset.
     * If asset search is visible, the key of the currently highlighted asset is
     * returned; if we're viewing an asset, its key is returned.
     *
     * @return {number} The key of the currently selected asset.
     */
    getEffectiveAssetKey: function () {
        return this.getEffectiveAsset().getKey();
    },

    getEffectiveAsset: function () {
        var hasHighlightedIssue = this.searchResults.hasHighlightedAsset(),
            hasSelectedAsset = this.searchResults.hasSelectedAsset();

        var issueModuleIssue = new SimpleAsset({
            id: QuoteFlow.application.request("assetEditor:getAssetId"),
            key: QuoteFlow.application.request("assetEditor:getAssetKey")
        });

        if (this.standalone) {
            return issueModuleIssue;
        } else if (hasSelectedAsset) {
            return this.searchResults.getSelectedAsset();
        } else if (hasHighlightedIssue) {
            return this.searchResults.getHighlightedAsset();
        } else {
            return issueModuleIssue;
        }
    },

    isHighlightedAssetAccessible: function () {
        return this.search.getResults().isHighlightedAssetAccessible();
    },

    /**
     * Show asset search and change the URL to match model state.
     * If returning from a stand-alone asset, reset to a blank search.
     */
    returnToSearch: function () {
        if (this.standalone) {
            this.resetToBlank();
            //QuoteFlow.trace("quoteflow.returned.to.search");
        } else if (this.fullScreenAsset.isVisible()) {
            this.searchResults.unselectAsset();
            QuoteFlow.application.execute("assetEditor:beforeHide");
            // TODO: defensive check, incase issue-nav-components is a lower version than expected. Can remove after soaking for bit on ondemand.
            if (this.queryModule.refreshLayout) {
                this.queryModule.refreshLayout();
            }
        } else {
            QuoteFlow.trace("quoteflow.returned.to.search");
        }
        jQuery.event.trigger("updateOffsets.popout");
    },

    toggleFilterPanel: function () {
        return this.filterModule.toggleFilterPanel();
    },

    issueTableSortRequested: function (jql, startIndex) {
        this.update({ jql: jql, startIndex: startIndex });
    },

    issueTableSearchError: function (response) {
        if (response.status !== 0) {
            // if we haven't aborted the request
            //this.filterModule.filtersComponent.markFilterHeaderAsInvalid();
            var errors;
            try {
                errors = JSON.parse(response.responseText);
            } catch (error) {
                errors = {
                    errorMessages: [
                        "Error occurred communicating with the server. Please reload the page and try again."
                    ]
                };
            }
            this.queryModule.onSearchError(errors);
        }
    },

    issueTableSearchSuccess: function (data) {
        this.update({
            startIndex: data.startIndex
        });
        this.queryModule.onSearchSuccess(data.warnings);
    },

    issueTableStableUpdate: function (startIndex) {
        this.update({ startIndex: startIndex });
    },

    /**
     * Prompt the user to confirm navigation if there are any dirty forms.
     *
     * @param {object} [options]
     * @param {function} [options.confirm=window.confirm] Show a confirmation dialog.
     * @param {boolean} [options.ignoreDirtiness=false] Whether to ignore dirty forms.
     * @return {boolean} whether the user confirmed navigation.
     */
    confirmNavigation: function (options) {
        options = _.defaults({}, options, {
            // Why can't we use bind or apply, I hear you ask? IE8, that's why.
            confirm: function (message) {
                return window.confirm(message);
            },
            ignoreDirtiness: false
        });

        var message = JIRA.DirtyForm.getDirtyWarning() || JIRA.Issue.getDirtyCommentWarning();
        return !!options.ignoreDirtiness || message === undefined || options.confirm(message);
    },

    /**
     * @return {boolean} whether a full screen issue is visible.
     */
    isFullScreenIssueVisible: function () {
        return this.fullScreenAsset && this.fullScreenAsset.isVisible();
    },

    isIssueVisible: function () {
        var layoutKey = JIRA.Issues.LayoutPreferenceManager.getPreferredLayoutKey();

        if (this.isFullScreenIssueVisible()) {
            return true;
        } else if (layoutKey === "list-view") {
            return this.fullScreenAsset.isVisible();
        } else if (layoutKey === "split-view") {
            // Issue is always visible in split view AS LONG AS there are results
            return this.search.getResults().hasAssets();
        }
        return false;
    },

    queryModuleSearchRequested: function (jql) {
        this.update({
            jql: jql,
            startIndex: 0,
            selectedIssueKey: null,
            searchId: _.uniqueId()
        });
    },

    filterModuleSaved: function (filterModel) {
        this.reset({ filter: filterModel.getId() });
    },

    discardFilterChanges: function () {
        this.update({
            jql: null,
            selectedIssueKey: null
        }, true);
    },

    getState: function () {
        var filter = this.getFilter();

        var state = {
            filter: filter && filter.getId(),
            filterJql: filter && filter.getJql(),
            jql: this.getJql()
        };

        if (this.standalone) {
            state.selectedIssueKey = QuoteFlow.application.request("assetEditor:getIssueKey");
        } else {
            _.extend(state, this.search.getResults().getState());
        }

        return state;
    },

    _doSearch: function (options) {
        options = options || {};
        var searchOptions = {};
        var filter = this.getFilter();
        searchOptions.startIndex = options.startIndex;
        if (filter) {
            searchOptions.filterId = filter.getId();
        }

        if (options.columnConfig) {
            searchOptions.columnConfig = options.columnConfig;
        }

        searchOptions.jql = this.getEffectiveJql();
        var searchPromise = this.assetSearchManager.search(searchOptions);

        searchPromise.done(_.bind(function (results) {
            if (this.fullScreenAsset.isVisible() && !AJS.Meta.get('serverRenderedViewIssue')) {
                QuoteFlow.application.execute("assetEditor:beforeHide");
            }
            this.searchResults.resetFromSearch(_.extend(options, results.assetTable));
            this.queryModule.onSearchSuccess(results.warnings);
            jQuery.event.trigger("updateOffsets.popout");
        }, this)).fail(_.bind(function (xhr) {
            if (xhr.statusText !== "abort") {
                if (xhr.status == 400 && options.selectedIssueKey) {
                    this.reset({ selectedIssueKey: options.selectedIssueKey }, { replace: true });
                } else {
                    this.searchResults.resetFromSearch(_.extend(_.pick(options, "selectedIssueId"), this.searchResults.defaults));
                    this.issueTableSearchError(xhr);
                }
            }
        }, this));

        return searchPromise;
    },

    updateWindowTitle: function (model) {
        if (this.isFullScreenIssueVisible()) {
            return;
        }

        // var filter = model,
        //     navigatorTitle = AJS.format('{0} - {1}', AJS.I18n.getText('navigator.title'), JIRA.Settings.ApplicationTitle.get());
        //
        // if (filter && filter.getIsValid()) {
        //     document.title = "[" + filter.getName() + "] " + navigatorTitle;
        // } else {
        //     document.title = navigatorTitle;
        // }
    },

    _applyState: function (state, isReset, options) {
        options = options || {};
        var prevState = _.extend(this.toJSON(), this.search.getResults().toJSON());
        var stateToApply = _.pick(state, Object.keys(this.namedAttributes));
        this.set(stateToApply);
        var newState = _.extend(this.toJSON(), state);
        this.updateWindowTitle(this.getFilter());

        if (isReset) {
            var jql = (state.filter && state.jql == null) ? state.filter.getJql() : state.jql;
            this.queryModule.resetToQuery(jql, { focusQuery: options.isNewSearch }).always(_.bind(function() {
                // Hide the query view for invalid filters.
                this.queryModule.setVisible(!state.filter || state.filter.getIsValid());
            }, this));
        }

        var searchPromise;
        if (this.shouldPerformNewSearch(prevState, newState)) {
            searchPromise = this._doSearch(newState);
        } else {
            searchPromise = jQuery.Deferred().resolve();
            if ("selectedIssueKey" in state) {
                this.searchResults.selectAssetByKey(state.selectedIssueKey);
            }
            // If an issue is selected, its position in the results determines the page and we can ignore startIndex.
            if ("startIndex" in state && !state.selectedIssueKey) {
                this.searchResults.goToPage(state.startIndex);
            }
        }

        this._showIntroDialogs(searchPromise);
    },

    /**
     * Determines if we would need to perform a new (unstable) search if
     * <tt>SearchPageModule</tt> was to be updated with the given attributes.
     *
     * @return {boolean} whether we should perform a new search.
     */
    shouldPerformNewSearch: function (prevState, newState) {
        var prevFilterId = prevState.filter && prevState.filter.getId();
        var filterId = newState.filter && newState.filter.getId();
        var filterChanged = prevFilterId !== filterId;
        var jqlChanged = newState.jql !== prevState.jql;
        var searchIdChanged = newState.searchId !== prevState.searchId;
        return filterChanged || jqlChanged || searchIdChanged;
    },

    refreshSearch: function () {
        return this._doSearch(_.extend({}, this.getState(), {
            selectedIssueKey: undefined
        }));
    },

    _navigateToState: function (state, isReset, options) {
        options = options || {};

        // if (!QuoteFlow.application.request("assetEditor:canDismissComment")) {
        //     this.queryModule.queryChanged();
        //     InlineLayer.current && InlineLayer.current.hide();
        //     return null;
        // }

        if (this._validateNavigate(state)) {
            options.replace ? this.assetNavRouter.replaceState(state) : this.assetNavRouter.pushState(state);
        }
        if (this.search.isStandAloneAsset(state)) {
            this.resetToStandaloneIssue(state);
        } else {
            return this.applyState(state, isReset, options);
        }
    },


    _validateNavigate: function (newState) {
        var urlFromState = UrlSerializer.getURLFromState;
        return urlFromState(newState) !== urlFromState(this.getState());
    },

    _getUpdateState: function (state) {
        return _.extend({}, this.getState(), state);
    },

    update: function (state, isReset, options) {
        this._navigateToState(this._getUpdateState(state), isReset, options);
    },

    reset: function (state, options) {
        var resetState = _.extend({}, this.defaults(), state);
        resetState.searchId = _.uniqueId();
        this._navigateToState(resetState, true, options);
    },

    _deactivateCurrentLayout: function () {
        var currentLayout = this.getCurrentLayout();
        if (currentLayout) {
            currentLayout.close && currentLayout.close();
            this.setCurrentLayout(null);
        }
    },

    resetToStandaloneIssue: function (state) {
        this._deactivateCurrentLayout();
        this.set(this.defaults());
        this.standalone = true;
        this.fullScreenAsset.show({
            key: state.selectedIssueKey,
            viewIssueQuery: state.viewIssueQuery
        });
    },

    applyState: function (state, isReset, options) {
        var filterRequest;
        //var systemFiltersRequest = this.initSystemFilters();

        QuoteFlow.application.execute("assetEditor:abortPending");
        this.createLayout();

        console.warn('todo: remove false constant');
        var shouldFetchFilter = false;

        if (shouldFetchFilter) {
            // Wait for the system filters request to finish as state.filter may refer to a system filter.
            filterRequest = jQuery.Deferred();
            systemFiltersRequest.always(_.bind(function () {
                this.filterModule.getFilterById(state.filter).always(function (filterModel) {
                    state.filter = filterModel;
                    filterRequest.resolve();
                });
            }, this));
        }

        jQuery.when(filterRequest).always(_.bind(function () {
            this._applyState(state, isReset, options);
        }, this));
    },

    updateFromRouter: function (state) {
        if (this.search.isStandAloneAsset(state)) {
            this.resetToStandaloneIssue(state);
        } else {
            this.applyState(state, !this._isSearchStateEqual(state));
        }
    },

    hasSelectedAsset: function () {
        return this.search.getResults().getSelectedAsset().getKey();
    },

    /**
     * Reset the application state to match a given filter.
     *
     * @param {number|JIRA.Components.Filters.Models.Filter} filter The (id of) the filter to reset to.
     */
    resetToFilter: function (filter) {
        //Selecting a filter should always attempt to use the filter columns by default
        //This will ensure request are being made with the specified behaviour above
        //Returning issue table request will contain the actual columns being used and
        //  the preference state will be updated accordingly
        this.reset({
            filter: filter,
            searchId: _.uniqueId()
        });
    },

    /**
     * Reset the query to jql=
     * A reset forces a new search to be performed even if there are no changes.
     */
    resetToBlank: function (options) {
        this.reset({ jql: "" }, options);
    },

    /**
     * @return {boolean} whether the current search is dirty (a modified filter).
     */
    isDirty: function () {
        var filter = this.getFilter();
        return !!filter && filter.getJql() !== this.getEffectiveJql();
    },

    getSearchMode: function () {
        return this.queryModule.getSearchMode();
    },

    getActiveBasicModeSearchers: function () {
        return this.queryModule.getActiveBasicModeSearchers();
    },

    /**
     * Set the user's session search to a given filter.
     *
     * @param filterModel The filter.
     * @private
     */
    setSessionSearch: function (filterModel) {
        // We don't really care if this request fails; it just means that the
        // URL may unnecessarily include the JQL parameter.
        AJS.$.ajax({
            data: {
                filterId: filterModel.getId()
            },
            type: "PUT",
            url: AJS.contextPath() + "/rest/issueNav/1/issueTable/sessionSearch/"
        });
    },

    openFocusShifter: function () {
        JIRA.Issues.FocusShifter.show();
    },

    /**
     * @param {Object} issueProps. Either id or key needs to be present.
     * @param issueProps.issueId
     * @param issueProps.issueKey
     */
    setAsInaccessible: function (issueProps) {
        this.issueTableModule.setAsInaccessible(issueProps);
    },

    /**
     * @param {Object|null} issueProps. If null/undefined, use currently selected issue.
     * @param issueProps.issueId
     * @param issueProps.issueKey
     */
    showInlineIssueLoadError: function (issueProps) {
        var html = JIRA.Components.IssueViewer.Templates.Body.errorsLoading();
        JIRA.Messages.showErrorMsg(html, { closeable: true });
    },

    /**
     * In the case of no filter selected, simply gets the jql property.
     * When a filter is selected, will get the filter jql and any modifications.
     *
     * @return {string} the effective JQL.
     */
    getEffectiveJql: function () {
        var filter = this.getFilter(),
            jql = this.getJql();

        if (_.isString(jql)) {
            return jql;
        } else if (filter) {
            return filter.getJql() || "";
        } else {
            return "";
        }
    },

    /**
     * On standalone VI, system filters data will not be available on page load
     * Thus make calls to make sure it is loaded properly via ajax
     */
    initSystemFilters: function () {
        return this.filterModule.initSystemFilters();
    },

    addOwnerToSystemFilters: function (systemFilters) {
        var loggedInUser = AJS.Meta.get('remote-user');

        if (!loggedInUser) {
            return systemFilters;
        }

        var ownerDisplayName = AJS.Meta.get('remote-user-fullname');
        var avatarUrl = AJS.Meta.get('remote-user-avatar-url');

        return _.map(systemFilters, function (filter) {
            filter.ownerUserName = loggedInUser;
            filter.ownerDisplayName = ownerDisplayName;
            filter.avatarUrl = avatarUrl;
            return filter;
        });
    },

    handleLeft: function () {
        if (this._allowLeftRightNavigation()) {
            this.getCurrentLayout() && this.getCurrentLayout().handleLeft();
        }
    },

    handleRight: function () {
        if (this._allowLeftRightNavigation()) {
            this.getCurrentLayout() && this.getCurrentLayout().handleRight();
        }
    },

    handleUp: function () {
        if (!this._allowUpDownNavigation()) {
            return false;
        }

        // Allow arrow scrolling up if first issue is highlighted.
        if (this.searchResults.isFirstAssetHighlighted()) {
            return false;
        }

        return this.prevAsset();
    },

    handleDown: function () {
        if (!this._allowUpDownNavigation()) {
            return false;
        }

        return this.nextAsset();
    },

    _allowLeftRightNavigation: function () {
        return !AJS.keyboardShortcutsDisabled;
    },

    _allowUpDownNavigation: function () {
        if (AJS.keyboardShortcutsDisabled) {
            return false;
        }

        // Don't allow up/down navigation if dropdowns are open.
        if (InlineLayer.current || AJS.Dropdown.current || JIRA.Dialog.current || AJS.$(".aui-dropdown2:visible").length > 0) {
            return false;
        }

        return this.getCurrentLayout() && !this.getCurrentLayout().isIssueViewActive();
    },

    _isSearchStateEqual: function (state) {
        var searchParams = ["filter", "jql", "startIndex"];
        return _.isEqual(_.pick(state, searchParams), _.pick(this.getState(), searchParams));
    },

    /**
     * Remove all of the tipsies that are open.
     */
    //removeOpenTipsies: JIRA.Issues.Tipsy.revalidate,

    _showIntroDialogs: function (searchPromise) {
        var filterPanelPromise = (this.filterModule && this.filterModule.filterPanelView) ? this.filterModule.filterPanelView.panelReady : undefined;
        if (!this._shownIntroDialog && this.layoutSwitcher) {
            jQuery.when(searchPromise, filterPanelPromise).done(_.bind(function () {
                this.layoutSwitcher.createHelptipForSwitchingToDetailView(1);
                this.filterModule.createHelptipForFilterPanelDocking(2);
                AJS.HelpTip.Manager.showSequences();
            }, this));
            this._shownIntroDialog = true;
        }
    }
});

module.exports = SearchPageModule;
