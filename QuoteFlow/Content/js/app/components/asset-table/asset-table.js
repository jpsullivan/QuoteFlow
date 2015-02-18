"use strict";

var Marionette = require('backbone.marionette');

var AssetTableController = require('./controllers/asset-table');
var TableView = require('../table/asset-table-view');

/**
 * 
 */
var AssetTable = Marionette.Controller.extend({

    initialize: function (options) {
        this.assetTableView = new TableView({
            columnConfig: options.columnConfig
        });
        this._createSearchService(options);
        this._createTableController(options);

        //JIRA.Issues.Application.on("issueEditor:loadError", this._handleIssueLoadError, this);
    },

    _createSearchService: function (options) {
        this.searchService = options.searchService;
        this.listenTo(this.searchService, {
            "before:search": function () {
                this.assetTableController.showPending();
            },
            "search": function (table, searchResults) {
                this.latestResults = {
                    table: table,
                    sortOptions: searchResults.getSortBy(),
                    totalDisplayableAssets: searchResults.getDisplayableTotal(),
                    startIndex: searchResults.getStartIndex(),
                    pageSize: searchResults.getPageSize(),
                    pageNumber: searchResults.getPageNumber(),
                    numberOfPages: searchResults.getNumberOfPages(),
                    totalAssets: searchResults.getTotal(),
                    currentSearch: JIRA.Issues.Application.request("issueNav:currentSearchRequest"),
                    quoteflowHasAssets: searchResults.getQuoteflowHasAssets(),
                    hasIssues: searchResults.hasAssets()
                };
                this.show();
                this.highlightAsset(searchResults.getHighlightedAsset().id, false);
            },
            "error:search": function () {
                this.assetTableController.showErrorMessage();
            },
            "assetUpdated": function (assetId, entity) {
                this.assetTableController.updateAsset(assetId, entity.table);
            },
            "assetHighlighted": function (assetId) {
                this.assetTableController.highlightAsset(assetId);
            },
            "selectedAssetChanged": function (selectedAsset, highlightedAsset) {
                if (!selectedAsset.hasAsset()) {
                    this.assetTableController.highlightAsset(highlightedAsset.id);
                }
            }

        });
    },

    _createTableController: function (options) {
        var columnConfig = options.columnConfig;
        var el = options.el;

        this.assetTableController = new AssetTableController({
            el: el
        });

        this.listenTo(this.assetTableController, {
            "goToPage": function (startIndex) {
                this.searchService.goToPage(startIndex);
            },
            "columnsChanged": function (cols) {
                columnConfig.saveColumns(cols);
            },
            "highlightAsset": function (assetId) {
                this.trigger("highlightAsset", assetId);
            },
            "sort": function (fieldId) {
                this.searchService.sort(fieldId);
            },
            "refresh": function () {
                this.searchService.runCurrentSearch();
            },
            "renderTable": function ($el) {
                JIRA.trigger(JIRA.Events.NEW_CONTENT_ADDED, [$el, JIRA.CONTENT_ADDED_REASON.issueTableRefreshed]);
                JIRA.trace("jira.search.stable.update");
            },
            "renderEmpty": function ($el) {
                JIRA.trigger(JIRA.Events.NEW_CONTENT_ADDED, [$el, JIRA.CONTENT_ADDED_REASON.issueTableRefreshed]);
                JIRA.trace("jira.search.stable.update");
            },
            "assetRowUpdated": function ($newRow) {
                JIRA.trigger(JIRA.Events.NEW_CONTENT_ADDED, [$newRow, JIRA.CONTENT_ADDED_REASON.issueTableRowRefreshed]);
                // even though we've only replaced one row, a search has occurred at this point. all our
                // WebDriver tests expect this trace after modifying issues from the issue nav
                JIRA.trace('jira.search.finished');
            }
        });
    },

    show: function () {
        if (!this.latestResults) {
            this.searchService.updateExitingResults();
        } else {
            this.assetTableController.show(this.latestResults);
            this.trigger("render");
            this.assetTableView._onSearchDone(this.assetTableController.view.$el);
        }
    },

    close: function () {
        this.assetTableController.close();
        this.stopListening(this.searchService);
        //JIRA.Issues.Application.off("issueEditor:loadError", this._handleIssueLoadError, this);
        delete this.assetTableController;
        delete this.searchService;
    },

    /**
     * Highlight an asset in the table.
     *
     * @param {number} assetId The ID of the asset to highlight.
     * @param {boolean} [focus=true] Whether the highlighted asset should have the focus
     */
    highlightAsset: function (assetId, focus) {
        this.assetTableController.highlightAsset(assetId, focus);
    },

    _handleAssetLoadError: function (entity) {
        // If the asset has been deleted, update its row in the table.
        if (entity.response.status === 404) {
            this.assetTableController.markIssueAsInaccessible(entity.issueId);
        }
    }
});

module.exports = AssetTable;