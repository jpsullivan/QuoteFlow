"use strict";

var _ = require('underscore');
var Brace = require('backbone-brace');

/**
 * The state of the quote sidebar.
 */
var QuoteSidebarModel = Brace.Model.extend({
    namedAttributes: [
        "activeLineItem"
    ],

    defaults: {
        "activeLineItem": null
    },

    /**
     * @param {object} attributes
     * @param {object} options
     * @param {object[]} options.systemFilters
     */
    initialize: function (attributes, options) {
        this.IS_DOCKED_STORAGE_KEY = "quote.sidebar.docked";
        this.WIDTH_STORAGE_KEY = "quote.sidebar.width";

        _.extend(this, _.defaults(options, {
            storage: window.localStorage
        }));
    },

    setDocked: function (state) {
        this._getStorage().setItem("dockStatesAnalyticsEnabled", true);
        return this._getStorage().setItem(this.IS_DOCKED_STORAGE_KEY, state);
    },

    setWidth: function (width) {
        return this._getStorage().setItem(this.WIDTH_STORAGE_KEY, width);
    },

    getWidth: function () {
        var storedWidth = parseInt(this._getStorage().getItem(this.WIDTH_STORAGE_KEY), 10);
        return ((storedWidth > 0) ? storedWidth : 200);
    },


    _getStorage: function () {
        return this.storage || window.localStorage;
    },

    isDockedPrefGiven: function () {
        return this._getStorage().getItem(this.IS_DOCKED_STORAGE_KEY) !== null;
    },

    shouldShowDockIntro: function () {
        return !this.isDocked() && !this.isDockedPrefGiven();
    },

    isExpanded: function () {
        // Showing dock intro should collapse the filter panel
        return this.shouldShowDockIntro() ? false : this.isDocked();
    },

    /**
     * @return {boolean}
     */
    isDocked: function () {
        var storage = this._getStorage();
        // dock the sidebar by default or if the user has chosen to do so.
        return storage.getItem(this.IS_DOCKED_STORAGE_KEY) === null || storage.getItem(this.IS_DOCKED_STORAGE_KEY) === "true";
    }
});

module.exports = QuoteSidebarModel;
