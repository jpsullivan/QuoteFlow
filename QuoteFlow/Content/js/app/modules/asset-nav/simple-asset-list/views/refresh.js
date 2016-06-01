"use strict";

var Marionette = require('backbone.marionette');

var RefreshView = Marionette.ItemView.extend({
    template: JST["quote-builder/simple-asset-list/refresh"],
    triggers: {
        "click": "refresh"
    },

    onRender: function () {
        this.unwrapTemplate();
    }
});

module.exports = RefreshView;
