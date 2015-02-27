"use strict";

var Marionette = require('backbone.marionette');

/**
 * 
 */
var AssetTableLayout = Marionette.LayoutView.extend({
    template: JST["quote-builder/split-view/structure"],

    regions: {
        pagination: ".pagination",
        table: ".issue-table-container",
        resultsCount: ".results-count",

        // this region is actually rendered inside 'ResultsCount' view
        refreshResults: {
            selector: ".refresh-table",
            regionType: Marionette.ReplaceRegion
        },
        endOfStableMessage: {
            selector: ".end-of-stable-message",
            regionType: Marionette.ReplaceRegion
        }
    },

    onRender: function () {
        this.hidePending();
    },

    showPending: function () {
        this.$el.addClass('pending');
    },

    hidePending: function () {
        this.$el.removeClass("pending");
    }
});

module.exports = AssetTableLayout;