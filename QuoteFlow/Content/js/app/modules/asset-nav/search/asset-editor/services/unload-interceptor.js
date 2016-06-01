"use strict";

var _ = require('underscore');
var Marionette = require('backbone.marionette');

var UnloadInterceptor = Marionette.Controller.extend({
    initialize: function () {
        this._afterEventHandlers = [];
        this.originalBeforeUnload = window.onbeforeunload;
        window.onbeforeunload = _.bind(this.onBeforeUnload, this);
    },

    addAfterEvent: function (handler) {
        this._afterEventHandlers.push(handler);
    },

    removeAfterEvent: function (handler) {
        this._afterEventHandlers = _.without(this._afterEventHandlers, handler);
    },

    onBeforeUnload: function () {
        var result;
        var args = arguments;

        // Run the original handler (if present)
        if (_.isFunction(this.originalBeforeUnload)) {
            result = this.originalBeforeUnload.apply(window, args);
        }

        // If the original handler returned a truty value, don't process the afterEventHandlers
        if (!result) {
            result = _.reduce(this._afterEventHandlers, function (memo, value) {
                // If the previous handler returned anything, skip the rest of handlers
                if (memo) {
                    return memo;
                }
                return value.apply(window, args);
            }, result);
        }

        return result;
    }
});

module.exports = UnloadInterceptor;
