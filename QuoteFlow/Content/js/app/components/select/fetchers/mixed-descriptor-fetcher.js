"use strict";

var $ = require('jquery');
var _ = require('underscore');
var Brace = require('backbone-brace');

var AjaxDescriptorFetcher = require('./ajax-descriptor-fetcher');

/**
 * Gets suggestions from unselected <option>s in <select> as well as going to the 
 * server upon character for more results on input.
 *
 * @class MixedDescriptorFetcher
 */
var MixedDescriptorFetcher = Brace.Evented.extend({
    /**
     *
     * @param {Object} options - jQuery ajax options object. With additional:
     * @param {function} options.formatResponse - function for creating descriptors out of server response
     * @param {number} options.minQueryLength - min input length before a request is made
     * @param {Object} options.ajaxOptions
     * @param {SelectModel} model - a wrapper around <select> element
     */
    initialize: function (options, model) {
        this.ajaxFetcher = new AjaxDescriptorFetcher(options.ajaxOptions);
        this.options = options;
        this.model = model;
    },

    /**
     * @param query
     * @param force
     * @return {jQuery.Deferred}
     */
    execute: function (query, force) {
        var deferred = jQuery.Deferred();
        // This needs to come after the return statement...
        if (query.length >= 1) {
            var ajaxDeferred = this.ajaxFetcher.execute(query, force).done(_.bind(function (suggestions) {
                // JRADEV-21004
                // Put suggestions at the front to avoid them being removed by removeDuplicates() method.
                // After that, we sort the descriptors based on a label, so this change won't affect the
                // final result
                var descriptors = [].concat(suggestions).concat(this.model.getAllDescriptors());
                deferred.resolve(descriptors, query);
            }, this));
            deferred.fail(function () {
                ajaxDeferred.reject();
            });
        } else {
            deferred.resolve(this.model.getUnSelectedDescriptors(), query);
        }
        return deferred;
    }
});

module.exports = MixedDescriptorFetcher;