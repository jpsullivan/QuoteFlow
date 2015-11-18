"use strict";

var Marionette = require('backbone.marionette');

var AssetTableLayout = require('../views/asset-table-layout');
var AssetTableView = require('../views/asset-table');
var EmptyResultsView = require('../views/empty-results');
var EndOfStableMessage = require('../views/end-of-stable-message');
var PaginationView = require('../views/pagination');
var RefreshResultsView = require('../views/refresh-results');
var ResultsCountView = require('../views/results-count');

/**
 *
 */
var AssetTableController = Marionette.Controller.extend({
    /**
     * @event columnsChanged
     * When the order of the columns has changed
     */

    /**
     * @event goToPage
     * When the user wants to load another page by clicking on the pager
     */

    /**
     * @event highlightAsset
     * When an asset has been highlighted.
     */

    /**
     * @event assetRowUpdated
     * When a asset in the table has been updated with new information
     */

    /**
     * @event refresh
     * When the user wants to refresh the search by clicking on the 'refresh' icon
     */

    /**
     * @event renderTable
     * When the table with results and the associated internal views have been rendered
     */

    /**
     * @event sort
     * When the results has been sorted by clicking on a column's header
     */

    /**
     * @param {Object} options Initialization options
     * @param {Element} options.el Element where the table should be rendered
     */
    initialize: function (options) {
        this.$el = jQuery(options.el);
    },

    /**
     * @returns {AssetTableLayout}
     */
    _createMainView: function () {
        return new AssetTableLayout();
    },

    /**
     * @param {jQuery} resultsTable Server-side rendered table with the results
     * @param {Object} sortOptions State of the current sort options
     * @param {string} sortOptions.fieldId ID of the field used for sorting the results
     * @param {string} sortOptions.order Direction used for the sorting ("DESC", "ASC")
     * @returns {AssetTableView}
     */
    _createTableView: function (resultsTable, sortOptions) {
        var table = new AssetTableView({
            resultsTable: resultsTable,
            sortOptions: sortOptions
        });
        this.listenAndRethrow(table, "columnsChanged");
        this.listenAndRethrow(table, "highlightAsset");
        this.listenAndRethrow(table, "assetRowUpdated");
        this.listenAndRethrow(table, "sort");
        return table;
    },

    /**
     * @param {object} data Data to use in this view
     * @param {number} data.total Number of assets in this search
     * @param {number} data.startIndex Index of first asset displayed in the table
     * @param {number} data.pageSize Size of each page
     * @returns {ResultsCountView}
     * @private
     */
    _createResultsCount: function (data) {
        return new ResultsCountView(data);
    },

    /**
     * @param {object} data Data to use in this view
     * @param {number} data.total Number of assets in this search
     * @param {number} data.startIndex Index of first asset displayed in the table
     * @param {number} data.pageSize Size of each page
     * @param {string} data.currentSearch JQL that produced this search results
     * @returns {PaginationView}
     * @private
     */
    _createPagination: function (data) {
        var pagination = new PaginationView(data);
        this.listenAndRethrow(pagination, "goToPage");
        return pagination;
    },

    /**
     * @returns {RefreshResultsView}
     */
    _createRefreshResults: function () {
        var refreshResults = new RefreshResultsView();
        this.listenAndRethrow(refreshResults, "refresh");
        return refreshResults;
    },

    /**
     * @param {object} data Data to use in this view
     * @param {number} data.total Number of asses in this search
     * @param {number} data.displayableTotal Number of assets than can be displayed in a stable search
     * @param {number} data.pageNumber Number of the current page
     * @param {number} data.numberOfPages Total number of pages in the search results
     * @returns {EndOfStableMessage}
     */
    _createEndOfStableMessage: function (data) {
        return new EndOfStableMessage(data);
    },

    /**
     * @param {object} data Data to use in this view
     * @param {boolean} data.quoteflowHasAssets Whether there are assets created in this QuoteFlow instance
     * @returns {EmptyResultsView}
     */
    _createEmptyResultsView: function (data) {
        return new EmptyResultsView(data);
    },

    _layoutIsRendered: function () {
        return this.view && this.view instanceof AssetTableLayout;
    },

    destroy: function () {
        if (this.view) {
            this.view.destroy();
            delete this.view;
        }
    },

    /**
     * @param {object} options Options
     * @param {jQuery} options.table Server-side rendered table with the results
     * @param {Object} options.sortOptions State of the current sort options
     * @param {string} options.sortOptions.fieldId ID of the field used for sorting the results
     * @param {string} options.sortOptions.order Direction used for the sorting ("DESC", "ASC")
     * @param {number} options.totalAssets Number of assets in this search
     * @param {number} options.displayableTotal Number of assets than can be displayed in a stable search
     * @param {number} options.pageNumber Number of the current page
     * @param {number} options.numberOfPages Total number of pages in the search results
     * @param {number} options.startIndex Index of first asset displayed in the table
     * @param {number} options.pageSize Size of each page
     * @param {string} options.currentSearch JQL that produced this search results
     * @param {boolean} options.quoteflowHasAssets Whether there are assets created in this QuoteFlow instance
     * @param {boolean} options.hasAssets Whether there asset result has assets
     */
    show: function (options) {
        if (options.hasAssets) {
            this._showTable(options);
        } else {
            this._showEmptyResults(options);
        }
    },

    /**
     * @param {object} options Options
     * @param {boolean} options.quoteflowHasAssets Whether there are assets created in this QuoteFlow instance
     */
    showErrorMessage: function (options) {
        this._showEmptyResults(options);
    },

    /**
     * Puts the view in the pending state (i.e. dims the table). This method also temporarily disables
     * the column reordering feature.
     */
    showPending: function () {
        if (this.view) {
            this.view.showPending();
            if (this._layoutIsRendered()) {
                this.view.table.currentView._removeDraggable();
            }
        }
    },

    /**
     * @param {object} options Options
     * @param {jQuery} options.table Server-side rendered table with the results
     * @param {Object} options.sortOptions State of the current sort options
     * @param {string} options.sortOptions.fieldId ID of the field used for sorting the results
     * @param {string} options.sortOptions.order Direction used for the sorting ("DESC", "ASC")
     * @param {number} options.totalAssets Number of assets in this search
     * @param {number} options.totalDisplayableAssets Number of assets than can be displayed in a stable search
     * @param {number} options.pageNumber Number of the current page
     * @param {number} options.numberOfPages Total number of pages in the search results
     * @param {number} options.startIndex Index of first asset displayed in the table
     * @param {number} options.pageSize Size of each page
     * @param {string} options.currentSearch JQL that produced this search results
     */
    _showTable: function (options) {
        options = options || {};

        this.destroy();
        this.view = this._createMainView();
        var table = this._createTableView(options.table, options.sortOptions);
        var resultsCount = this._createResultsCount({
            total: options.totalAssets,
            startIndex: options.startIndex,
            pageSize: options.pageSize
        });
        var pagination = this._createPagination({
            startIndex: options.startIndex,
            pageSize: options.pageSize,
            total: options.totalAssets,
            currentSearch: options.currentSearch
        });
        var refreshResults = this._createRefreshResults();
        var endOfStableMessage = this._createEndOfStableMessage({
            total: options.totalAssets,
            displayableTotal: options.totalDisplayableAssets,
            pageNumber: options.pageNumber,
            numberOfPages: options.numberOfPages
        });

        this.listenTo(this.view, {
            "render": function () {
                this.view.table.show(table);
                this.view.resultsCount.show(resultsCount);
                this.view.pagination.show(pagination);
                this.view.refreshResults.show(refreshResults);
                this.view.endOfStableMessage.show(endOfStableMessage);
            }
        });

        this.view.render();
        this.$el.empty().append(this.view.$el);
        this.trigger("renderTable", this.$el);
    },

    /**
     * @param {object} options Options
     * @param {boolean} options.quoteflowHasAssets Whether there are assets created in this QuoteFlow instance
     */
    _showEmptyResults: function (options) {
        this.destroy();
        this.view = this._createEmptyResultsView(options);
        this.view.render();
        this.$el.empty().append(this.view.$el);
        this.trigger("renderEmpty", this.$el);
    },

    /**
     * Highlight an asset in the table.
     *
     * @param {number} assetId The ID of the asset to highlight.
     * @param {boolean} [focus=true] Whether the highlighted asset should have the focus
     */
    highlightAsset: function (assetId, focus) {
        if (this._layoutIsRendered()) {
            this.view.table.currentView.highlightAsset(assetId, focus);
        }
    },

    /**
     * Updates an asset in the table with a new row
     *
     * @param {number} assetId of the asset to update
     * @param {jQuery} newTable Server rendered table with the new data
     */
    updateAsset: function (assetId, newTable) {
        if (this._layoutIsRendered()) {
            this.view.table.currentView.updateAsset(assetId, newTable);
        }
    }
});

module.exports = AssetTableController;
