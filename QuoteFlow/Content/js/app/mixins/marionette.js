"use strict";

var _ = require('underscore');
var Marionette = require('backbone.marionette');

/**
 * A set of mixins that are designed to be used with Marionette.
 * @type {Object}
 */
var MarionetteMixins = {

    /**
     * Listen events from a source and rethrow them. By invoking this method, the current object
     * will listen (using {@link Backbone.Events.listenTo}) to those events and trigger them
     * (using {@link Backbone.Events.trigger}) with all the arguments.
     *
     * In other words, this method is a shorthand for:
     *
     *     this.listenTo(obj, "event", function(arg1, arg2, ...) {
     *         this.trigger("event", arg1, arg2, ...);
     *     }
     *
     * @param {Backbone.Events} source Object that will fire the events
     * @param {string|string[]} events Event or list of events to listen for
     */
    listenAndRethrow: function(source, events) {
        events = [].concat(events);
        _.each(events, function(event) {
            this.listenTo(source, event, function() {
                this.trigger.apply(this, [event].concat(_.toArray(arguments)));
            }, this);
        }, this);
    }
};

_.extend(Marionette.Controller.prototype, MarionetteMixins);

module.exports = MarionetteMixins;
