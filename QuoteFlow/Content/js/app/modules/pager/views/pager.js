"use strict";

var Marionette = require('backbone.marionette');

var PagerView = Marionette.ItemView.extend({
    getTemplate: function () {
        if (this.searchResults && this.searchResults.selected) {
            return JST["asset-viewer/asset/pager"];
        }
        return false;
    },

    ui: {
        "next": ".next a",
        "previous": ".previous a"
    },

    triggers: {
        "simpleClick @ui.next": "next",
        "simpleClick @ui.previous": "previous"
    },

    update: function (searchResults) {
        this.searchResults = searchResults;
        // Backbone usually delegates DOM events when the View is constructed, not when it is rendered.
        // As we have variable templates (i.e. the first time we render the view the template is likely
        // to be undefined), we need to re-delegate the events every time we render.
        this.undelegateEvents();
        this.render();
        this.delegateEvents();
    },

    onRender: function () {
        if (AJS.activeShortcuts) {
            if (AJS.activeShortcuts.j) { AJS.activeShortcuts.j._addShortcutTitle(this.$el.find(this.ui.nextIssue));}
            if (AJS.activeShortcuts.k) { AJS.activeShortcuts.k._addShortcutTitle(this.$el.find(this.ui.previousIssue));}
        }
    },

    serializeData: function () {
        var model = this.searchResults;

        var selected = model.selected;
        if (!selected) {
            return;
        }

        var currentIssuePosition = this.searchResults.getPositionOfIssueInSearchResults(this.searchResults.selected.id);
        var templateData = {
            position: currentIssuePosition + 1,
            resultCount: this.searchResults.getTotalIssues()
        };

        var nextIssue = this.searchResults.getIssueAtGlobalIndex(currentIssuePosition + 1);
        if (nextIssue) {
            templateData.nextIssue = {
                id: nextIssue.id,
                key: nextIssue.key
            };
        }

        var previousIssue = this.searchResults.getIssueAtGlobalIndex(currentIssuePosition - 1);
        if (previousIssue) {
            templateData.previousIssue = {
                id: previousIssue.id,
                sku: previousIssue.sku
            };
        }

        return templateData;
    }
});

module.exports = PagerView;
