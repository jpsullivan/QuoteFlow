"use strict";

var $ = require('jquery');
var _ = require('underscore');
var Class = require('../../class/class');
var SmartAjax = require('../../ajax/smart-ajax');

/**
 * Retrieves json from server and converts it into descriptors using formatSuggestions function supplied by user.
 * @class AjaxDescriptorFetcher
 */
var AjaxDescriptorFetcher = Class.extend({

    /**
     * @constructor
     * @param options
     */
     init: function (options) {
        this.options = _.extend({
            keyInputPeriod: 75, // Wait this long between key strokes before going to server
            minQueryLength: 1, // Need these many characters before we go to server
            data: {},
            dataType: "json"
        }, options);
    },

    // Actually make the request and notify those interested
    makeRequest: function (deferred, ajaxOptions, query) {
        ajaxOptions.complete = _.bind(function () {
            this.outstandingRequest = null;
        }, this);
        ajaxOptions.success = _.bind(function (data) {
            if (ajaxOptions.query) {
                deferred.resolve(ajaxOptions.formatResponse(data, query));
            } else {
                this.lastResponse = ajaxOptions.formatResponse(data, query);
                deferred.resolve(this.lastResponse);
            }
        }, this);
        var originalError = ajaxOptions.error;
        ajaxOptions.error = function (xhr, textStatus, msg, smartAjaxResult) {
            if (!smartAjaxResult.aborted) {
                if (originalError) {
                    originalError.apply(this, arguments);
                } else {
                    alert(SmartAjax.buildSimpleErrorContent(smartAjaxResult, { alert: true }));
                }
            }
        };

        this.outstandingRequest = SmartAjax.makeRequest(ajaxOptions); // issue requestcle
    },

    /**
     * Prepare the data and prevent throttling of server
     * @param {jQuery.Deferred} deferred
     * @param {Object} ajaxOptions - standard jQuery ajax options
     * @param {String} query - in most cases this is the user input
     * @param {Boolean} force - ignore request buffers. I want my request dispatched NOW.
     */
    incubateRequest: function (deferred, ajaxOptions, query, force) {
        clearTimeout(this.queuedRequest); // cancel any queued requests

        if (force && this.outstandingRequest) {
            this.outstandingRequest.abort();
            this.outstandingRequest = null;
        }

        if (!ajaxOptions.query && this.lastResponse) {
            deferred.resolve(this.lastResponse);
        } else if (!this.outstandingRequest) {
            if (typeof ajaxOptions.data === 'function') {
                ajaxOptions.data = ajaxOptions.data(query);
            } else {
                ajaxOptions.data.query = query;
            }

            if (typeof ajaxOptions.url === 'function') {
                ajaxOptions.url = ajaxOptions.url();
            }

            if ((query.length >= parseInt(ajaxOptions.minQueryLength, 10)) || force) {
                this.makeRequest(deferred, ajaxOptions, query);
            } else {
                deferred.resolve();
            }
        } else {
            this.queuedRequest = setTimeout(_.bind(function () {
                this.incubateRequest(deferred, ajaxOptions, query, true);
            }, this), ajaxOptions.keyInputPeriod);
        }

        return deferred;
    },

    /**
     * Sets up a request
     * @param {Function} query - lazily evaluated value of input field.
     * @param {Boolean} force - Piss off all buffers etc. Make request now!
     * @return {jQuery.Deferred}
     */
    execute: function (query, force) {
        var deferred = jQuery.Deferred();
        deferred.fail(_.bind(function () {
            clearTimeout(this.queuedRequest);
            if (this.outstandingRequest) {
                this.outstandingRequest.abort();
            }
        }, this));
        this.incubateRequest(deferred, _.extend({}, this.options), query, force);
        return deferred;
    }
});

module.exports = AjaxDescriptorFetcher;
