"use strict";

var Marionette = require('backbone.marionette');
var ScrollIntoView = require('jquery-scroll-into-view');

var AssetView = Marionette.ItemView.extend({
    template: JST["quote-builder/simple-asset-list/asset"],

    ui: {
        link: "a"
    },

    events: {
        'simpleClick': function (ev) {
            ev.preventDefault();
            this.trigger('select');
        }
    },

    serializeData: function () {
        var model = this.model;
        return {
            "id": model.get("id"),
            "sku": model.get("sku"),
            "summary": model.get("summary")
        };
    },

    onRender: function () {
        this.unwrapTemplate();
    },

    highlight: function () {
        this.$el.addClass("focused");
        this.ui.link.focus();
        this.scrollIntoView();
    },

    unhighlight: function () {
        this.$el.removeClass("focused");
    },

    scrollIntoView: function () {
        this.$el.scrollIntoViewForAuto();
    }
});

module.exports = AssetView;
