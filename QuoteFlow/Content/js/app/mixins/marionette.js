"use strict";

var _ = require('underscore');
var Marionette = require('backbone.marionette');

/**
 * A set of mixins that are designed to be used with Marionette.
 * @type {Object}
 */
var MarionetteMixins = {

    controllerMixins: {
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
    },

    viewMixins: {
        /**
         * Calls <code>other.onEvent(callback, context)</code> and adds the callback to the list of listeners so it can
         * be removed later by calling <code>removeListeners()</code>.
         *
         * @param other the object to call onEvent on
         * @param event the name of the event (used to determine the method name)
         * @param callback the callback
         * @param context an optional context to use when running the callback
         */
        addListener: function(other, event, callback, context) {
            if (arguments.length < 3) { throw "The 'other', 'event', and 'callback' arguments are mandatory"; }

            var capitalisedEvent = event.charAt(0).toUpperCase() + event.slice(1);
            var registerMethodName = "on" + capitalisedEvent;
            var unregisterMethodName = "off" + capitalisedEvent;
            var braceRegisterMethodName = "bind" + capitalisedEvent;
            var braceUnregisterMethodName = "unbind" + capitalisedEvent;
            var finalRegisterName;
            var finalUnRegisterName;

            if (other[registerMethodName]) {
                // listener for methods
                finalRegisterName = registerMethodName;
                finalUnRegisterName = unregisterMethodName || braceUnregisterMethodName;
                if (typeof other[finalRegisterName] !== 'function') { throw "object does not have method " + registerMethodName + "'"; }
                //if (typeof other[finalUnRegisterName] !== 'function') { throw "object does not have method " + unregisterMethodName + "'"; }
            } else {
                // listener for brace events
                finalRegisterName = braceRegisterMethodName;
                finalUnRegisterName = braceUnregisterMethodName;
                if (typeof other[finalRegisterName] !== 'function') { throw "object does not have event [" + event + "] registered'"; }
            }

            // register using the listen method and add to the list so we can clean up in removeListeners()
            other[finalRegisterName](callback, context);
            this._cleanerUppers = this._cleanerUppers || [];
            this._cleanerUppers.push(function() { other[finalUnRegisterName](callback, context); });
        },

        /**
         * Removes all listeners added using <code>addListener()</code>.
         */
        removeListeners: function() {
            if (this._cleanerUppers) {
                _.each(this._cleanerUppers, function(cleanerUpper) {
                    cleanerUpper(); // un-register the listener
                });
            }
        },

        applyToDom: function () {
            this.delegateEvents();
            this.bindUIElements();
            this.triggerMethod("applyToDom");
        }
    },

    layoutMixins: {
        applyToDom: function () {
            this.delegateEvents();
            this.bindUIElements();
            this._reInitializeRegions();
            this.triggerMethod("applyToDom");
        }
    }
};

_.extend(Marionette.Controller.prototype, MarionetteMixins.controllerMixins);
_.extend(Marionette.View.prototype, MarionetteMixins.viewMixins);
_.extend(Marionette.LayoutView.prototype, MarionetteMixins.layoutMixins);

module.exports = MarionetteMixins;
