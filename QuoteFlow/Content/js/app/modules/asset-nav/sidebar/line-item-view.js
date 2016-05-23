"use strict";

var Marionette = require('backbone.marionette');

/**
 * Renders each individual line item in the quote sidebar list.
 * @extends Marionette.ItemView
 */
var LineItemView = Marionette.ItemView.extend({
    template: JST["quote-builder/sidebar/line-item"],

    ui: {
        filterLink: ".filter-link"
    },

    triggers: {
        "simpleClick .filter-link": "selectFilter"
    },

    modelEvents: {
        "change": "render"
    },

    onRender: function () {
        this.unwrapTemplate();
    },

    highlight: function () {
        this.ui.filterLink.addClass("active");
        this.ui.filterLink.scrollIntoView();
    },

    unhighlight: function () {
        this.ui.filterLink.removeClass("active");
    }
});

module.exports = LineItemView;
