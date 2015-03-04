"use strict";

var AssetsApi = require('./assets-api');

var AssetNavigatorApi = {
    searchPageModule: null,

    mixin: {
        /**
         * @return {Boolean} whether issue search is visible.
         */
        isNavigator: function () {
            return searchPageModule.isIssueVisible();
        }
    },

    shortcutsMixin: {
        focusSearch: AssetsApi.focusSearch,
        isNavigator: mixin.isNavigator,
        selectNextIssue: AssetsApi.nextIssue,
        selectPreviousIssue: AssetsApi.prevIssue,
        viewSelectedIssue: AssetsApi.viewSelectedIssue
    },

    /**
     * Override the <tt>JIRA.IssueNavigator</tt> API with the above mixins.
     *
     * @param {object} options
     * @param {SearchPageModule} options.searchPageModule
     */
    override: function (options) {
        this.searchPageModule = options.searchPageModule;

//        var output = _.extend(JIRA.IssueNavigator, this.mixin);
//        output.Shortcuts = _.extend(JIRA.IssueNavigator.Shortcuts, this.shortcutsMixin);
//
//        return output;
    }
};

module.exports = AssetNavigatorApi;