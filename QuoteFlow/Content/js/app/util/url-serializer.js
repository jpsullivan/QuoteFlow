﻿"use strict";

var BASE_BROWSE = "browse/",
    BASE_ASSETS = "quote/3/test-quote/builder/"; // todo: rip this shit out omg

var returnAsIs = function(x) {
    return x;
};

var returnAsNumber = function(x) {
    if (typeof x === 'string') {
        return parseInt(x, 10);
    }
    return x;
};

var _ = require('underscore');
var QueryStringParser = require('./query-string-parser');

/**
 * An object describing the state of the issue navigator.
 *
 * @typedef {object} UrlSerializer.state
 * @property {string} selectedAssetSku
 * @property {(string|null)} filterJql
 * @property {(string|null)} filter
 * @property {(string|null)} jql
 * @property {number} startIndex
 */
var UrlSerializer = {

    /**
     * Construct a URL representation of a state object.
     *
     * @param {UrlSerializer.state} state The state object.
     * @return {string} A URL representation of <tt>state</tt>.
     */
    getURLFromState: function(state) {
        console.warn("todo: fix hardcoded quote url");
        state = state || {};

        var query = [];
        var base;

        if (state.selectedAssetSku) {
            base = BASE_BROWSE + state.selectedAssetSku;
        } else {
            base = BASE_ASSETS;
        }
        if (state.filter != null) {
            query.push('filter=' + state.filter);
        }

        if (state.jql != null && (state.filterJql == null || state.jql !== state.filterJql)) {
            query.push('jql=' + encodeURIComponent(state.jql));
        }

        if (state.startIndex && !state.selectedAssetSku) {
            query.push('startIndex=' + state.startIndex);
        }
        return base + (query.length ? '?' + query.join('&') : "");
    },

    /**
     * Extract state from a URL.
     *
     * @param {string} URL The URL.
     * @return {JIRA.Issues.URLSerializer.state} The state object.
     */
    getStateFromURL: function(url) {
        var parameters = {},
            path = url.split("?")[0],
            queryString;

        var state = {
            filter: null,
            jql: null,
            selectedAssetSku: null,
            startIndex: 0
        };

        if (url.indexOf(BASE_BROWSE) == 0) {
            state.selectedAssetSku = path.split("/")[1];
        }

        if (url.indexOf("?") !== -1) {
            queryString = url.substr(url.indexOf("?"));
            parameters = QueryStringParser.parse(queryString);

            //Need to keep a record of these and pass them along down to view issue
            //so that the correct element can be scrolled into view.
            //These can be trashed afterward with no side effect.
            var viewAssetQuery = _.pick(parameters, 'focusedCommentId', 'attachmentSortBy', 'attachmentOrder');
            if (!_.isEmpty(viewAssetQuery)) {
                state.viewAssetQuery = viewAssetQuery;
            }
        }

        // return convert the parameters using the conversion functions before returning
        return _.inject(this.PARAMETER_TRANSFORM, function(state, convertFn, key) {
            var value = parameters[key];
            if (value !== undefined) {
                // apply conversion and override the defaults
                state[key] = convertFn(value);
            }

            return state;
        }, _.extend(state));
    },

    /**
     * Parameters that are stored in the query string (with an optional conversion/transformation function).
     */
    PARAMETER_TRANSFORM: {
        "filter": returnAsIs,
        "jql": returnAsIs,
        "startIndex": returnAsNumber
    }
};

module.exports = UrlSerializer;
