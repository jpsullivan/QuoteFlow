"use strict";

var _ = require('underscore');
var jQuery = require('jquery');

var RecurringPromise = function (parent) {
    if (!(this instanceof RecurringPromise)) {
        return new RecurringPromise();
    }

    this._parent = parent;
    this._doneCallbacks = jQuery.Callbacks();
    this._failCallbacks = jQuery.Callbacks();
    this._alwaysCallbacks = jQuery.Callbacks();
};

RecurringPromise.prototype = {
    add: function (deferred) {
        var wrapperDeferred = jQuery.Deferred();
        wrapperDeferred.original = deferred;

        var pending = this._getPending();
        this._abortIfPending(pending);

        this._setPending(wrapperDeferred);

        deferred.done(_.bind(function () {
            if (wrapperDeferred === this._getPending()) {
                wrapperDeferred.resolveWith(this, arguments);
                this._doneCallbacks.fireWith(this, arguments);
            }
        }, this));
        deferred.fail(_.bind(function () {
            if (wrapperDeferred === this._getPending()) {
                wrapperDeferred.rejectWith(this, arguments);
                this._failCallbacks.fireWith(this, arguments);
            }
        }, this));
        deferred.always(_.bind(function () {
            if (wrapperDeferred === this._getPending()) {
                this._alwaysCallbacks.fireWith(this, arguments);
            }
        }, this));

        return wrapperDeferred.promise();
    },

    done: function (callback) {
        this._doneCallbacks.add(callback);
        return this;
    },

    fail: function (callback) {
        this._failCallbacks.add(callback);
        return this;
    },

    always: function (callback) {
        this._alwaysCallbacks.add(callback);
        return this;
    },

    reset: function () {
        this._abortIfPending(this._getPending());
        this._setPending(null);
    },

    sub: function () {
        return new RecurringPromise(this);
    },

    _getPending: function () {
        return this._parent ? this._parent._getPending() : this._pending;
    },

    _setPending: function (deferred) {
        if (this._parent) {
            this._parent._setPending(deferred);
        } else {
            this._pending = deferred;
        }
    },

    _abortIfPending: function (wrapperDeferred) {
        if (wrapperDeferred && wrapperDeferred.state() === 'pending') {
            wrapperDeferred.reject("abort");

            // Abort existing pending request if an abort function is available
            if (jQuery.isFunction(wrapperDeferred.original.abort)) {
                wrapperDeferred.original.abort();
            }
        }
    }
};

jQuery.RecurringPromise = RecurringPromise;

module.exports = RecurringPromise;
