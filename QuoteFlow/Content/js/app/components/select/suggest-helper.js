"use strict";

var _ = require('underscore');

var GroupDescriptor = require('../list/group-descriptor');
var ItemDescriptor = require('../list/item-descriptor');
var MixedDescriptorFetcher = require('./fetchers/mixed-descriptor-fetcher');
var AjaxDescriptorFetcher = require('./fetchers/ajax-descriptor-fetcher');
var FuncDescriptorFetcher = require('./fetchers/func-descriptor-fetcher');
var StaticDescriptorFetcher = require('./fetchers/static-descriptor-fetcher');

/**
 * A utility object to manipulate/create suggestions.
 */
var SuggestHelper = {

    /**
     * Factory method to create descriptor fetcher based on user optiosn
     *
     * @param options
     * @param {SelectModel} model
     */
    createDescriptorFetcher: function (options, model) {
        if (options.ajaxOptions && options.ajaxOptions.url) {
            if (model && options.content === "mixed") {
                return new MixedDescriptorFetcher(options, model);
            } else {
                return new AjaxDescriptorFetcher(options.ajaxOptions);
            }
        } else if (options.suggestions) {
            return new FuncDescriptorFetcher(options);
        } else if (model) {
            return new StaticDescriptorFetcher(options, model);
        }
    },

    /**
     * Extract all item descriptors within an array of group descriptors.
     *
     * @param descriptors {GroupDescriptor[]} The group descriptors.
     * @return {ItemDescriptor[]} All item descriptors within.
     */
    extractItems: function (descriptors) {
        return _.flatten(_.map(descriptors, function (descriptor) {
            if (descriptor instanceof GroupDescriptor) {
                return descriptor.items();
            } else {
                return [descriptor];
            }
        }));
    },
    /**
     * Creates a descriptor group that mirrors the inputted query
     * @param {String} query
     * @param {String} label
     * @param {Boolean} uppercaseValue
     * @return {GroupDescriptor}
     */
    mirrorQuery: function (query, label, uppercaseValue) {
        var value = uppercaseValue ? query.toUpperCase() : query;
        return new GroupDescriptor({
            label: "user inputted option",
            showLabel: false,
            replace: true
        }).addItem(new ItemDescriptor({
            value: value,
            label: value,
            labelSuffix: " (" + label + ")",
            title: value,
            allowDuplicate: false,
            noExactMatch: true          // this item doesn't count as an exact query match for selthis.ection purposes
        }));
    },
    /**
     * Does the item descriptor match any of the selected values
     * @param {ItemDescriptor} itemDescriptor
     * @param {String[]} selectedVals
     * @return {Boolean}
     */
    isSelected: function (itemDescriptor, selectedVals) {
        return _.any(selectedVals, function (descriptor) {
            return itemDescriptor.value() === descriptor.value();
        });
    },
    /**
     * Removes duplicate descriptors
     *
     * @param descriptors
     * @param vals
     * @return {Array}
     */
    removeDuplicates: function (descriptors, vals) {
        vals = vals || [];
        return _.filter(descriptors, _.bind(function (descriptor) {
            if (descriptor instanceof GroupDescriptor) {
                descriptor.items(this.removeDuplicates(descriptor.items(), vals));
                return true;
            } else if (!_.include(vals, descriptor.value())) {
                if (descriptor.value()) {
                    vals.push(descriptor.value());
                }
                return true;
            }
        }, this));
    },
    /**
     * Loop over all descriptors and remove descriptors that match selected vals. Usually if the user has already
     * selected a suggestion, we don't want to show it.
     * @param {GroupDescriptor[], ItemDescriptor[]} descriptors
     * @param {String[]} selectedValues
     * @return {GroupDescriptor[], ItemDescriptor[]} descriptors
     * @private
     */
    removeSelected: function (descriptors, selectedValues) {
        return _.filter(descriptors, _.bind(function (descriptor) {
            if ((descriptor instanceof ItemDescriptor) && this.isSelected(descriptor, selectedValues)) {
                return false;
            }
            if (descriptor instanceof GroupDescriptor) {
                descriptor.items(this.removeSelected(descriptor.items(), selectedValues));
            }
            return true;
        }, this));
    }
};

module.exports = SuggestHelper;
