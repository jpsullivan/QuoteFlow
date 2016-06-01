"use strict";

var Marionette = require('backbone.marionette');

var ContainerView = Marionette.ItemView.extend({
    template: JST["quote-builder/details-layout/container"],

    onRender: function () {
        this.unwrapTemplate();
    },

    showView: function (view) {
        this.$el.append(view.$el);
    },

    showLoading: function (loadingView) {
        this.$el.prepend(loadingView.$el);
    },

    hideLoading: function (loadingView) {
        loadingView.$el.detach();
    }
});

module.exports = ContainerView;
