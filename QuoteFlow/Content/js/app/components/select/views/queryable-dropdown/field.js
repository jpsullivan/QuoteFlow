"use strict";

var $ = require('jquery');
var Marionette = require('backbone.marionette');

var Field = Marionette.ItemView.extend({
    template: function () {
        // Create <input> element in a way that is compatible with IE8 "input" event shim.
        // @see jquery.inputevent.js -- Note #2
        return jQuery("<input>").attr({
            "autocomplete": "off",
            "class": "text",
            "id": this.options.id + "-field",
            "type": "text"
        });
    },

    events: {
        "blur": "handleBlur",
        "click": "handleClick",
        "keydown": "handleKeyPress",
        "keyup": "handleKeyPress"
    },

    triggers: {
        "aui:keydown input": "handleKeyEvent"
    },

    initialize: function (options) {
        this.options = options;
    },

    handleBlur: function() {
        if (!this.ignoreBlurEvent) {
            this.trigger("deactivate");
        } else {
            this.ignoreBlurEvent = false;
        }
    },

    handleClick: function(e) {
        e.stopPropagation();
    },

    handleKeyPress: function(e) {
        if (e.keyCode === 27) {
            this.trigger("handleKeypress", e);
        }
    }
});

module.exports = Field;
