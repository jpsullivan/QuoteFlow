"use strict";

var $ = require('jquery');
var Class = require('../../class/class');

/**
 * A single fetcher that will just return the result of calling supplied function.
 *
 * @class FuncDescriptorFetcher
 */
var FuncDescriptorFetcher = Class.extend({
    /**
     * @constructor
     * @param options
     */
    init: function (options) {
        this.options = options;
    },

    /**
     * Gets result of function
     * @param query

     */
    execute: function (query) {
        var deferred = jQuery.Deferred();
        deferred.resolve(this.options.suggestions(query), query);
        return deferred;
    }
});

module.exports = FuncDescriptorFetcher;
