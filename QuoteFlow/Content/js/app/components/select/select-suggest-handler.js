"use strict";

var Brace = require('backbone-brace');
var SuggestHelper = require('./suggest-helper');
var DefaultSuggestHandler = require('./default-suggest-handler');

/**
 * A suggestion handler that removes suggestions that have already been selected in <select>
 * @class SelectSuggestHandler
 * @extends DefaultSuggestHandler
 */
var SelectSuggestHandler = DefaultSuggestHandler.extend({

    /**
     * @constructor
     * @param {Object} options
     * @param {SelectModel} model
     */
     init: function (options, model) {
        this.descriptorFetcher = SuggestHelper.createDescriptorFetcher(options, model);
        this.options = options;
        this.model = model;
    },

    /**
     * Formats suggestions removing already selected descriptors
     * @param descriptors
     * @param query
     * @return {GroupDescriptor[]}*/
    formatSuggestions: function (descriptors, query) {
        debugger;
        var suggestions = this._super(descriptors, query);
        var selectedDescriptors = this.model.getDisplayableSelectedDescriptors();
        if (this.options.removeDuplicates) {
            suggestions = SuggestHelper.removeDuplicates(descriptors);
        }
        return SuggestHelper.removeSelected(suggestions, selectedDescriptors);
    }
});

module.exports = SelectSuggestHandler;
