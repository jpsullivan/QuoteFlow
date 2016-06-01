"use strict";

var Marionette = require('backbone.marionette');

var LoadingView = Marionette.ItemView.extend({
    template: JST["quote-builder/simple-asset-list/loading"],

    onRender: function () {
        this.unwrapTemplate();
    }
});

module.exports = LoadingView;
