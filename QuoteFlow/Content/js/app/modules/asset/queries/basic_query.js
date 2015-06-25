"use strict";

var $ = require('jquery');
var _ = require('underscore');
var Backbone = require('backbone');
var Brace = require('backbone-brace');
Backbone.$ = $;

var SearcherCollection = require('../../../collections/asset/searcher');
var BasicQueryView = require('../../asset-nav/query/basic-query-view');

/**
 * Module for basic query mode.
 */
var BasicQueryModule = Brace.Evented.extend({
    namedEvents: ["jqlTooComplex", "searchRequested", "basicModeCriteriaCountWhenSearching", "verticalResize"],

    initialize: function(options) {
        this._queryStateModel = options.queryStateModel;
        this.searcherCollection = new SearcherCollection([], {
            fixedLozenges: options.primaryClauses,
            queryStateModel: options.queryStateModel,
            initData: options.initialSearcherCollectionState
        });

        this.view = new BasicQueryView({
           queryStateModel: options.queryStateModel,
           searcherCollection: this.searcherCollection
        })
        .onVerticalResize(this.triggerVerticalResize, this)
        .onSearchRequested(this.triggerSearchRequested, this);

        this.searcherCollection.onSearchRequested(_.bind(function(jql) {
            this.triggerBasicModeCriteriaCountWhenSearching({
                count: this.searcherCollection.getAllSelectedCriteriaCount()
            });
            var jqlWithOrderBy = this._attachOrderByClause(jql);
            this.triggerSearchRequested(jqlWithOrderBy);
        }, this));

        this.searcherCollection.onJqlTooComplex(_.bind(function(jql) {
            this.triggerJqlTooComplex(jql);
        }, this));
    },

    hasErrors: function () {
        var hasErrors = this.searcherCollection.any(function (searcherModel) {
            return searcherModel.hasErrorInEditHtml();
        });
        return hasErrors;
    },

    /**
     * Remove all searchers and clear the text query.
     */
    clear: function () {
        this.searcherCollection.clear();
    },

    queryChanged: function () {
        this.searcherCollection.restoreFromQuery(this._queryStateModel.getJql());
    },

    queryReset: function (jql) {
        this.searcherCollection.setInteractive(false);
        return this.searcherCollection.restoreFromQuery(jql, true).always(_.bind(function () {
            this.searcherCollection.setInteractive(true);
        }, this));
    },

    /**
     * Wait any in flight updates to search collection.
     */
    searchersReady: function () {
        return this.searcherCollection.searchersReady();
    },

    createView: function() {
        return this.view;
    },

    getSelectedCriteria: function () {
        return this.searcherCollection.getAllSelectedCriteria()
    },

    _attachOrderByClause: function (jql) {
        var orderByRegex = /\bORDER\s+BY\b.*$/i;
        var existingOrderByClause = orderByRegex.exec(this._queryStateModel.getJql());
        if (existingOrderByClause && orderByRegex.exec(jql) === null) {
            jql = jql ? jql + ' ' + existingOrderByClause[0] : existingOrderByClause[0];
        }
        return jql;
    }
});

module.exports = BasicQueryModule;
