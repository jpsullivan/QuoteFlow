"use strict";

var $ = require('jquery');
var scrollLock = require('jquery-scrollLock');
var _ = require('underscore');
var Backbone = require('backbone');
Backbone.$ = $;

// Data Layer
var NavigationCriteria = require('./criteria');
var CriteriaModel = require('../../../models/asset/criteria');

// UI Components
var BaseView = require('../../../view');

/**
 *
 */
var PrimaryCriteriaContainer = BaseView.extend({
    el: '.search-criteria',

    initialize: function() {
        this._criteriaViews = _.map(this.collection.fixedLozenges, function(criteria) {
            var el = $('li.' + criteria.id, this.$el);
            AJS.$(el).scrollLock(".aui-list-scroll");

            return new NavigationCriteria({
                el: el,
                model: new CriteriaModel(criteria),
                searcherCollection: this.collection
            });
        }, this);
    },

//    render: function () {
//        _.each(this._criteriaViews, function (a) {
//            a.render();
//        });
//        this.$el.prepend(_.pluck(this._criteriaViews, "el"));
//    },

    getCriteriaViews: function() {
        return this._criteriaViews;
    },

    /**
     * Returns a jQuery array of elements within this container that can be tab-focused
     */
    getFocusables: function () {
        return this.$('.criteria-selector, #searcher-query, .add-criteria, .search-button');
    },

    /**
     * Returns the focusable element for the given criteria. The element returned should be one
     * of the elements in getFocusables()
     */
    getFocusableForCriteria: function (criteriaId) {
        return this.$('.criteria-selector[data-id="' + criteriaId + '"]');
    }
});

module.exports = PrimaryCriteriaContainer;