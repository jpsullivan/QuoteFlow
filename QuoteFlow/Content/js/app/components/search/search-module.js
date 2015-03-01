﻿"use strict";

var Brace = require('backbone-brace');

//var SearchResults = 

/**
 * 
 */
var SearchModule = Brace.Evented.extend({
    initialize: function (options) {
        this._assetSearchManager = options.assetSearchManager;
        this._searchPageModule = options.searchPageModule;
        this._searchResults = new JIRA.Issues.SearchResults(null, {
            assetSearchManager: this._assetSearchManager,
            initialSelectedIssue: options.initialSelectedIssue,
            columnConfig: this._searchPageModule.columnConfig
        });

        // TF-447 - Refactoring step to remove nested dependencies on SearchModule.
        JIRA.Issues.Application.reqres.setHandler("issueNav:currentSearchRequest", this.getCurrentSearchRequest, this);

        JIRA.Issues.Application.commands.setHandler("issueNav:refreshSearch", this.refresh, this);
    },

    /**
     * Sorts this search using the specified JQL or fieldId.
     *
     * @param {object} sortOptions
     * @param {string} sortOptions.fieldId
     * @param {string} sortOptions.jql
     */
    doSort: function (jql) {
        if (jql) {
            this._searchPageModule.update({
                jql: jql,
                startIndex: null,
                selectedIssueKey: null
            }, true);
        }
    },

    getFilterId: function () {
        var filter = this._searchPageModule.getFilter();
        return filter && filter.getId();
    },

    getJql: function () {
        return this._searchPageModule.getJql();
    },

    getEffectiveJql: function () {
        return this._searchPageModule.getEffectiveJql();
    },

    getResults: function () {
        return this._searchResults;
    },

    getState: function () {
        return this._searchPageModule.getState();
    },

    getCurrentSearchRequest: function () {
        return {
            jql: this.getJql(),
            filterId: this.getFilterId()
        }
    },

    /**
     * @param {object} [state=this._searchPageModule.getState()] The state to inspect.
     * @return {boolean} Whether <tt>state</tt> describes a state where a stand alone issue is visible.
     */
    isStandAloneIssue: function (state) {
        state = state || this._searchPageModule.getState();
        return !!state.selectedIssueKey && !_.isString(state.jql) && !state.filter;
    },

    /**
     * Register a callback to be executed before a search is performed.
     *
     * @param {function} callback The callback to execute.
     * @param {object} context The context in which to execute.
     */
    onBeforeSearch: function (callback, context) {
        this._assetSearchManager.bindBeforeSearch(callback, context);
    },

    /**
     * Remove a before search callback.
     *
     * @param {function} callback The callback to remove.
     * @param {object} context The callback's context.
     */
    offBeforeSearch: function (callback, context) {
        this._assetSearchManager.unbindBeforeSearch(callback, context);
    },

    /**
     * Register a callback to be executed when a search fails.
     *
     * @param {function} callback The callback to execute.
     * @param {object} context The context in which to execute.
     */
    onSearchError: function (callback, context) {
        this._assetSearchManager.bindSearchError(callback, context);
    },

    /**
     * Remove a search error callback.
     *
     * @param {function} callback The callback to remove.
     * @param {object} context The callback's context.
     */
    offSearchError: function (callback, context) {
        this._assetSearchManager.unbindSearchError(callback, context);
    },

    refresh: function () {
        return this._searchPageModule.refreshSearch()
    },

    /**
     * Triggers the StableUpdate event. It will force a new
     * search
     * 
     * @param {Object} [opts] Config object with custom options 
     */
    stableUpdate: function (opts) {
        this._searchResults.triggerStableUpdate(_.extend({
            force: true
        }, opts));
    }
});

module.exports = SearchModule;