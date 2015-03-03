"use strict";

var $ = require('jquery');
var ItemDescriptor = require('./item-descriptor');

/**
 * The message descriptor is used in {@link QueryableDropdownSelect} to define characteristics and
 * display of items added to suggestions dropdown and in the case of {@link QueryableDropdownSelect}
 * and {@link SelectModel} also.
 *
 * It displays an AUI message instead a regular item.
 *
 * @class MessageDescriptor
 * @extends ItemDescriptor
 */
var MessageDescriptor = ItemDescriptor.extend({

    /**
     * Gets the useAUI attribute
     *
     * @method useAUI
     * @return {Boolean}
     */
    useAUI: function () {
        return this.properties.useAUI;
    },

    /**
     * Gets message ID, used for the DOM Element
     *
     * @method domID
     * @return {String}
     */
    messageID: function () {
        return this.properties.messageID;
    },

    _getDefaultOptions: function () {
        return $.extend(this._super(), {
            useAUI: true
        });
    }
});

/**
 * Factory method that creates a MessageDescriptor.
 */
MessageDescriptor.create = function(suggestion, groupIndex) {
    return new MessageDescriptor({
        label: suggestion.label,
        styleClass: suggestion.styleClass,
        useAUI: suggestion.useAUI,
        messageID: suggestion.messageID,
        value: suggestion.value,
        keywords: suggestion.keywords,
        meta: { groupIndex: groupIndex }
    });
};

module.exports = MessageDescriptor;