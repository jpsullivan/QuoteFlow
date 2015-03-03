"use strict";

var $ = require('jquery');
var Brace = require('backbone-brace');

/**
 * Gets unselected <option>s from <select> as suggestions
 * @class StaticDescriptorFetcher
 */
var StaticDescriptorFetcher = Brace.Evented.extend({
    /**
     * @param {Object} options - empty in this case
     * @param {SelectModel} model - a wrapper around <select> element
     */
    initialize: function (options, model) {
        this.model = model;
        this.model.$element.data("static-suggestions", true);
    },

    /**
     * @return {jQuery.Deferred}
     */
    execute: function (query) {
        var deferred = jQuery.Deferred();
        deferred.resolve(this.model.getUnSelectedDescriptors(), query);
        return deferred;
    }
});

module.exports = StaticDescriptorFetcher;