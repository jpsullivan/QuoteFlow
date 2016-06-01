"use strict";

var Marionette = require('backbone.marionette');

var EmptyView = Marionette.ItemView.extend({
    template: JST["quote-builder/details-layout/empty"],

    onRender: function () {
        this.unwrapTemplate();
    }
});

module.exports = EmptyView;
