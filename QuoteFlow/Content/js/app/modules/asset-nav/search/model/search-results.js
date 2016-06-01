"use strict";

var _ = require('underscore');
var $ = require('jquery');
var Backbone = require('backbone');
var PageableCollection = require('backbone.paginator');
var Result = require('./search-result');

var SearchResults = PageableCollection.extend({
    initialize: function (models, options) {
        options = options || {};
        this.allAssets = options.assets;
        this.state.pageSize = options.pageSize;
        this.state.totalRecords = options.totalRecordsInSearch;
        this.totalRecordsInDB = options.totalRecordsInDB;
        this.allowNoSelection = options.allowNoSelection;
        this.jql = options.jql;
    },

    model: Result,

    url: function () {
        return QuoteFlow.RootUrl + "api/assetTable/stable";
    },

    // Initial pagination states
    state: {
        firstPage: 0,
        currentPage: 0
    },

    queryParams: {
        currentPage: null,
        pageSize: null,
        totalPages: null,
        totalRecords: null,
        sortKey: null,
        directions: null
    },

    sync: function () {
        var args = _.toArray(arguments);
        var params = args[2];

        params.type = "POST";
        params.headers = {
            "X-Atlassian-Token": "no-check"
        };
        params.data.layoutKey = "split-view";
        params.data.id = this._getStableKeys();

        return PageableCollection.prototype.sync.apply(this, args);
    },

    isAtTheEndOfStableSearch: function () {
        var isLastPage = this.state.currentPage === this.state.lastPage;
        var areThereMoreAssets = this.totalRecordsInDB > this.state.totalRecords;
        return isLastPage && areThereMoreAssets;
    },

    _getStableKeys: function () {
        var start = this._getStartIndex();
        return _.pluck(this.allAssets, "id").slice(start, start + this.state.pageSize);
    },

    _getStartIndex: function () {
        return this.state.pageSize * this.state.currentPage;
    },

    _loadPageAndSelect: function (pageToLoad, assetToSelect) {
        var deferred = new jQuery.Deferred();

        var options = {fetch: true, reset: true};
        this.trigger("before:loadpage", {
            pageToLoad: pageToLoad
        });
        this.getPage(pageToLoad, options)
            .done(_.bind(function () {
                this.select(assetToSelect);
                deferred.resolve(this.length);
            }, this))
            .fail(_.bind(function (response) {
                deferred.reject(response);
                this.trigger("error:loadpage", response);
            }, this));

        return deferred.promise();
    },

    jumpToPage: function (page) {
        var deferred = new jQuery.Deferred();
        this._loadPageAndSelect(page, "first")
            .done(deferred.resolve)
            .fail(deferred.reject);
        return deferred.promise();
    },

    jumpToPageForAsset: function (assetIdOrSku) {
        var deferred = new jQuery.Deferred();
        var isAssetId = (typeof assetIdOrSku === "number");
        var isAssetSku = (typeof assetIdOrSku === "string");

        // Find which page the asset belongs to
        var pageToLoad;
        var asset;
        for (var i = 0, len = this.allAssets.length; i < len && !asset; i++) {
            if (
                (isAssetId && this.allAssets[i].id === assetIdOrSku) ||
                (isAssetSku && this.allAssets[i].key === assetIdOrSku)
            ) {
                asset = this.allAssets[i];
                pageToLoad = Math.floor(i / this.state.pageSize);
            }
        }

        if (typeof pageToLoad === "number" && isFinite(pageToLoad)) {
            // Page for asset found, go to the page and select the asset
            this._loadPageAndSelect(pageToLoad, asset.id).done(deferred.resolve);
        } else if (this.allowNoSelection) {
            // Page for asset not found, but we allow having no selection
            // Load the page but do not select anything
            this._loadPageAndSelect("first", null).done(_.bind(function () {
                this.trigger("selectAssetNotInList", new Result({
                    id: isAssetId ? assetIdOrSku : undefined,
                    key: isAssetSku ? assetIdOrSku : undefined
                }));
            }, this)).done(deferred.resolve);
        } else {
            // Page for asset not found and we require a selection, select
            // the first asset in the first page
            this._loadPageAndSelect("first", "first").done(deferred.resolve);
        }
        return deferred.promise();
    },

    parse: function (resp, options) {
        var requestData = options.data;
        var data = resp && resp.assetTable;

        var allAssets = this.allAssets;
        _.each(data.table, function (rawAsset, index) {
            // If an asset is missing from the server, it will contain the value 'null'
            if (rawAsset === null) {
                // We take advantage of the fact that the order of requestData and the response is the same.
                var assetId = requestData.id[index];

                // Search for this particular asset in the list of assets for this search.
                var oldAssetData = _.findWhere(allAssets, {id: assetId});
                if (oldAssetData) {
                    // Even if the asset is missing from the server, populate the required fields (id and key)
                    // so all our views, events and url management will continue to work.
                    data.table[index] = {
                        id: oldAssetData.id,
                        sku: oldAssetData.sku,
                        inaccessible: true
                    };
                }
            }
        });

        return PageableCollection.prototype.parse.call(this, data);
    },

    parseState: function () {
        return {};
    },

    parseRecords: function (resp) {
        if (resp.table) {
            return resp.table;
        }
        return [];
    },

    select: function (model) {
        if (typeof model === "string") {
            switch (model) {
                case "first":
                    model = this.at(0);
                    break;
                case "last":
                    model = this.at(this.length - 1);
                    break;
                default:
                    model = this.findWhere({key: model});
                    break;
            }
        } else if (typeof model === "number") {
            model = this.get(model);
        }

        if (this.selected !== model) {
            this.unselect();
        }
        if (!model) {
            return;
        }
        this.selected = model;
        this.selected.trigger("select", this.selected);
    },

    unselect: function () {
        if (this.selected) {
            this.selected.trigger("unselect", this.selected);
        }
        this.selected = null;
    },

    selectNext: function () {
        var index = this.indexOf(this.selected);
        var model = this.at(index + 1);
        if (model) {
            this.select(model);
        } else if (this.hasNextPage()) {
            this._loadPageAndSelect("next", "first");
        }
    },

    selectPrev: function () {
        var index = this.indexOf(this.selected);
        var model = this.at(index - 1);
        if (model) {
            this.select(model);
        } else if (this.hasPreviousPage()) {
            this._loadPageAndSelect("prev", "last");
        }
    },

    updateSelectedAsset: function () {
        if (this.selected) {
            this.selected.fetch();
        }
    },

    updateAsset: function (assetId) {
        var asset = this.get(assetId);
        if (asset) {
            asset.fetch();
        }
    },

    hasAssetInSearch: function (assetSku) {
        return Boolean(_.where(this.allAssets, {sku: assetSku}).length);
    },

    isEmptySearch: function () {
        return !this.allAssets.length;
    },

    /**
     * This method removes the model from the collection. If the model was
     * selected, it will try to select a new asset based on the following algorithm:
     *
     *  * If there is a next asset, select it. ...
     *  * If there is a previous asset, select it. Else...
     *  * The list is empty, select nothing
     *
     * @param {SearchResultModel} model Asset to delete
     */
    removeAndUpdateSelectionIfNeeded: function (model) {
        var assetSku = model.get('sku');
        var assetToSelectAfterRemoveOperation;
        var allSkus = _.chain(this.allAssets).map(function (asset) {return asset.sku;}).value();
        var currentIndex = _.indexOf(allKeys, assetSku);
        var hasNext = currentIndex + 1 < allKeys.length;
        var hasPrevious = currentIndex - 1 >= 0;

        if (hasNext) {
            assetToSelectAfterRemoveOperation = allSkus[currentIndex + 1];
        } else if (hasPrevious) {
            assetToSelectAfterRemoveOperation = allSkus[currentIndex - 1];
        } else {
            // There are no more assets, select nothing
            assetToSelectAfterRemoveOperation = null;
        }

        // Remove the asset from this.allAssets
        for (var i = 0; i < this.allAssets.length; i++) {
            var asset = this.allAssets[i];
            if (asset.sku === assetSku) {
                this.allAssets.splice(i, 1);
                break;
            }
        }

        // Remove the asset from the pagination related counters
        this.state.totalRecords = this.state.totalRecords - 1;
        this.totalRecordsInDB = this.totalRecordsInDB - 1;

        // Unselect and remove it
        this.unselect();
        this.remove(model);

        // If we have an asset to select after the remove operation, load its page
        if (assetToSelectAfterRemoveOperation) {
            return this.jumpToPageForAsset(assetToSelectAfterRemoveOperation).done(_.bind(function () {
                this.trigger("assetDeleted");
            }, this));
        } else {
            return new jQuery.Deferred().resolve(0).promise().done(_.bind(function () {
                this.trigger("assetDeleted");
            }, this));
        }
    },

    getAssetSkuForIndex: function (index) {
        var asset = this.allAssets[index];
        if (!asset) {
            return;
        }

        return asset.sku;
    },

    getAssetAtGlobalIndex: function (index) {
        return this.allAssets[index];
    },

    getPositionOfAssetInSearchResults: function (assetId) {
        var position = _.indexOf(_.pluck(this.allAssets, 'id'), assetId);
        if (position > -1) {
            return position;
        }

        return null;
    },

    getPositionOfAssetInPage: function (assetId) {
        var model = this.get(assetId);
        var position = this.indexOf(model);
        if (position > -1) {
            return position;
        }

        return null;
    },

    getTotalAssets: function () {
        return this.allAssets.length;
    }
});

module.exports = SearchResults;
