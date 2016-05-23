"use strict";

var Marionette = require('backbone.marionette');

/**
 * A view containing the entire search/asset app.
 *
 * Handles switching between the search and asset views.
 */
var FullScreenLayoutController = Marionette.Controller.extend({

    /**
     * Initialise the FullScreenLayout.
     *
     * @param {object} options
     * @param {element} options.searchContainer The element into which the search is to be rendered.
     */
    initialize: function (options) {
        this.searchService = new SearchService({
            searchModule: options.search,
            searchResults: options.search.getResults(),
            columnConfig: options.columnConfig
        });

        this.$navigatorContent = options.searchContainer.find('.navigator-content');
        this.assetTable = new IssueTable({
            searchService: this.searchService,
            el: this.$navigatorContent,
            columnConfig: options.columnConfig
        });

        this.listenTo(this.assetTable, {
            "highlightIssue": function (assetId) {
                this.searchService.highlightAsset(assetId);
            },
            "render": function () {
                if (!this.searchService.hasSelectedAsset()) {
                    this.fullScreenAsset.hide();
                }
                this.fullScreenAsset.bindSearchService(this.searchService);
                this.trigger("render");
            }
        });

        this.fullScreenAsset = options.fullScreenAsset;

        this.listenTo(this.fullScreenAsset, {
            "assetHidden": function () {
                // This is the second highlight. The first one is inside IssueTable component, but due the
                // internals of FullScreenIssue, when the first one is fired the IssueTable is not in the DOM
                // so the scrollIntoView() operation will not work. We need to re-highlight the same issue now
                // that the IssueTable is present in the DOM to force the scroll behaviour
                this.assetTable.highlightIssue(this.searchService.getHighlightedAsset());
            }
        });

        Application.on("assetEditor:loadError", this.onLoadError, this);
    },

    onLoadError: function (issue) {
        if (!this.fullScreenAsset.isVisible()) {
            this.searchService.unselectAsset();
            Messages.showErrorMsg(
                AJS.I18n.getText('viewissue.error.message.cannotopen', issue.issueKey),
                { closeable: true }
            );
        }
    },

    render: function () {
        this.assetTable.show();
    },

    onDestroy: function () {
        // this.fullScreenAsset.deactivate();
        this.fullScreenAsset.destroy();
        this.assetTable.destroy();
        this.searchService.destroy();

        Application.off("assetEditor:loadError", this.onLoadError, this);

        delete this.fullScreenAsset;
        delete this.assetTable;
        delete this.searchService;
    },

    nextAsset: function () {
        this.searchService.selectNextAsset();
    },

    prevAsset: function () {
        this.searchService.selectPreviousIssue();
    },

    returnToSearch: function () {
        this.searchService.unselectAsset();
    },

    handleLeft: function () {
        // No-op
    },

    handleRight: function () {
        // No-op
    },

    isIssueViewActive: function () {
        return this.fullScreenAsset.isVisible();
    }
});

module.exports = FullScreenLayoutController;
