"use strict";

var jQuery = require('jquery');

var ctx = jQuery(document);
var Events = {
    /**
     * Subscribes to events
     *
     * @param types
     * @param data
     * @param fn
     */
    bind: function (types, data, fn) {
        ctx.bind(types, data, fn);
    },

    /**
     * Bind to an event once
     *
     * @param evt
     * @param handler
     */
    one: function (evt, handler) {
        ctx.one(evt, handler);
    },

    /**
     * Unbind an event
     *
     * @param evt
     * @param handler
     */
    unbind: function (evt, handler) {
        ctx.unbind(evt, handler);
    },

    /**
     * Publishes events
     *
     * @param evt
     * @param args
     */
    trigger: function (evt, args) {
        ctx.trigger(evt, args);
    }
};

module.exports = Events;
