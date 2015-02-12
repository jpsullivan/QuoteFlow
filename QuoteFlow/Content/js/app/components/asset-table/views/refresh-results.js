"use strict";

var Marionette = require('backbone.marionette');

/**
 * This view renders a 'refresh' button.
 *
 * @extends Marionette.ItemView
 */
var RefreshResultsView = Marionette.ItemView.extend({
    tagName: 'span',
    template: JIRA.Templates.IssueNav.refreshResults,
    triggers: {
        /**
         * @event refresh
         * When the user clicks the refresh button
         */
        "click a": "refresh"
    }
});