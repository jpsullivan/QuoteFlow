"use strict";

var _ = require('underscore');
var Brace = require('backbone-brace');
var ContainerModel = require('./criteria-model');
var ContainerView = require('./criteria-view');

/**
 * View that handles displaying a search clause and creating its associated dialog
 */
var ExtendedCriteriaContainerView = Brace.View.extend({

    namedEvents: ["verticalResize"],

    initialize: function(options) {
        this.collection.on('change:isSelected', this._onCriteriaSelectionChanged, this);
        this.collection.on('add', this._onCriteriaSelectionChanged, this);
        this.collection.on('remove', this._removeViewForSearcher, this);
        this.collection.on('reset', this._onReset, this);

        this._criteriaViews = _.map(this.collection.getSelectedCriteria(), _.bind(function(searcherModel) {
            if (searcherModel.getPosition() == null) {
                searcherModel.setPosition(this.collection.getNextPosition());
            }
            return this._buildCriteriaViewForSearcher(searcherModel);
        }, this));
    },

    render: function() {
        _.each(this._criteriaViews, _.bind(function(criteriaView) {
            criteriaView.render();
            this.$el.append(criteriaView.$el);
        }, this));
    },

    /**
     * Returns a jQuery array of elements within this container that can be tab-focused
     */
    getFocusables: function() {
        return this.$('.criteria-selector');
    },

    /**
     * Returns the focusable element for the given criteria. The element returned should be one
     * of the elements in getFocusables()
     */
    getFocusableForCriteria: function(criteriaId) {
        return this.$('.criteria-selector[data-id="' + criteriaId + '"]');
    },

    _buildCriteriaViewForSearcher: function(searcherModel) {
        return new CriteriaView({
            model: new CriteriaModel({
                id: searcherModel.get('id'),
                name: searcherModel.get('name')
            }),
            searcherCollection: this.collection,
            extended: true
        });
    },

    /**
     * Handles the adding and removing of extended criteria
     */
    _onCriteriaSelectionChanged: function(searcherModel) {
        if (searcherModel.getIsSelected() && !this.collection.isFixed(searcherModel)) {
            this._addViewForSearcher(searcherModel);
        } else {
            this._removeViewForSearcher(searcherModel);
        }
    },

    _addViewForSearcher: function(searcherModel) {
        var criteriaView = this._buildCriteriaViewForSearcher(searcherModel);
        criteriaView.render();
        this.$el.append(criteriaView.$el);
        this._criteriaViews.push(criteriaView);
        this.triggerVerticalResize();
    },

    _removeViewForSearcher: function(searcherModel) {
        this._criteriaViews = _.reject(this._criteriaViews, function(criteriaView) {
            if (criteriaView.model.getId() === searcherModel.getId()) {
                criteriaView.destroy();
                return true; // Reject
            }
            return false;
        });
        this.triggerVerticalResize();
    },

    _onReset: function() {
        _.each(this._criteriaViews, function(criteriaView) {
            criteriaView.destroy();
        });
        this._criteriaViews = [];
    }
});

module.exports = ExtendedCriteriaContainerView;
