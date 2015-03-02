"use strict";

var Brace = require('backbone-brace');

/**
 * Updates a ViewAssetData asset cache in response to changes in a SearchResults instance.
 * This includes removing deleted assets from the cache and pre-fetching assets that are
 * likely to be requested.
 */
var AssetCacheManager = Brace.Evented.extend({
    /**
     * @param {object} options
     * @param {SearchResults} options.searchResults The application's <tt>SearchResults</tt> instance.
     * @param {ViewAssetData} options.viewAssetData The application's <tt>ViewAssetData</tt> instance.
     */
    initialize: function (options) {
        this.searchResults = options.searchResults;
        this.viewAssetData = options.viewAssetData;

        this.searchResults.bind("assetDeleted", this._onAssetDeleted, this);
        this.searchResults.bind("nextAssetSelected", this._onNextAssetSelected, this);
        this.searchResults.bind("prevAssetSelected", this._onPrevAssetSelected, this);

        this._prefetchCandidate = null;
    },

    /**
     * Triggers pre-fetching of assets that this AssetCacheManager sees fit to pre-fetch. Calling this method is
     * essentially a hint to the AssetCacheManager that now is a good time to fetch things from the server.
     */
    prefetchAssets: function () {
        if (this._prefetchCandidate && this._prefetchCandidate.key) {
            this._prefetchAsset(this._prefetchCandidate.key);
            this._prefetchCandidate = null;
        }
    },

    scheduleAssetToBePrefetched: function (issue) {
        this._prefetchCandidate = issue;
    },

    /**
     * Remove an asset from the cache in response to its deletion.
     *
     * @param {object} asset
     * @param {number} asset.id The asset's ID.
     * @param {string} asset.key The asset's key.
     * @private
     */
    _onAssetDeleted: function (asset) {
        this.viewAssetData.remove(asset.key);
    },

    /**
     * Handles a SearchResults "nextAssetSelected" event.
     *
     * @param event the event
     * @param event.selected the currently selected asset in the search results
     * @param event.next the next asset in the search results
     */
    _onNextAssetSelected: function (event) {
        // mark the asset just after the selected one as a candidate for pre-fetching
        this._prefetchCandidate = event.nextNextAsset;
    },

    /**
     * Handles a SearchResults "prevAssetSelected" event.
     *
     * @param event the event
     * @param event.selected the currently selected asset in the search results
     * @param event.next the next asset in the search results
     */
    _onPrevAssetSelected: function (event) {
        // mark the asset just previous to the selected one as a candidate for pre-fetching
        this._prefetchCandidate = event.prevPrevAsset;
    },

    /**
     * Pre-fetches an asset by getting it from the cache.
     */
    _prefetchAsset: function (issueKey) {
        if (!JIRA.Issues.DarkFeatures.NO_PREFETCH.enabled()) {
            this.viewAssetData.get(issueKey, false, { prefetch: true });
        }
    }
});

module.exports = AssetCacheManager;