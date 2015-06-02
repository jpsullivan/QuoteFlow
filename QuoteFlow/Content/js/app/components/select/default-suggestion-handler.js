"use strict";

var $ = require('jquery');
var _ = require('underscore');

var Class = require('../class/class');
var SuggestHelper = require('./suggest-helper');

/**
 * A default suggestion handler. Used for autocomplete without a backing <select>
 * @class SuggestHandler
 * @class DefaultSuggestHandler
 */
var DefaultSuggestHandler = Class.extend({
    /**
     * @constructor
     * @param options
     */
     init: function (options) {
        this.options = options;
        this.descriptorFetcher = SuggestHelper.createDescriptorFetcher(options);
    },

    /**
     * Check if we should mirror input as a suggestion
     * @param {String} query
     * @return {Boolean}
     */
    validateMirroring: function (query) {
        return this.options.userEnteredOptionsMsg && query.length > 0;
    },

    /**
     * Applies default formatting
     *
     * @param {Array} descriptors
     * @param {String} query
     * @return {*}
     */
    formatSuggestions: function (descriptors, query) {
        if (this.validateMirroring(query)) {

            descriptors.push(SuggestHelper.mirrorQuery(query, this.options.userEnteredOptionsMsg, this.options.uppercaseUserEnteredOnSelect));
        }
        return descriptors;
    },

    /**
     * Requests descriptors then formats them
     * @param {String} query
     * @param {Boolean} force
     * @return {*}
     */
    execute: function (query, force) {
        var deferred = jQuery.Deferred();
        var fetcherDef = this.descriptorFetcher.execute(query, force).done(_.bind(function (descriptors) {
            if (descriptors) {
                descriptors = this.formatSuggestions(descriptors, query);
            }
            deferred.resolve(descriptors, query);
        }, this));
        deferred.fail(function () {
            fetcherDef.reject();
        });
        return deferred;
    }
});

module.exports = DefaultSuggestHandler;
