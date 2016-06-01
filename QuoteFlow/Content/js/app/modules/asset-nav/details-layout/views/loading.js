"use strict";

var Marionette = require('backbone.marionette');

var LoadingView = Marionette.ItemView.extend({
    template: JST["quote-builder/details-layout/loading"],

    onRender: function () {
        this.unwrapTemplate();
    }
});

module.exports = LoadingView;
