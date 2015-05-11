"use strict";

var $ = require('jquery');
var ContentRetriever = require('./content-retriever');

var AjaxContentRetriever = ContentRetriever.extend({
    init: function (func) {
        this.func = func;
    },

    /**
     * Gets content via invocation or callback
     *
     * @method content
     * @param {Function} callback - if provided executes callback with content being the first argument
     */
    content: function (callback) {
        if ($.isFunction(callback)) {
            var res = this.func();
            if (res instanceof $) {
                callback(res);
            } else {
                res.done(_.bind(function (content) {
                    callback(content);
                }, this));
            }
        }
    },

    // these methods below are only used by asynchronous content retrievers, however we still need to define them.

    /** @method cache */
    cache: function () {
        return false;
    },

    /** @method isLocked */
    isLocked: function () {},

    /** @method startingRequest */
    startingRequest: function () {},

    /** @method startingRequest */
    finishedRequest: function () {}
});

module.exports = AjaxContentRetriever;
