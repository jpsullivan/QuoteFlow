"use strict";

var $ = require('jquery');
var Marionette = require('backbone.marionette');

var Overlabel = Marionette.ItemView.extend({
    template: function () {
        return jQuery("<span id='" + this.options.id + "-overlabel' data-target='" + this.options.id + "-field' class='overlabel'>" + this.options.overlabel + "</span>");
    },

    initialize: function(options) {
        this.options = options;
    }
});

module.exports = Overlabel;