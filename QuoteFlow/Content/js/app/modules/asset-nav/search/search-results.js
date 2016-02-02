"use strict";

var _ = require('underscore');
var $ = require('jquery');
var Brace = require('backbone-brace');

var ModelUtils = require('../util/model-utils');
var NumberFormatter = require('../../../helpers/number-formatter/number-formatter');
var SimpleAsset = require('./asset/simple-asset');

/**
 *
 */
var SearchResults = Brace.Model.extend({
    defaults: {
        assetIds: [],
        selectedAsset: new SimpleAsset(),
        highlightedAsset: new SimpleAsset(),
        total: 0
    },

    namedAttributes: [
        "columnSortJql",
        "highlightedAsset",
        "assetIds", // only present in stable search
        "assetIdsForPage",
        "quoteflowHasAssets",
        "pageSize",
        "resultsId",
        "selectedAsset",
        "startIndex",
        "sortBy",
        "columns",
        "table",
        "total",
        "initialPayload",
        "columnConfig"
    ],

    namedEvents: [
        "prevAssetSelected",
        "nextAssetSelected",

        "assetUpdated",
        "assetDeleted",

        "assetDoesNotExist",

        "stableUpdate"
    ],

    initialize: function (attr, options) {
        ModelUtils.makeTransactional(this, "resetFromSearch", "selectNextAsset", "selectPrevAsset");
        this.assetUpdateCallbacks = [];
        this._assetSearchManager = options.assetSearchManager;
        this._initialSelectedAsset = options.initialSelectedIssue;
        this._columnConfig = options.columnConfig;
    },

    /**
     * Returns the startIndex for a given page.
     *
     * @param {Number} page the page number (0 based)
     * @return {number} the startIndex
     */
    getStartIndexForPage: function (page) {
        return page * this.getPageSize();
    },

    hasAsset: function (id) {
        return _.indexOf(this.getAssetIds(), id) !== -1;
    },

    getPager: function () {
        if (this.hasAsset(this.getSelectedAsset().get("id"))) {
            var assetIds = this.getAssetIds();
            var selectedId = this.getSelectedAsset().get("id");
            var position = _.indexOf(assetIds, selectedId);
            var resultCount = this.getTotal();
            var stableSearchLimit = assetIds.length;
            var pager = {
                position: NumberFormatter.format(position + 1),
                resultCount: NumberFormatter.format(resultCount)
            };
            if (position > 0) {
                var prevAssetId = this._getPrevAssetId(selectedId);
                if (prevAssetId) {
                    pager.previousAsset = {
                        id: prevAssetId,
                        key: this._getAssetKeyForId(prevAssetId)
                    };
                }
            }
            if (position < resultCount - 1 && position < stableSearchLimit - 1) {
                var nextAssetId = this._getNextAssetId(selectedId);
                if (nextAssetId) {
                    pager.nextAsset = {
                        id: nextAssetId,
                        key: this._getAssetKeyForId(nextAssetId)
                    };
                }
            }
            return pager;
        }
    },

    /**
     * Returns the number of pages in this SearchResults.
     *
     * @return {Number} the number of pages
     */
    getNumberOfPages: function () {
        return this._getPageNumberForStartIndex(this.getDisplayableTotal() - 1);
    },

    /**
     * Remove an asset from the search results.
     * The highlighted asset is updated accordingly.
     *
     * @param {number} id The ID of the asset to remove.
     */
    removeAsset: function (id) {
        id = parseInt(id, 10);

        var isFirstAsset = this.getAssetIds()[0] === id,
            isHighlighted = this.getHighlightedAsset().get("id") === id,
            isLastAsset = _.last(this.getAssetIds()) === id;

        if (isHighlighted) {
            if (!isLastAsset) {
                this.highlightNextAsset({ replace: true });
            } else if (!isFirstAsset) {
                this.highlightPrevAsset({ replace: true });
            }
        }

        this.setAssetIds(_.without(this.getAssetIds(), id));
        this.setTotal(this.getTotal() - 1);
        this.triggerAssetDeleted({
            id: id,
            key: this._getAssetKeyForId(id)
        });
    },

    isFirstAssetHighlighted: function () {
        if (!this.hasHighlightedAsset()) {
            return false;
        }

        return this.getHighlightedAsset().get("id") === this.getAssetIds()[0];
    },

    updateAssetById: function (assetUpdateObject, options) {
        return this.updateAsset(assetUpdateObject, options);
    },

    /**
     * Update the search results in response to an asset update.
     *
     * @param {object} assetUpdate An asset update object (see <tt>JIRA.Issues.Utils.getUpdateCommandForDialog</tt>).
     * @param {object} [options]
     * @param {boolean} [options.showMessage=true] Whether a success message should be shown.
     * @param {JIRA.Components.Filters.Models.Filter} [options.filter=null] Filter, in which context assets should be shown.
     * @return {jQuery.Deferred} A deferred that is resolved when the update completes.
     */
    updateAsset: function (assetUpdate, options) {
        var isDelete = assetUpdate.action === "delete",
            assetId = assetUpdate.id,
            promises = [];

        options = _.defaults({}, options, {
            showMessage: true,
            filter: null
        });

        if (isDelete) {
            this.removeAsset(assetId);
            options.showMessage && this._notifyOfAssetUpdate(assetUpdate);
            return $.Deferred().resolve().promise();
        } else {
            return this.getResultForId(assetId, options.filter).done(_.bind(function (entity) {
                _.each(this.assetUpdateCallbacks, function (callback) {
                    var result = callback.handler.call(callback.ctx || window, assetId, entity, assetUpdate);
                    if (result && result.promise) {
                        promises.push(result);
                    }
                });

                $.when(promises).done(_.bind(function () {
                    options.showMessage && this._notifyOfAssetUpdate(assetUpdate);
                }, this));
            }, this));
        }
    },

    _notifyOfAssetUpdate: function (assetUpdate) {
        assetUpdate.message && JIRA.Issues.showNotification(assetUpdate.message, assetUpdate.key);
        console.log('quoteflow.search.stable.update');
    },

    getResultForId: function (id, filter) {
        var options = { columnConfig: this._columnConfig.columnPickerModel.getColumnConfig() };

        // if used filter is system filter we don't want to get results based off him
        if (filter && !filter.getIsSystem()) {
            options = _.extend({ filterId: filter.get("id") }, options);
        }
        return this._assetSearchManager.getRowsForIds([id], options);
    },

    isHighlightedAssetAccessible: function () {
        if (!this.hasHighlightedAsset()) {
            return null;
        }
        return !this._assetSearchManager.assetKeys.hasError(this.getHighlightedAsset().get("id"));
    },

    getDisplayableTotal: function () {
        return this.getAssetIds().length;
    },

    selectAssetByKey: function (key, options) {
        if (!key) {
            this.unselectAsset();
            return;
        }

        var id = this._getAssetIdForKey(key);

        if (!id || id === -1) {
            this._unhighlightAsset();

            this.getSelectedAsset().set({
                id: -1
            });

            this.triggerAssetDoesNotExist();
        } else {
            this._selectExistingAssetById(id, options);
        }
    },

    _unhighlightAsset: function () {
        this.getHighlightedAsset().set({
            id: null
        });
    },

    selectFirstInPage: function (options) {
        this.selectAssetById(this.getAssetIds()[this.getStartIndex()], options);
    },

    getPrevPageStartIndex: function () {
        var target = (this.getStartIndex() || 0) - this.getPageSize();
        return (target < 0) ? null : target;
    },

    getNextPageStartIndex: function () {
        var target = this.getPositionOfLastAssetInPage();
        return (target >= this.getDisplayableTotal()) ? null : target;
    },

    /**
     * Returns the position in the search results of the last asset on the current page
     * (e.g. if the page size is 5 and we are on the first page this returns 5).
     *
     * @return {Number}
     */
    getPositionOfLastAssetInPage: function () {
        var startIndex = this.getStartIndex();
        var pageSize = this._pageSize();

        return Math.min(startIndex + pageSize, this.getDisplayableTotal());
    },

    highlightFirstInPage: function () {
        this.highlightAssetById(this.getAssetIds()[this.getStartIndex()]);
    },

    selectAssetById: function (id, options) {
        if (!id) {
            this.unselectAsset();
        } else {
            this._selectExistingAssetById(id, options);
        }
    },

    _selectExistingAssetById: function (id, options) {
        if (id !== this.getSelectedAsset().get("id")) {
            id = id ? parseInt(id, 10) : null;
            this.getSelectedAsset().set({
                id: id
            }, options);
            this.highlightAssetById(id);
        }
    },

    /**
     * Sets the asset with the given id as the highlighted asset and updates the startIndex accordingly (such that
     * the startIndex is equal to the offset of the first asset of the page that contains the highlighted asset).
     *
     * @param {number} id The ID of the asset to highlight.
     * @param {object} [options]
     * @param {boolean} [options.replace=false] Whether highlighting the asset should be a "replace" operation.
     */
    highlightAssetById: function (id, options) {
        options = _.defaults({}, options, {
            replace: false
        });

        if (id && id !== this.getHighlightedAsset().get("id")) {
            id = id ? parseInt(id, 10) : null;
            this.getHighlightedAsset().set({
                id: id
            }, options);
            if (id) {
                this.setStartIndex(this._getStartIndexForAssetId(id));
            }
        }
    },

    getState: function () {
        return {
            selectedAssetId: this.getSelectedAsset().get("id"),
            startIndex: this.getStartIndex()
        };
    },

    /**
     * Resets this SearchResults using new search data. This effectively wipes any existing state and replaces it
     * with the passed-in state.
     *
     * Calling this method generates a new <code>resultsId</code>, which triggers a "newPayload" event.
     *
     * @param state
     */
    resetFromSearch: function (state) {
        state.resultsId = _.uniqueId();
        this.getSelectedAsset().set({ id: null });
        this.set({ sortBy: null });
        this.set("startIndex", state.startIndex, { silent: true });
        this.set(_.pick(state, Object.keys(this.namedAttributes)));
        if (typeof state.selectedAssetId === 'string') {
            this.selectAssetById(state.selectedAssetId);
        } else {
            if (this.hasAssets()) {
                this.highlightFirstInPage();
            } else {
                this.getHighlightedAsset().set({ id: null });
            }
        }
    },

    hasAssets: function () {
        return !!this.getAssetIds().length;
    },

    isAssetOnPage: function (id) {
        return _.indexOf(this.getPageAssetIds(), id) !== -1;
    },

    isFirstAssetSelected: function () {
        if (this.hasAsset(this.getSelectedAsset().get("id"))) {
            var assetIds = this.getAssetIds();
            var selectedId = this.getSelectedAsset().get("id");
            var position = _.indexOf(assetIds, selectedId);
            return position === 0;
        }

        return false;
    },

    /**
     * @param options
     * @param options.filterId
     * @param options.columnConfig
     */
    getResultsForPage: function (options) {
        _.extend(options, { columnConfig: this._columnConfig.columnPickerModel.getColumnConfig() });

        if (this.getTable()) {
            var result = this.getTable();
            this.setTable(null, { silent: true }); //do not trigger a new search
            _.defer(function () {
                console.log('quoteflow.search.finished');
            });
            return $.Deferred().resolve(result).promise();
        }
        return this.getResultsForIds(this.getPageAssetIds(), options);
    },

    /**
     * @param ids
     * @param options
     * @param options.filterId
     * @param options.columnConfig
     */
    getResultsForIds: function (ids, options) {
        var instance = this;
        var deferred = $.Deferred();
        this._assetSearchManager.getRowsForIds(ids, options)
        .done(function (result) {
            //HACK - The REST endpoint for splitview always returns columnConfig="user", which is very
            //wrong. We can update the columnConfig only for listview, as that REST endpoint is the one
            //returning the good values.
            var isSplitViewResponse = _.isArray(result.table);
            if (!isSplitViewResponse) {
                instance.setColumnConfig(result.columnConfig);
                instance.setColumns(result.columns);
                instance.setColumnSortJql(result.columnSortJql);
            }
            deferred.resolve(result.table);
        }).fail(deferred.reject)
            .always(function () {
                _.defer(function () {
                    console.log('quoteflow.search.finished');
                });
            });
        return deferred;
    },

    applyState: function (state) {
        this.set(_.pick(state, Object.keys(this.namedAttributes)));
    },

    unselectAsset: function (options) {
        var selectedAsset = this.getSelectedAsset();
        if (selectedAsset.get("id")) {
            selectedAsset.set({
                id: null
            }, options);
        }
    },

    hasSelectedAsset: function () {
        return !!this.getSelectedAsset().get("id");
    },

    hasHighlightedAsset: function () {
        return !!this.getHighlightedAsset().get("id");
    },

    getPageAssetIds: function () {
        var startIndex = this.getStartIndex();
        var pageSize = this._pageSize();

        var assetIds = this.getAssetIds();
        return assetIds ? assetIds.slice(startIndex, Math.min(startIndex + pageSize, assetIds.length)) : [];
    },

    getPageAssets: function () {
        // return ids and keys
        return _.map(this.getPageAssetIds(), function (id) {
            return { id: id };
        }, this);
    },

    getPageNumber: function () {
        return Math.floor(this.getStartIndex() / this._pageSize());
    },

    /**
     * Returns the position of the given asset id on the page (0-based).
     *
     * @param assetId
     * @return {number}
     */
    getPositionOfAssetInPage: function (assetId) {
        return this.getPositionOfAssetInSearchResults(assetId) % this._pageSize();
    },

    /**
     * Returns the position of the given asset in the stable search results (0-based).
     *
     * @param assetId
     * @return {*}
     */
    getPositionOfAssetInSearchResults: function (assetId) {
        return _.indexOf(this.getAssetIds(), assetId);
    },

    /**
     * (Async) Selects the next asset in the search results if possible. Clients can register a
     * callback using onSelectedAssetChange() to subscribe to selected asset change events,
     * or alternatively use the promise that this method returns.
     *
     * @return {jQuery.Promise} a promise with the id of the selected asset if successful
     */
    selectNextAsset: function (options) {
        if (!this.hasSelectedAsset()) {
            return $.Deferred().reject().promise();
        }

        var nextId = this._getNextAssetId(this.getSelectedAsset().get("id"));
        this._triggerNextAssetSelectedEvent(nextId);

        return this._selectAsset(nextId, options);
    },

    /**
     * (Async) Selects the previous asset in the search results if possible. Clients can register a
     * callback using onSelectedAssetChange() to subscribe to selected asset change events,
     * or alternatively use the promise that this method returns.
     *
     * @return {jQuery.Promise} a promise with the id of the selected asset if successful
     */
    selectPrevAsset: function (options) {
        if (!this.hasSelectedAsset()) {
            return $.Deferred().reject().promise();
        }

        var prevId = this._getPrevAssetId(this.getSelectedAsset().get("id"));
        this._triggerPrevAssetSelectedEvent(prevId);

        return this._selectAsset(prevId, options);
    },

    _selectAsset: function (assetId, options) {
        this.selectAssetById(assetId, options);
        return $.Deferred().resolve(assetId).promise();
    },

    /**
     * (Async) Highlights the previous asset in the search results if possible. Clients can register a
     * callback using onHighlightedAssetChange() to subscribe to selected asset highlighted events,
     * or alternatively use the promise that this method returns.
     *
     * @param {object} [options]
     * @param {boolean} [options.replace=false] Whether highlighting the asset should be a "replace" operation.
     * @return {jQuery.Promise} a promise with the id of the highlighted asset if successful
     */
    highlightNextAsset: function (options) {
        if (!this.hasHighlightedAsset()) {
            return $.Deferred().reject().promise();
        }

        // this is always synchronous in stable search but it is async when we are going across page boundaries
        // in dynamic search (we need to get the set of assets in the next page at this point)
        var nextId = this._getNextAssetId(this.getHighlightedAsset().get("id"));
        this.highlightAssetById(nextId, options);

        return $.Deferred().resolve(nextId).promise();
    },

    /**
     * (Async) Highlights the next asset in the search results if possible. Clients can register a
     * callback using onHighlightedAssetChange() to subscribe to selected asset highlighted events,
     * or alternatively use the promise that this method returns.
     *
     * @param {object} [options]
     * @param {boolean} [options.replace=false] Whether highlighting the asset should be a "replace" operation.
     * @return {jQuery.Promise} a promise with the id of the highlighted asset if successful
     */
    highlightPrevAsset: function (options) {
        if (!this.hasHighlightedAsset()) {
            return $.Deferred().reject().promise();
        }

        // this is always synchronous in stable search but it is async when we are going across page boundaries
        // in dynamic search (we need to get the set of assets in the next page at this point)
        var prevId = this._getPrevAssetId(this.getHighlightedAsset().get("id"));
        this.highlightAssetById(prevId, options);

        return $.Deferred().resolve(prevId).promise();
    },

    /**
     * Show the page starting at <tt>startIndex</tt>.
     * <p/>
     * This method is asynchronous.
     *
     * @param {number} startIndex The index of the first asset on the page.
     * @param {object} [options]
     * @param {boolean} [options.replace=false] Whether showing the page should be a "replace" operation.
     * @return {jQuery.Deferred} A deferred that will be resolved with the ID of the newly highlighted asset.
     */
    goToPage: function (startIndex, options) {
        options = _.defaults({}, options, {
            replace: false
        });

        if (startIndex === null || startIndex === this.getStartIndex()) {
            return $.Deferred().resolve().promise();
        }

        // Highlighting an asset updates the startIndex to ensure it is on the current page.
        var ID = this.getAssetIds()[startIndex];
        if(!ID){
            // fallback
            return $.Deferred().resolve().promise();
        }

        this.highlightAssetById(ID, options);

        return $.Deferred().resolve(ID).promise();
    },

    onHighlightedAssetChange: function (callback, context) {
        this.getHighlightedAsset().on("change", callback, context);
    },

    offHighlightedAssetChange: function (callback, context) {
        this.getHighlightedAsset().off("change", callback, context);
    },

    onSelectedAssetChange: function (callback, context) {
        this.getSelectedAsset().on("change", callback, context);
    },

    offSelectedAssetChange: function (callback, context) {
        this.getSelectedAsset().off("change", callback, context);
    },

    onColumnConfigChange: function (callback, context) {
        this.on("change:columnConfig", callback, context);
    },

    offColumnConfigChange: function (callback, context) {
        this.off("change:columnConfig", callback, context);
    },

    onColumnsChange: function (callback, context) {
        this.on("change:columns", callback, context);
    },

    offColumnsChange: function (callback, context) {
        this.off("change:columns", callback, context);
    },

    onStartIndexChange: function (callback, context) {
        this.on("change:startIndex", callback, context);
    },

    onNewAssetIds: function (callback, context) {
        this.on("change:assetIds", callback, context);
    },

    offNewAssetIds: function (callback, context) {
        this.off("change:assetIds", callback, context);
    },

    offStartIndexChange: function (callback, context) {
        this.off("change:startIndex", callback, context);
    },

    onNewPayload: function (func, context) {
        this.on("change:resultsId", func, context);
    },

    offNewPayload: function (func, context) {
        this.off("change:resultsId", func, context);
    },

//    onAssetUpdated: function (func, ctx) {
//        this.assetUpdateCallbacks.push({
//            handler: func,
//            ctx: ctx
//        });
//    },
//
//    offAssetUpdated: function (func) {
//        var filteredCallbacks = [];
//        this.assetUpdateCallbacks = _.each(this.assetUpdateCallbacks, function (callback) {
//            if (callback.handler !== func) {
//                filteredCallbacks.push(callback);
//            }
//        });
//        this.assetUpdateCallbacks = filteredCallbacks;
//    },

    _getNextAssetId: function (id) {
        var assetIds = this.getAssetIds();
        return assetIds[Math.min(_.indexOf(assetIds, id) + 1, this.getDisplayableTotal() - 1)];
    },

    getNextAssetForId: function (id) {
        var nextId = this._getNextAssetId(id);
        return { id: nextId };
    },

    getNextAssetForSelectedAsset: function () {
        return this.getNextAssetForId(this.getSelectedAsset().get("id"));
    },

    _getPrevAssetId: function (id) {
        var assetIds = this.getAssetIds();
        return assetIds[Math.max(0, _.indexOf(this.getAssetIds(), id) - 1)];
    },

    /**
     * Calculate the start index that should be used to show a particular asset.
     * Returns 0 if the asset isn't present in the search results.
     *
     * @param {number} id The asset's ID.
     * @return {number} The start index.
     * @private
     */
    _getStartIndexForAssetId: function (id) {
        var assetIndex = _.indexOf(this.getAssetIds(), id),
            pageSize = this._pageSize();

        return Math.max(0, Math.floor(assetIndex / pageSize) * pageSize);
    },

    _getAssetIdForKey: function (key) {
        // this only happens if the selected asset is not in the search results (i.e. when the user
        // navigates to the selected asset directly but has a search context).
        if (this._initialSelectedAsset && key === this._initialSelectedAsset.key) {
            return this._initialSelectedAsset.id;
        }

        return this._getAssetKeysToIds()[key];
    },

    _getAssetKeyForId: function (id) {
        // this only happens if the selected asset is not in the search results (i.e. when the user
        // navigates to the selected asset directly but has a search context).
        if (this._initialSelectedAsset && id === this._initialSelectedAsset.id) {
            return this._initialSelectedAsset.key;
        }

        return this._getAssetIdsToKeys()[id];
    },

    _getAssetIdsToKeys: function () {
        return this._assetSearchManager.assetKeys.getAllCached();
    },

    _getAssetKeysToIds: function () {
        var obj = {};
        var idsToKeys = this._assetSearchManager.assetKeys.getAllCached();
        _.each(idsToKeys, function (value, name) {
            obj[value] = name;
        });

        return obj;
    },

    /**
     * Returns a 0-based page number.
     *
     * @param startIndex the start index
     * @return {Number} a 0-based page number.
     * @private
     */
    _getPageNumberForStartIndex: function (startIndex) {
        return Math.floor(startIndex / this._pageSize());
    },

    /**
     * Returns the page size used for the search.
     *
     * @return {Number} the page size
     * @private
     */
    _pageSize: function () {
        return this.getPageSize();
    },

    _triggerPrevAssetSelectedEvent: function (prevId) {
        if (prevId !== this.getSelectedAsset().get("id")) {
            var prevPrevId = this._getPrevAssetId(prevId);

            this.trigger("prevAssetSelected", {
                prevAsset: { id: prevId, key: this._getAssetKeyForId(prevId) },
                prevPrevAsset: { id: prevPrevId, key: this._getAssetKeyForId(prevPrevId) }
            });
        }
    },

    _triggerNextAssetSelectedEvent: function (nextId) {
        if (nextId !== this.getSelectedAsset().get("id")) {
            var nextNextId = this._getNextAssetId(nextId);

            this.trigger("nextAssetSelected", {
                nextAsset: { id: nextId, key: this._getAssetKeyForId(nextId) },
                nextNextAsset: { id: nextNextId, key: this._getAssetKeyForId(nextNextId) }
            });
        }
    }
});

module.exports = SearchResults;
