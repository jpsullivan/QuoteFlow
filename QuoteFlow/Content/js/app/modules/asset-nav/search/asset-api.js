"use strict";

var _ = require('underscore');
var AssetsApi = require('./assets-api');

var AssetApi = {
    searchPageModule: null,

    patch: {
        /**
         * @return {null|string} the currently selected issue's ID or
         *     <tt>null</tt> if no issue is selected.
         */
        getIssueId: function () {
            return AssetsApi.getSelectedIssueId();
        },

        /**
         * @return {null|string} the currently selected issue's key or
         *     <tt>null</tt> if no issue is selected.
         */
        getIssueKey: function () {
            return AssetsApi.getSelectedIssueKey();
        }
    },

    /**
     * Patch the Asset API with the above methods.
     *
     * @param {object} options
     */
    override: function (options) {
        // We override the target in tests.
        options = _.defaults({}, options, {
            target: JIRA.Issue
        });

        this.searchPageModule = options.searchPageModule;
        _.extend(options.target, patch);
    }
};

module.exports = AssetApi;