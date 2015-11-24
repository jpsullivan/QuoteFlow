"use strict";

var $ = require('jquery');
var _ = require('underscore');
var Brace = require('backbone-brace');

var EventTypes = require('../util/types');
var OrderByDropDownView = require('./dropdown-view');

/**
 * The order by view that's shown over the list of issues in split view.
 *
 * @type OrderByView
 */
var OrderByView = Brace.View.extend({

    template: JST["quote-builder/split-view/orderby"],

    events: {
        "click a.order-by": "_onClickOrderBy",
        "click a.order-options": "_toggleShowDropDown"
    },

    initialize: function () {
        // Whenever the sort jql changes then we re-render
        this.model.on("change:sortBy", this.render, this);
    },

    /**
     * Renders this view base on the SearchResults' 'sortBy' property.
     *
     * @return {JIRA.Issues.OrderByView}
     */
    render: function () {
        this.$el.html(this.template(this.model.toJSON()));
        return this;
    },

    deactivate: function () {
        // make sure we cleanup events when we switch to list view
        this.orderByDropDown && this.orderByDropDown.deactivate();
        this.undelegateEvents();
    },

    /**
     * Toggles sort between ASC/DESC
     * @param e
     */
    _onClickOrderBy: function (e) {
        var event = new $.Event(EventTypes.ASSET_TABLE_REORDER);
        QuoteFlow.trigger(event);
        if (!event.isDefaultPrevented()) {
            var fieldId = $(e.currentTarget).data('field-id');
            if (fieldId) {
                this.model.toggleSort();
            }
        }
        e.preventDefault();
    },
    /**
     * Hiding/Showing of sparkler
     */
    _toggleShowDropDown: function (e) {
        if (!this.orderByDropDown) {
            // lazy create
            this.orderByDropDown = new OrderByDropDownView({
                model: this.model,
                offsetTarget: this.$('a.order-by'),
                onHideCallback: _.bind(function () { this.orderByDropDown = null; }, this)
            });
        }
        this.orderByDropDown.toggle();
        e.preventDefault();
    }
});

module.exports = OrderByView;
