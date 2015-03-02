"use strict";

var Marionette = require('backbone.marionette');

/**
 * This view renders the count of results in a search (e.g "1-20 of 42")
 *
 * @param {object} options Options
 * @param {number} options.total Number of assets in this search
 * @param {number} options.startIndex Index of first asset displayed in the table
 * @param {number} options.pageSize Size of each page
 */
var ResultsCountView = Marionette.ItemView.extend({
    template: JST["quote-builder/results/results-count"],

    serializeData: function () {
        var total = this.options.total;
        var start = this.options.startIndex + 1;
        var end = this.options.startIndex + this.options.pageSize;
        return {
            start: start,
            end: Math.min(end, total),
            total: total
        };
    }
});

module.exports = ResultsCountView;