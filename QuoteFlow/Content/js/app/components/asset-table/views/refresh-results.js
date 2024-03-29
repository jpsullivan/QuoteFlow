﻿"use strict";

var Marionette = require('backbone.marionette');

/**
 * This view renders a 'refresh' button.
 *
 * @extends Marionette.ItemView
 */
var RefreshResultsView = Marionette.ItemView.extend({
    tagName: 'span',
    template: JST["quote-builder/results/refresh-results"],
    triggers: {
        /**
         * @event refresh
         * When the user clicks the refresh button
         */
        "click a": "refresh"
    }
});

module.exports = RefreshResultsView;
