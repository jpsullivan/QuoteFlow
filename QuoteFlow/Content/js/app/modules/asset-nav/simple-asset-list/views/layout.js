"use strict";

var Marionette = require('backbone.marionette');

var SimpleAssetListLayout = Marionette.LayoutView.extend({
    template: JST["quote-builder/simple-asset-list/layout"],

    regions: {
        pagination: ".pagination-container",
        refresh: ".refresh-container",
        searchResults: ".search-results",
        orderBy: ".list-ordering",
        endOfStableMessageContainer: '.end-of-stable-message-container',
        inlineIssueCreateContainer: '.inline-issue-create-container'
    },

    ui: {
        listPanel: ".list-panel"
    },

    showLoading: function () {
        this.ui.listPanel.addClass("loading");
    },

    hideLoading: function () {
        this.ui.listPanel.removeClass("loading");
    }
});

module.exports = SimpleAssetListLayout;
