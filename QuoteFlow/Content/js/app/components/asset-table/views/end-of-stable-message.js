"use strict";

var Marionette = require('backbone.marionette');

/**
 * Renders the end of stable message.
 *
 * @extends Marionette.ItemView
 *
 * TODO: This logic for deciding to show this message or not, can be moved to the controller.
 *
 * @param {object} options Options
 * @param {number} options.total Number of assets in this search
 * @param {number} options.displayableTotal Number of assets than can be displayed in a stable search
 * @param {number} options.pageNumber Number of the current page
 * @param {number} options.numberOfPages Total number of pages in the search results
 */
var EndOfStableMessage = Marionette.ItemView.extend({
    className: "end-of-stable-message",

    serializeData: function () {
        return this.options;
    },

    getTemplate: function () {
        return (this.shouldRender) ? JIRA.Templates.IssueNav.endOfStableMessage : jQuery.noop;
    },

    onBeforeRender: function () {
        this.shouldRender = this._hasMoreIssues() && this._onLastPage();
    },

    onRender: function () {
        if (this.shouldRender) {
            this.$el.addClass("visible");
        } else {
            this.$el.removeClass("visible");
        }
    },

    _hasMoreIssues: function () {
        return !!(this.options.total - this.options.displayableTotal);
    },

    _onLastPage: function () {
        return this.options.pageNumber === this.options.numberOfPages;
    }
});

module.exports = EndOfStableMessage;