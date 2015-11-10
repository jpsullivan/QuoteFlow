"use strict";

var _ = require('underscore');
var Backbone = require('backbone');

var ModelUtils = {

    transax: undefined,

    /**
     * A collection of events that *weren't* emitted.
     * @constructor
     */
    EventLog: function () {
        this.modelEvents = {};
    },

    /**
     * Runs the given closure within a "transaction", meaning that all change events are fired atomically AFTER the
     * closure returns. Inside the closure, this==model.
     * <p/>
     * Calling this method when there is a transaction already in progress will make the inner transaction enlist in
     * the outer (all change events will be fired at the end of the outermost transaction).
     *
     * @param closure {function} the code to run in a transaction
     * @param context {object} an optional context to use when calling the closure (defaults to undefined)
     * @return {undefined}
     */
    transaction: function (closure, context) {
        var tx = this.beginTransaction();

        try {
            return closure.apply(context);
        } finally {
            tx.commit();
        }
    },

    /**
     * Makes the methods with the given names transactional. If no method names are provided then any method whose name
     * does not start with _ is made transactional. "Transactional" methods will not raise Backbone change events until
     * the transaction is finished (which happens when the outermost transactional method returns).
     *
     * @param {object} object the instance on which the methods are
     * @return {undefined}
     */
    makeTransactional: function (object) {
        var self = this;
        var transactionalise = function (realMethod) {
            return function () {
                var args = arguments;

                return self.transaction(function () {
                    return realMethod.apply(this, args); // calls the original, non-transactional, method
                }, this);
            };
        };

        var methods = Array.prototype.slice.call(arguments, 1);
        if (methods.length === 0) { methods = _.functions(object); }
        _.each(methods, function(methodName) {
            // skip "private" methods
            if (methodName.indexOf('_') !== 0) {
                object[methodName] = transactionalise(object[methodName]);
            }
        });
    },

    /**
     * A drop-in replacement for Backbone.Events.trigger that captures change[:] events rather
     * than emitting them.
     */
    filteredTrigger: function (name) {
        var model = this;

        if (name === "change" || name.indexOf("change:") === 0) {
            this.transax.log.captureEvent(model, _.toArray(arguments));
        } else {
            Backbone.Events.trigger.apply(model, arguments);
        }
    },

    beginTransaction: function () {
        // if there is a batch already in progress then just piggy-back onto that one. this is
        // necessary so that changes to multiple Backbone models are all published as part of
        // a single batch.
        if (this.transax) {
            return {
                commit: function () {} // the outer batch will publish everything
            };
        }

        this.transax = {
            log: new this.EventLog()
        };

        var restoreTrigger = this.patch(Backbone.Model.prototype, "trigger", _.bind(this.filteredTrigger, this));

        return {
            commit: _.bind(function () {
                restoreTrigger();
                this.transax.log.replayEvents();
                this.transax = null;
            }, this)
        };
    },

    /**
     * Patches an attribute on an object, and provides an easy way to 'undo'.
     *
     * @param {object} object
     * @param {string} name The attribute to patch
     * @param {object} replacement
     * @return {Function} A 'restore' function that reverts the patch.
     */
    patch: function (object, name, replacement) {
        var hadOriginal = object.hasOwnProperty(name),
            original = object[name];

        object[name] = replacement;

        return function restore() {
            if (hadOriginal) {
                object[name] = original;
            } else {
                delete object[name];
            }
        };
    }
};

/**
 * Record the details of an event, so it can be replayed later.
 *
 * @param {Backbone.Model} model
 * @param {*[]} args
 */
ModelUtils.EventLog.prototype.captureEvent = function (model, args) {
    var name = args[0];

    // We only want to trigger each type of event at most once, so they're keyed on their name.
    this.modelEvents[model.cid] || (this.modelEvents[model.cid] = {
        model: model,
        events: {}
    });

    this.modelEvents[model.cid].events[name] = args;
};

/**
 * Replay all the events that were captured.
 */
ModelUtils.EventLog.prototype.replayEvents = function () {
    _.each(this.modelEvents, function (item) {
        _.each(item.events, function (args, name) {
            Backbone.Events.trigger.apply(item.model, args);
        });
    });
};

module.exports = ModelUtils;
