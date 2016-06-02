"use strict";

var Group = require('../group/group');

/**
 * A list item group has key handling for shifting focus with the vertical arrow keys,
 * and accepting an item with the return key.
 *
 * @class Dropdown.ListItemGroup
 * @extends Group
 */
var DropdownListItemGroup = Group.extend({
    keys: {
        "Up": function (event) {
            this.shiftFocus(-1);
            event.preventDefault();
        },
        "Down": function (event) {
            this.shiftFocus(1);
            event.preventDefault();
        },
        "Return": function (event) {
            this.items[this.index].trigger("accept");
            event.preventDefault();
        }
    }
});

module.exports = DropdownListItemGroup;
