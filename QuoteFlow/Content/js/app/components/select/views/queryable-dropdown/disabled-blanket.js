"use strict";

var $ = require('jquery');
var Marionette = require('backbone.marionette');

var DisabledBlanket = Marionette.ItemView.extend({
    template: function () {
        return jQuery("<div class='aui-disabled-blanket' />");
    },

    initialize: function (options) {
        this.options = options;
    },

    onRender: function () {
        this.$el.height(this.options.outerHeight);
    }
});

module.exports = DisabledBlanket;