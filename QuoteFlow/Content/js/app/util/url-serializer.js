﻿(function () {
    var BASE_BROWSE = "browse/",
        BASE_ASSETS = "assets/";

    var returnAsIs = function (x) {
        return x;
    };

    var returnAsNumber = function (x) {
        if (typeof x === 'string') {
            return parseInt(x, 10);
        }

        return x;
    };

    /**
     * An object describing the state of the issue navigator.
     *
     * @typedef {object} JIRA.Issues.URLSerializer.state
     * @property {string} selectedIssueKey
     * @property {(string|null)} filterJql
     * @property {(string|null)} filter
     * @property {(string|null)} jql
     * @property {number} startIndex
     */

    /**
     * (De)serializes page state to URLs.
     */
    QuoteFlow.Utilities.URLSerializer = {

        /**
         * Construct a URL representation of a state object.
         *
         * @param {JIRA.Issues.URLSerializer.state} state The state object.
         * @return {string} A URL representation of <tt>state</tt>.
         */
        getURLFromState: function (state) {
            state = state || {};

            var query = [];
            var base;

            if (state.selectedIssueKey) {
                base = BASE_BROWSE + state.selectedIssueKey;
            } else {
                base = BASE_ASSETS;
            }
            if (state.filter != null) {
                query.push('filter=' + state.filter);
            }

            if (state.jql != null && (state.filterJql == null || state.jql !== state.filterJql)) {
                query.push('jql=' + encodeURIComponent(state.jql));
            }
            if (state.startIndex && !state.selectedIssueKey) {
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
        getStateFromURL: function (url) {
            var parameters = {},
                path = url.split("?")[0],
                queryString;

            var state = {
                filter: null,
                jql: null,
                selectedIssueKey: null,
                startIndex: 0
            };

            if (url.indexOf(BASE_BROWSE) == 0) {
                state.selectedIssueKey = path.split("/")[1];
            }

            if (url.indexOf("?") !== -1) {
                queryString = url.substr(url.indexOf("?"));
                parameters = JIRA.Issues.QueryStringParser.parse(queryString);

                //Need to keep a record of these and pass them along down to view issue
                //so that the correct element can be scrolled into view.
                //These can be trashed afterward with no side effect.
                var viewIssueQuery = _.pick(parameters, 'focusedCommentId', 'attachmentSortBy', 'attachmentOrder');
                if (!_.isEmpty(viewIssueQuery)) {
                    state.viewIssueQuery = viewIssueQuery;
                }
            }

            // return convert the parameters using the conversion functions before returning
            return _.inject(this.PARAMETER_TRANSFORM, function (state, convertFn, key) {
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
})();