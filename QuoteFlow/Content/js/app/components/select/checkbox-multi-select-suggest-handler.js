"use strict";

var GroupDescriptor = require('../list/group-descriptor');
var SelectSuggestHandler = require('./select-suggest-handler');
var SuggestHelper = require('./suggest-helper');

/**
 * A suggestion handler that without a query, shows selected items at the top followed by unselected items in their groups.
 * When querying selected and unselected items are munged together and sorted in alphabetical order.
 * @class CheckboxMultiSelectSuggestHandler
 * @extends SelectSuggestHandler
 */
var CheckboxMultiSelectSuggestHandler = SelectSuggestHandler.extend({

    /**
     * Creates html string for clear all
     * @return {String}
     */
    createClearAll: function () {
        return "<li class='check-list-group-actions'><a class='clear-all' href='#'>" + AJS.I18n.getText("jira.ajax.autocomplete.clear.all") + "</a></li>";
    },

    /**
     * Formats descriptors for display in checkbox multiselect
     *
     * @param descriptors
     * @param query
     * @return {Array} formatted descriptors
     */
    formatSuggestions: function (descriptors, query) {

        var selectedItems = SuggestHelper.removeDuplicates(this.model.getDisplayableSelectedDescriptors());
        var selectedGroup = new GroupDescriptor({
            styleClass: "selected-group",
            items: selectedItems,
            actionBarHtml: selectedItems.length > 1 ? this.createClearAll()  : null
        });
        descriptors.splice(0, 0, selectedGroup);
        if (query.length > 0) {
            descriptors = SuggestHelper.removeDuplicates(descriptors);
            // Extract all items from the descriptors and sort them by label.
            var items = SuggestHelper.extractItems(descriptors).sort(function(a, b) {
                a = a.label().toLowerCase();
                b = b.label().toLowerCase();
                return a.localeCompare(b);
            });
            descriptors = [new GroupDescriptor({items: items})];
        }
        return descriptors;
    }
});

module.exports = CheckboxMultiSelectSuggestHandler;
