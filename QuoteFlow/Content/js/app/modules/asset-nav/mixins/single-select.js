"use strict";

/**
 * Mixin that provides single selection on a collection.
 * Requires that the object being mixed into provides "getSelected" and "setSelected" methods. These can be created by having a "selected" attribute or otherwise.
 */
var SingleSelect = {

    /**
     * collection: item in selection
     */
    namedAttributes: ["collection"],

    /**
     * Selects the model with the given id. No validation is done; if the id is invalid, this.selected will be undefined.
     * @param id if of model to select.
     */
    selectById: function(id) {
        this.setSelected(this.getCollection().get(id));
    },

    /**
     * Selects the model and the given index.
     * @param index index of model to select
     */
    selectAt: function(index) {
        this.setSelected(this.getCollection().at(index));
    },

    /**
     * Clears selection
     */
    clearSelection: function() {
        this.setSelected();
    },

    /**
     * Selects the next element, or the first if none is selected.
     */
    next: function() {
        var selected = this.getSelected(),
                col = this.getCollection();

        if (!selected) {
            if (col.length > 0) {
                this.setSelected(col.first());
            }
        }
        else {
            var index = col.indexOf(selected);
            var nextIndex = (index + 1) % col.length;
            this.setSelected(col.at(nextIndex));
        }
    },

    /**
     * Selects the previous element, or the last if none is selected.
     */
    prev: function() {
        var selected = this.getSelected(),
                col = this.getCollection();

        if (!selected) {
            if (col.length > 0) {
                this.setSelected(col.last());
            }
        }
        else {
            var index = col.indexOf(selected);
            var nextIndex = (index + col.length - 1) % col.length;
            this.setSelected(col.at(nextIndex));
        }
    }
};

module.exports = SingleSelect;
