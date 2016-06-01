"use strict";

var _ = require('underscore');
var UrlSerializer = require('../../../util/url-serializer');

/**
 * An object representing  application state.
 * @property {number} filter - ID of the filter
 * @property {string} selectedIssueKey - key of the selected issue
 * @property {string} jql - JQL of the search
 * @property {string} filterJql - JQL of the filter
 * @property {number} startIndex - 0-based index of the first issue visible in search results
 * @property {string} viewIssueQuery - a query parameters for the view issue page
 * @property {number} searchId - unique search id
 * @constructor
 *
 * @param {Object} state - an object containing initial state parameters
 */
var State = function State (state) {
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
    toUrl: function toUrl () {
        return UrlSerializer.getURLFromState(this);
    },

    /**
     * Does this state represent standalone issue (View Asset page).
     * @return {boolean}
     */
    isStandaloneIssue: function isStandaloneIssue () {
        return Boolean(this.selectedIssueKey) && !_.isString(this.jql) && !this.filter;
    },

    /**
     * Convert URL to state object.
     * @param {string} url - URL from which the state will be derived
     * @return {State} state object
     */
    getStateFromUrl: function getStateFromUrl (url) {
        return UrlSerializer.getStateFromURL(url);
    }
});

module.exports = State;
