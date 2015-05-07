"use strict";

var _ = require('underscore');
var Brace = require('backbone-brace');
var CriteriaView = require('./criteria-view');
var CriteriaModel = require('./criteria-model');

var PrimaryCriteriaContainerView = Brace.View.extend({

    initialize: function() {
        this._criteriaViews = _.map(this.collection.fixedLozenges, function(primary) {
            return new CriteriaView({
                model: new CriteriaModel(primary),
                searcherCollection: this.collection
            });
        }, this);
    },

    render: function() {
        _.each(this._criteriaViews, function(view) {
            view.render();
        });

        this.$el.prepend(_.pluck(this._criteriaViews, 'el'));
    },

    getCriteriaViews: function() {
        return this._criteriaViews;
    },

    /**
     * Returns a jQuery array of elements within this container that can be tab-focused
     */
    getFocusables: function() {
        return this.$('.criteria-selector, #searcher-query, .add-criteria, .search-button');
    },

    /**
     * Returns the focusable element for the given criteria. The element returned should be one
     * of the elements in getFocusables()
     */
    getFocusableForCriteria: function(criteriaId) {
        return this.$('.criteria-selector[data-id="' + criteriaId + '"]');
    }
});

module.exports = PrimaryCriteriaContainerView;
