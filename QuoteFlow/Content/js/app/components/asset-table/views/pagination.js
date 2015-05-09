"use strict";

var Marionette = require('backbone.marionette');

var AssetTableLayout = require('../views/asset-table-layout');
var AssetTableView = require('../views/asset-table');
var ResultsCountView = require('../views/results-count');

var UrlSerializer = require('../../../util/url-serializer');

/**
 * This view renders a set of pagination links
 *
 * @extends Marionette.ItemView
 *
 * @param {object} options Options
 * @param {string} options.currentSearch Search that produced these search results
 * @param {string} [options.currentSearch.jql] JQL that produced these search results
 * @param {string} [options.currentSearch.filterId] ID of the filter used for this search
 * @param {number} options.pageSize Size of each page
 * @param {number} options.startIndex Index of first asset displayed in the table
 * @param {number} options.total Number of assets in this search
 */
var PaginationView = Marionette.ItemView.extend({
    template: JST["quote-builder/pagination/pagination"],

    events: {
        /**
         * @event goToPage
         * When the user clicks in a pagination link
         */
        "simpleClick a[data-start-index]": function (e) {
            e.preventDefault();
            var val = e.target.getAttribute("data-start-index");
            this.trigger("goToPage", parseInt(val, 10));
        }
    },

    serializeData: function () {
        return {
            startIndex: this.options.startIndex,
            pageSize: this.options.pageSize,
            searchQuery: this._getPagingUri(),
            displayableTotal: this.options.total
        };
    },

    /**
     * Construct a URL for an asset table pagination link.
     *
     * @return {string} the URL.
     */
    _getPagingUri: function () {
        return UrlSerializer.getURLFromState({
            selectedIssueKey: null,
            jql: this.options.currentSearch.jql,
            filter: this.options.currentSearch.filterId
        });
    }
});

module.exports = PaginationView;
