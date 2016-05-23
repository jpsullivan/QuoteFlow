"use strict";

var Marionette = require('backbone.marionette');

/**
 * Displays a message for the list.
 * @extends Marionette.ItemView
 */
var ListMessage = Marionette.ItemView.extend({
    template: JST["quote-builder/sidebar/list-message"],

    /**
     * Text to display in the message
     * @type {string}
     */
    text: "",

    /**
     * @param {Object} options Options
     * @param {string} options.text Text to display in the message.
     */
    initialize: function (options) {
        this.text = options.text;
    },

    onRender: function () {
        this.unwrapTemplate();
    },

    templateHelpers: function () {
        return {
            text: this.text,
            className: this.className
        };
    }
});

module.exports = ListMessage;
