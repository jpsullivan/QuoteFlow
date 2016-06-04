"use strict";

var Marionette = require('backbone.marionette');

var ToolsView = Marionette.LayoutView.extend({
    initialize: function (options) {
        this.showExpand = options.showExpand;
    },

    template: JST["quote-builder/details-layout/tools"],

    regions: {
        pager: ".pager-container"
    },

    ui: {
        expand: ".expand"
    },

    triggers: {
        "click @ui.expand": "expand"
    },

    serializeData: function () {
        return {
            showExpand: this.showExpand || false,
            expandShortcutKey: "z"
        };
    },

    onRender: function () {
        if (this.showExpand) {
            this.ui.expand.tooltip({
                gravity: 'e'
            });
        }
    }
});

module.exports = ToolsView;
