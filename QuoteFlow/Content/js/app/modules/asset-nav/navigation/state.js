"use strict";

var _ = require('underscore');
var UrlSerializer = require('../../../util/url-serializer');

/**
 * An object representing  application state.
 * @property {number} filter - ID of the filter
 * @property {string} selectedIssueKey - key of the selected asset
 * @property {string} jql - JQL of the search
 * @property {string} filterJql - JQL of the filter
 * @property {number} startIndex - 0-based index of the first asset visible in search results
 * @property {string} viewIssueQuery - a query parameters for the view asset page
 * @property {number} searchId - unique search id
 * @constructor
 *
 * @param {Object} state - an object containing initial state parameters
 */
var State = function (state) {
    this.filter = null;
    this.jql = null;
    if (state) {
        _.extend(this, state.toJSON());
    }
};

/** @lends State.prototype */
_.extend(State.prototype, {
    /**
     * Construct an URL representation.
     * @return {string} URL representation of state
     */
    toUrl: function () {
        return UrlSerializer.getURLFromState(this);
    },

    /**
     * Does this state represent standalone asset (View Asset page).
     * @return {boolean}
     */
    isStandaloneAsset: function () {
        return Boolean(this.selectedAssetSku) && !_.isString(this.jql) && !this.filter;
    },

    /**
     * Convert URL to state object.
     * @param {string} url - URL from which the state will be derived
     * @return {State} state object
     */
    getStateFromUrl: function (url) {
        return UrlSerializer.getStateFromURL(url);
    }
});

module.exports = State;
