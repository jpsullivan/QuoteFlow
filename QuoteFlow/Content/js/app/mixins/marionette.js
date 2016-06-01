"use strict";

var _ = require('underscore');
var Marionette = require('backbone.marionette');

/**
 * Triggers a preventable event.
 * The event will include an EventObject, and the handler can call eventObject.preventDefault() to prevent the event.
 * @param {string} eventName Name of the event being triggered.
 * @param {EventObject} [eventObject] EventObject used as template to construct the actual EventObject used in the event.
 * @return {EventObject} EventObject passed to the event.
 */
var triggerPreventable = function (eventName, eventObject) {
    /**
     * EventObject passed to preventable events
     * @typedef {Object} EventObject
     * @property {Object} emitter Original emitter of the event.
     * @property {boolean} isPrevented Whether the event has been prevented by the event handler.
     * @property {Function} preventDefault Syntax sugar for set the `isPrevented` value.
     */
    var event = _.defaults({}, eventObject || {}, {
        isPrevented: false,
        emitter: this,
        preventDefault: function () {
            this.isPrevented = true;
        }
    });

    this.trigger(eventName, event);
    return event;
};

var retriggerPreventable = function (eventName, eventObject) {
    var groupEvent = this.triggerPreventable(eventName, eventObject);
    if (groupEvent.isPrevented) {
        eventObject.preventDefault();
    }
};

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
        listenAndRethrow: function (source, events) {
            events = [].concat(events);
            _.each(events, function (event) {
                this.listenTo(source, event, function () {
                    this.trigger.apply(this, [event].concat(_.toArray(arguments)));
                }, this);
            }, this);
        },

        triggerPreventable: triggerPreventable,
        retriggerPreventable: retriggerPreventable
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
        addListener: function (other, event, callback, context) {
            if (arguments.length < 3) {
                throw "The 'other', 'event', and 'callback' arguments are mandatory";
            }

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
                if (typeof other[finalRegisterName] !== 'function') {
                    throw "object does not have method " + registerMethodName + "'";
                }
                // if (typeof other[finalUnRegisterName] !== 'function') { throw "object does not have method " + unregisterMethodName + "'"; }
            } else {
                // listener for brace events
                finalRegisterName = braceRegisterMethodName;
                finalUnRegisterName = braceUnregisterMethodName;
                if (typeof other[finalRegisterName] !== 'function') {
                    throw "object does not have event [" + event + "] registered'";
                }
            }

            // register using the listen method and add to the list so we can clean up in removeListeners()
            other[finalRegisterName](callback, context);
            this._cleanerUppers = this._cleanerUppers || [];
            this._cleanerUppers.push(function () {
                other[finalUnRegisterName](callback, context);
            });
        },

        /**
         * Removes all listeners added using <code>addListener()</code>.
         */
        removeListeners: function () {
            if (this._cleanerUppers) {
                _.each(this._cleanerUppers, function (cleanerUpper) {
                    cleanerUpper(); // un-register the listener
                });
            }
        },

        applyToDom: function () {
            this.delegateEvents();
            this.bindUIElements();
            this.triggerMethod("applyToDom");
        },

        /**
         * This method unwraps the Backbone.View.
         *
         * By default, Backbone will create a <div> and render the template inside. By calling this
         * method, you can get rid of that <div>, so the main element in your template will be the
         * root element in your template.
         */
        unwrapTemplate: function () {
            if (this.$el.parent().length) {
                // If the template is already rendered in the page
                var children = this.$el.children();
                this.$el.replaceWith(children);
                this.setElement(children);
            } else {
                // If the template is in memory
                this.setElement(this.$el.children());
            }
        },

        triggerPreventable: triggerPreventable,
        retriggerPreventable: retriggerPreventable
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
