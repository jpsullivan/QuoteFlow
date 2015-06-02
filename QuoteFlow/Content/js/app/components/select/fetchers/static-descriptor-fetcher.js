"use strict";

var $ = require('jquery');
var Class = require('../../class/class');

/**
 * Gets unselected <option>s from <select> as suggestions
 * @class StaticDescriptorFetcher
 */
var StaticDescriptorFetcher = Class.extend({
    /**
     * @param {Object} options - empty in this case
     * @param {SelectModel} model - a wrapper around <select> element
     */
    init: function (options, model) {
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
