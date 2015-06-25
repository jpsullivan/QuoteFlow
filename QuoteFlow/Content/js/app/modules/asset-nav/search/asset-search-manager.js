"use strict";

var _ = require('underscore');
var Brace = require('backbone-brace');

var AsyncData = require('../util/async-data');

/**
 * Handles the AJAX asset search requests, including both jql search and stable update.
 * For the very first search, this class will return the initial search data delivered with the page,
 * if available.
 *
 * Also keeps AsyncData of assetKeys within the search.
 */
var AssetSearchManager = Brace.Evented.extend({
    namedEvents: [
        // Triggered before executing a search.
        "beforeSearch",

        // Triggered when a search request fails.
        "searchError"
    ],

    /**
     * @param {object} options
     * @param {object} options.initialIssueTableState The asset table's initial state.
     */
    initialize: function (options) {
        _.extend(this, options);
        this.assetKeys = new AsyncData();
    },

    /**
     * @return {boolean} whether the next search request will return the initial search without making an AJAX request.
     */
    hasInitialSearch: function () {
        return !!this.initialIssueTableState;
    },

    /**
     * Execute a new search, generating a new set of stable asset IDs.
     *
     * @param {object} data
     * @param {number} [data.filterId] The ID of the search filter.
     * @param {string} data.jql The search JQL.
     * @param {number} [data.startIndex=0] The index of the first result to return.
     * @param {boolean} [data.columnConfig] The request columns to be used. Either user, filter, system or explicit.
     * @return {jQuery.Promise} a promise that is resolved when the search completes.
     */
    search: function (data) {
        var deferred,
            traceKey;

        data = _.extend({}, data);

        if (_.isNumber(data.startIndex) === false) {
            data.startIndex = 0;
        }

        // We don't want to have more than one request in flight for results. This can cause unexpected results.
        if (this.activeResultsReq) {
            // If it is the same as the request we are currently waiting for we can just ignore.
            if (JSON.stringify(this.activeRequestData) === JSON.stringify(data)) {
                return jQuery.Deferred().reject().promise();
            } else {
                // Otherwise we will abort and issue a new request.
                this.activeResultsReq.abort();
            }
        }

        this.activeRequestData = data;
        this.triggerBeforeSearch();

        // Initial asset search state is included in the page to avoid making an AJAX request.
        if (this.hasInitialSearch()) {
            deferred = jQuery.Deferred().resolve(this.initialIssueTableState);
            traceKey = "quoteflow.search.finished.initial";
            this.initialIssueTableState = null;
            this.initialIssueIds = null;
        } else {
            deferred = this.activeResultsReq = this._doSearch(data);
            traceKey = "quoteflow.search.finished.secondary";
        }

        deferred.always(_.bind(function () {
            this.activeResultsReq = null;
            this.activeRequestData = null;
            console.log(traceKey);
        }, this));

        deferred.done(_.bind(this._updateAssetKeysOnSearchSuccess, this));

        deferred.fail(_.bind(function () {
            this.assetKeys.reset();
            this.triggerSearchError();
            _.defer(QuoteFlow.trace, "quoteflow.search.finished");
        }, this));

        return deferred.pipe(function (data) {
            if (data.assetTable) {
                // Only AssetSearchManager uses these.
                delete data.assetTable.assetKeys;
            }

            return data;
        }).promise();
    },

    /**
     * Construct a request for asset table information.
     * Fails fast if the given data is invalid (e.g. invalid filter ID) and
     * doesn't actually make an AJAX request; just returns a rejected deferred.
     *
     * @param data The data to use in the request.
     * @return {jQuery.Deferred} a deferred response.
     */
    _doSearch: function (data) {
        // If the filter ID is invalid, fail. We really should move this logic
        // into AssetTableResource, but that's a slightly more risky change.
        var isInteger = /^-?\d+$/;
        if (data.filterId && !isInteger.test(data.filterId)) {
            var response = {
                status: 400,
                responseText: JSON.stringify({
                    errors: [AJS.I18n.getText("navigator.error.filter.id.not.number", data.filterId)]
                })
            };

            return jQuery.Deferred().reject(response).promise();
        }

        debugger;
        return jQuery.ajax({
            type: "POST",
            url: QuoteFlow.RootUrl + "api/assetTable",
            //headers: JIRA.Issues.XsrfTokenHeader,
            data: _.extend(data, {
                layoutKey: "split-view"
            })
        });
    },

    _updateAssetKeysOnSearchSuccess: function (searchResult) {
        var assetIds = searchResult.assetIds;
        var assetKeys = searchResult.assetKeys;
        var assetKeyMapping;

        if (assetIds && assetKeys) {
            assetKeyMapping = {};
            _.each(assetIds, function (assetId, index) {
                assetKeyMapping[assetId] = {
                    value: assetKeys[index],
                    error: false
                };
            });
            this.assetKeys.reset(assetKeyMapping);
        } else {
            // Stable search is off, resort to extracting keys for current page only from the table html
            assetKeyMapping = AssetSearchManager._extractAssetKeysFromTable(searchResult.table);
            this.assetKeys.reset(assetKeyMapping);
        }
    },

    /**
     * Retrieve asset table information for the assets matching the given IDs.
     *
     * @param {number[]} ids The asset IDs.
     * @return {jQuery.Deferred} A deferred that is resolved when the request completes.
     */
    getRowsForIds: function (ids, searchOptions) {
        if (!ids.length) {
            // Don't need to make a request, respond with an empty results set
            return jQuery.Deferred().resolve({}).promise();
        }

        var request = jQuery.ajax({
            data: _.extend({
                id: ids,
                layoutKey: "split-view"
            }, searchOptions),
            type: "POST",
            //headers: JIRA.Issues.XsrfTokenHeader,
            url: QuoteFlow.RootUrl + "api/assetTable/stable"
        });

        request.fail(_.bind(function () {
            this.triggerSearchError();
        }, this));

        return request.pipe(function (data) {
            return data.assetTable;
        }).done(_.bind(function (data) {
            this.assetKeys.setMultiple(AssetSearchManager._extractAssetKeysFromTable(data.table, ids));
            _.defer(QuoteFlow.trace, "quoteflow.search.finished");
        }, this)).promise();
    },

    setAsInaccessible: function (id) {
        return this.assetKeys.setError(id);
    }
}, {
    // Returns id->key map information from asset table html.
    // @param assetTableHtml - table html for the current page
    // @param assetIds - optional. If supplied, inaccessible rows will have an entry in the map (mapping to null).
    _extractAssetKeysFromTable: function (assetTableHtml, assetIds) {
        var map = {};
        AJS.$(assetTableHtml).find('.issuerow').each(function (i) {
            var $row = AJS.$(this);
            var id = assetIds ? assetIds[i] : $row.attr('rel');
            var key = $row.data('issuekey') || null;
            if (id) {
                map[id] = key ? { value: key, error: false } : { error: true };
            }
        });
        return map;
    }
});

module.exports = AssetSearchManager;
