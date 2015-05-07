"use strict";

var $ = require('jquery');
var _ = require('underscore');
var Backbone = require('backbone');
var Brace = require('backbone-brace');
Backbone.$ = $;

var BasicQueryModule = require('./basic_query');
var JqlQueryModule = require('./jql_query');
var QueryView = require('../../asset-nav/query/query-view');

/**
 * Module for basic query mode
 */
var AssetQueryModule = Brace.Evented.extend({
    namedEvents: [
        "jqlChanged",
        "jqlTooComplex",
        "jqlError",
        "jqlSuccess",
        "searchRequested",
        "queryTooComplexSwitchToAdvanced",
        "changedPreferredSearchMode",
        "basicModeCriteriaCountWhenSearching",
        "verticalResize",
        "initialized"
    ],

    initialize: function(options) {
        this._queryStateModel = options.queryStateModel;
        this._queryStateModel.on("change:preferredSearchMode", _.bind(function() {
            this.triggerChangedPreferredSearchMode(this._queryStateModel.getPreferredSearchMode());
        }, this));

//        JIRA.Issues.SearcherDialog.initialize({
//            queryStateModel: this._queryStateModel
//        });

        this._jqlQueryModule = new JqlQueryModule({
            queryStateModel: this._queryStateModel
        })
        .onSearchRequested(this.handleAdvancedSearchRequested, this)
        .onVerticalResize(this.triggerVerticalResize, this);

        this._errors = {};
        this._errors[this._queryStateModel.BASIC_SEARCH] = [];
        this._errors[this._queryStateModel.ADVANCED_SEARCH] = [];

        this._queryStateModel.on("change:searchMode", this.showSearchErrors, this);

        this._basicQueryModule = new BasicQueryModule({
            queryStateModel: this._queryStateModel,
            primaryClauses: options.primaryClauses,
            initialSearcherCollectionState: options.searchers
        })
        .onSearchRequested(this.clearSearchErrors, this)
        .onJqlTooComplex(this.handleJqlTooComplex, this)
        .onSearchRequested(this.handleSearchRequested, this)
        .onVerticalResize(this.triggerVerticalResize, this)
        .onBasicModeCriteriaCountWhenSearching(this.triggerBasicModeCriteriaCountWhenSearching, this);
    },

    /**
     * If we have rendered in the background (when hidden), our size calculations for jql box are
     * incorrect so we need a way for the outside world to tell us when to recalculate.
     */
    refreshLayout: function() {
        this._jqlQueryModule.setQuery();
    },

    handleAdvancedSearchRequested: function (jql) {
        this.handleSearchRequested(jql);
        this._basicQueryModule.queryChanged();
    },

    handleSearchRequested: function (jql) {
        this._queryStateModel.setJql(jql);
        this.clearSearchErrors();
    },

    handleJqlTooComplex: function (jql) {
        if (this.getSearchMode() !== this._queryStateModel.ADVANCED_SEARCH) {
            this.triggerQueryTooComplexSwitchToAdvanced();
        }
        this.setSearchMode(this._queryStateModel.ADVANCED_SEARCH);
        this.triggerJqlTooComplex(jql);
        if (this._queryView) {
            this._queryView.switcherViewModel.disableSwitching();
        }
    },

    getJql: function () {
        return this._queryStateModel.getJql();
    },

    getSearcherCollection: function () {
        return this._basicQueryModule.searcherCollection
    },

    /**
     * @return {boolean} whether the query module is currently in basic mode.
     * @private
     */
    isBasicMode: function () {
        return this._queryStateModel.getSearchMode() === this._queryStateModel.BASIC_SEARCH;
    },

    /**
     * Reset the query module to match the current query.
     *
     * Clears error messages, switches to the user's preferred search mode, and
     * hides the entire query view if the currently selected filter is invalid.
     *
     * If the user has requested a new search then this method will focus the search view.
     *
     * @param options
     * @param options.focusQuery true if we should focus the searchers after resetting
     */
    resetToQuery: function (jql, options) {
        this.clearSearchErrors();
        return this._basicQueryModule.queryReset(jql).always(_.bind(function () {
            this._queryStateModel.switchToPreferredSearchMode();
            this._jqlQueryModule.setQuery();
            if (options && options.focusQuery === true) {
                this._queryView.getView().focus();
            }

            this._basicQueryModule.off("searchRequested", this.publishJqlChanges);
            this._jqlQueryModule.off("searchRequested", this.publishJqlChanges);

            // subsequent search requestes are published
            this._basicQueryModule.onSearchRequested(this.publishJqlChanges, this);
            this._jqlQueryModule.onSearchRequested(this.publishJqlChanges, this);
        }, this));
    },

    publishJqlChanges: function (jql) {
        this.triggerJqlChanged(jql);
    },

    setVisible: function (value) {
        this._queryView.setVisible(value);
    },

    /**
     * Notifies this module that the underlying jql has changed and it should update itself
     */
    queryChanged: function () {
        this.clearSearchErrors();
        this._basicQueryModule.queryChanged();
    },

    onSearchSuccess: function (warnings) {
        if (this._queryView) {
            this._queryView.showWarnings(warnings);
        }
        this.triggerJqlSuccess();
    },

    /**
     * Wait any in flight updates to search collection.
     */
    searchersReady: function () {
        return this._basicQueryModule.searchersReady();
    },

    onSearchError: function (response) {
        this._errors.renderFunction = "showErrors";
        var basicModeErrors = (response.errorMessages) ? response.errorMessages.concat() : [];
        var advancedModeErrors = [];

        _.each(response.errors, function (message, type) {
            if (type === "jql") {
                advancedModeErrors.push(message);
            } else {
                basicModeErrors.push(message);
            }
        });

        if (this.getSearchMode() === this._queryStateModel.BASIC_SEARCH && !this._basicQueryModule.hasErrors() && advancedModeErrors.length > 0) {
            // If the search was performed in basic mode, an advanced mode error was
            // encountered, switch to advanced mode before rendering these errors.
            this.setSearchMode(this._queryStateModel.ADVANCED_SEARCH);
        }

        this._errors[this._queryStateModel.BASIC_SEARCH] = basicModeErrors;
        this._errors[this._queryStateModel.ADVANCED_SEARCH] = advancedModeErrors.concat(basicModeErrors);

        this.showSearchErrors();
        this.triggerJqlError();
    },

    /**
     * Show error messages applicable to the current search mode.
     */
    showSearchErrors: function () {
        if (this._queryView) {
            this._queryView.clearNotifications();
            var renderFunction = this._errors.renderFunction || "showErrors";
            this._queryView[renderFunction](this._errors[this.getSearchMode()]);
        }
    },

    /**
     * Remove error messages from all search modes.
     */
    clearSearchErrors: function () {
        if (this._queryView) {
            this._queryView.clearNotifications();
            this._queryView.switcherViewModel.enableSwitching();
        }
        this._errors[this._queryStateModel.BASIC_SEARCH].length = 0;
        this._errors[this._queryStateModel.ADVANCED_SEARCH].length = 0;
    },

    getSearchMode: function () {
        return this._queryStateModel.getSearchMode();
    },

    getActiveBasicModeSearchers: function () {
        return this._basicQueryModule.getSelectedCriteria()
    },

    /**
     * @param {string} searchMode -- Either "basic" or "advanced"
     * @return {boolean} -- Indicates whether or not the search mode actually changed
     */
    setSearchMode: function (searchMode) {
        if (this.getSearchMode() !== searchMode) {
            this._queryStateModel.switchToSearchMode(searchMode);
            return true;
        }
        return false;
    },

    createAndRenderView: function ($el) {
        this._queryView = new QueryView({
            el: $el,
            queryStateModel: this._queryStateModel,
            basicQueryModule: this._basicQueryModule,
            jqlQueryModule: this._jqlQueryModule
        }).onVerticalResize(this.triggerVerticalResize, this);
        this._queryView.render();
    },

    isQueryValid: function () {
        return (this._errors &&
                this._errors[this._queryStateModel.BASIC_SEARCH].length === 0 &&
                this._errors[this._queryStateModel.ADVANCED_SEARCH].length === 0);
    }
});

module.exports = AssetQueryModule;
