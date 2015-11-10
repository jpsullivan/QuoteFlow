"use strict";

var _ = require('underscore');
var Marionette = require('backbone.marionette');

/**
 * This service encapsulates all the search related stuff.
 *
 * @extends Marionette.Controller
 */
var SearchServiceController = Marionette.Controller.extend({
    /**
     * @event assetUpdated
     * When an asset has been updated
     */

    /**
     * @event search
     * When an new search has been done
     */

    /**
     * @event before:search
     * Before doing a new search
     */

    /**
     * @event error:search
     * When a new search has been done but it thrown an error
     */

    /**
     * @event assetHighlighted
     * When an asset has been marked as highlighted in the internal model
     */

    /**
     * @event selectedAssetChanged
     * When an asset has been marked as selected in the internal model
     */

    /**
     * @param {Object} options
     * @param {SearchModule} options.searchModule
     * @param {SearchResults} options.searchResults
     * @param {ColumnConfigModel} options.columnConfig
     */
    initialize: function (options) {
        this.searchModule = options.searchModule;
        this.searchResults = options.searchResults;
        this.columnConfig = options.columnConfig;

        _.bindAll(this, "_onAssetUpdated", "_onHighlightedAssetChange", "_onSelectedAssetChange");

        this.listenTo(this.searchResults, "change:resultsId change:startIndex stableUpdate assetDeleted", function () {
            debugger;
            this._doSearch();
        });

        // These are *not* regular Backbone events, we can't use listenTo.
        this.searchResults.onAssetUpdated(this._onAssetUpdated);
        this.searchResults.onHighlightedAssetChange(this._onHighlightedAssetChange);
        this.searchResults.onSelectedAssetChange(this._onSelectedAssetChange);
    },

    _onSelectedAssetChange: function (asset) {
        this.trigger("selectedAssetChanged", asset, this.searchResults.getHighlightedAsset());
    },

    _onAssetUpdated: function (assetId, entity, reason) {
        this.trigger("assetUpdated", assetId, entity, reason);
    },

    _onHighlightedAssetChange: function (asset) {
        this.trigger("assetHighlighted", asset.getId());
    },

    close: function () {
        this.stopListening();
        this.searchResults.offAssetUpdated(this._onAssetUpdated);
        this.searchResults.offHighlightedAssetChange(this._onHighlightedAssetChange);
    },

    /**
     * Asks the SearchResults object to do a new search with the parameters already contained
     * in the SearchModule
     * @private
     */
    _doSearch: function () {
        this.searchInProgress = true;
        this.trigger("before:search");

        var filterId = this.searchModule.getFilterId();
        var isSystemFilter = filterId < 0;
        filterId = (isSystemFilter || _.isNull(filterId)) ? undefined : filterId;

        this.searchResults.getResultsForPage({
            jql: this.searchModule.getEffectiveJql(),
            filterId: filterId
        })
            .always(_.bind(function () {
                this.searchInProgress = false;
            }, this))
            .done(_.bind(function (table) {
                if (!this.searchResults.hasHighlightedAsset()) {
                    this.searchResults.highlightFirstInPage();
                }
                this.trigger("search", table, this.searchResults);
            }, this))
            .fail(_.bind(function () {
                this.trigger("error:search");
            }, this));
    },

    /**
     * Loads a page of the current search results. This code expects the asset position,
     * not the page number. Example: using a pageSize of 25, passing startIndex=50 will load the
     * assets #50 to #74.
     *
     * This method will do nothing if a search is already in progress
     *
     * @param {number} startIndex Position of the asset in the page
     */
    goToPage: function (startIndex) {
        if (this.searchInProgress) {
            return;
        }
        this.searchResults.goToPage(startIndex);
    },

    /**
     * Sorts the current search results by a field.
     *
     * This method will do nothing if a search is already in progress
     *
     * @param {string} fieldId ID of the field to sort by
     */
    sort: function (fieldId) {
        if (this.searchInProgress) {
            return;
        }

        var allSorts = this.searchModule.getResults().getColumnSortJql();
        var sortJql = allSorts[fieldId];
        if (sortJql) {
            this.searchModule.doSort(sortJql);
        }
    },

    /**
     * Re-runs the current search.
     *
     * This method will do nothing if a search is already in progress
     */
    runCurrentSearch: function () {
        if (this.searchInProgress) {
            return;
        }

        if (JIRA.Issues.Application.request("assetEditor:canDismissComment")) {
            JIRA.Issues.Application.execute("analytics:trigger", "kickass.assetTableRefresh");
            this.searchModule.refresh();
        }
    },

    /**
     * This method updates the existing set of results (aka Stable Search). It does not re-run the search again, so the
     * list of Issues will remain the same.
     *
     * This method will do nothing if a search is already in progress.
     */
    updateExitingResults: function () {
        if (this.searchInProgress) {
            return;
        }

        if (JIRA.Issues.Application.request("assetEditor:canDismissComment")) {
            JIRA.Issues.Application.execute("analytics:trigger", "kickass.assetTableRefresh");
            this._doSearch();
        }
    },

    highlightAsset: function (assetId) {
        this.searchResults.highlightAssetById(assetId);
    },

    getPager: function () {
        return this.searchResults.getPager();
    },

    selectNextAsset: function () {
        this.searchResults.highlightNextAsset();
        if (this.searchResults.hasSelectedAsset()) {
            this.searchResults.selectNextAsset();
        }
    },

    selectPreviousAsset: function () {
        this.searchResults.highlightPrevAsset();
        if (this.searchResults.hasSelectedAsset()) {
            this.searchResults.selectPrevAsset();
        }
    },

    unselectAsset: function () {
        this.searchResults.unselectAsset();
    },

    hasSelectedAsset: function () {
        return this.searchResults.hasSelectedAsset();
    },

    getHighlightedAsset: function () {
        return this.searchResults.getHighlightedAsset().id;
    }
});

module.exports = SearchServiceController;
