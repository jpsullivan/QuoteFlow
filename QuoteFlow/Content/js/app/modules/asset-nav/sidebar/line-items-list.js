"use strict";

var Marionette = require('backbone.marionette');
var LineItemView = require('./line-item-view');

/**
 * Displays a list of line items.
 * @extends {Marionette.CompositeView}
 */
var LineItemsList = Marionette.CompositeView.extend({
    template: JST["quote-builder/sidebar/list"],
    childView: LineItemView,
    childViewContainer: ".filter-list",

    onRender: function () {
        this.unwrapTemplate();
    },

    templateHelpers: function () {
        return {
            className: this.className
        };
    },

    unhighlightAllFilters: function () {
        this.children.apply("unhighlight");
    },

    /**
     * Highlight a filter. If the model does not exist in the collection represented by this list,
     * this method does nothing.
     *
     * @param {LineItem} filterModel Model to highlight
     */
    highlightLineItem: function (filterModel) {
        var itemView = this.children.findByModel(filterModel);
        if (itemView) {
            itemView.highlight();
        }
    }
});

module.exports = LineItemsList;
