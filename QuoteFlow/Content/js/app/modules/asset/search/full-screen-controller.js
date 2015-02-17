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
        this.issueTable = new IssueTable({
            searchService: this.searchService,
            el: this.$navigatorContent,
            columnConfig: options.columnConfig
        });

        this.listenTo(this.issueTable, {
            "highlightIssue": function (issueId) {
                this.searchService.highlightIssue(issueId);
            },
            "render": function () {
                if (!this.searchService.hasSelectedIssue()) {
                    this.fullScreenIssue.hide();
                }
                this.fullScreenIssue.bindSearchService(this.searchService);
                this.trigger("render");
            }
        });

        this.fullScreenIssue = options.fullScreenIssue;

        this.listenTo(this.fullScreenIssue, {
            "assetHidden": function () {
                // This is the second highlight. The first one is inside IssueTable component, but due the
                // internals of FullScreenIssue, when the first one is fired the IssueTable is not in the DOM
                // so the scrollIntoView() operation will not work. We need to re-highlight the same issue now
                // that the IssueTable is present in the DOM to force the scroll behaviour
                this.issueTable.highlightIssue(this.searchService.getHighlightedIssue());
            }
        });

        Application.on("assetEditor:loadError", this.onLoadError, this);
    },

    onLoadError: function (issue) {
        if (!this.fullScreenIssue.isVisible()) {
            this.searchService.unselectIssue();
            Messages.showErrorMsg(
                AJS.I18n.getText('viewissue.error.message.cannotopen', issue.issueKey),
                { closeable: true }
            );
        }
    },

    render: function () {
        this.issueTable.show();
    },

    onClose: function () {
        this.fullScreenIssue.deactivate();
        this.issueTable.close();
        this.searchService.close();

        Application.off("issueEditor:loadError", this.onLoadError, this);

        delete this.fullScreenIssue;
        delete this.issueTable;
        delete this.searchService;
    },

    nextIssue: function () {
        this.searchService.selectNextIssue();
    },

    prevIssue: function () {
        this.searchService.selectPreviousIssue();
    },

    returnToSearch: function () {
        this.searchService.unselectIssue();
    },

    handleLeft: function () {
        // No-op
    },

    handleRight: function () {
        // No-op
    },

    isIssueViewActive: function () {
        return this.fullScreenIssue.isVisible();
    }
});

module.exports = FullScreenLayoutController;