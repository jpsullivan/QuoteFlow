"use strict";

var _ = require('underscore');
var Brace = require('backbone-brace');
var SingleSelectMixin = require('../mixins/single-select');

/**
 * Model that represents a switcher collection with a selection for the switcher in the query view.
 */
var QuerySwitcherViewModel = Brace.Model.extend({

    mixins: [SingleSelectMixin],

    namedAttributes: ["disabled"],

    namedEvents: ["selectionChanged"],

    initialize: function (attributes, options) {
        this.queryStateModel = options.queryStateModel;
        this.queryStateModel.on("change:searchMode", _.bind(function () {
            this.triggerSelectionChanged.apply(this, arguments);
        }, this));
    },

    getSelected: function () {
        var id = this.queryStateModel.getSearchMode();
        return id ? this.getCollection().get(id) : null;
    },

    setSelected: function (selected) {
        this.queryStateModel.switchPreferredSearchMode(selected ? selected.id : null);
    },

    enableSwitching: function () {
        this.setDisabled(false);
    },

    disableSwitching: function () {
        this.setDisabled(true);
    }
});

module.exports = QuerySwitcherViewModel;
