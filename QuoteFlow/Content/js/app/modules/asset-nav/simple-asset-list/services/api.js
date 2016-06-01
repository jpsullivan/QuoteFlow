"use strict";

var AssetsApi = {
    init: function (simpleAssetList) {
        JIRA.API.Issues.nextIssue = function () {
            simpleAssetList.selectNext();
        };

        JIRA.API.Issues.previousIssue = function () {
            simpleAssetList.selectPrevious();
        };
    }
};

module.exports = AssetsApi;
