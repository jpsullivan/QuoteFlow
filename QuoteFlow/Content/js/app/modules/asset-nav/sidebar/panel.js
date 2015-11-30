"use strict";

var _ = require('underscore');
var $ = require('jquery');
var Marionette = require('backbone.marionette');

var EventTypes = require('../util/types');

var QuoteSidebarPanelView = Marionette.ItemView.extend({
    template: JST['quote-builder/sidebar/panel'],

    initialize: function (options) {
        // Resolved when price and line-items are rendered
        this.panelReady = $.Deferred();

        QuoteFlow.bind(EventTypes.LAYOUT_RENDERED, _.bind(function (e, layoutType) {
            if (layoutType) {
                delete this._splitViewSidebarElement;
            } else {
                this._splitViewSidebarElement = $(".list-results-panel:first");
            }
        }, this));
    },

    onRender: function () {
        this.panelReady.resolve();
    }
});

module.exports = QuoteSidebarPanelView;
