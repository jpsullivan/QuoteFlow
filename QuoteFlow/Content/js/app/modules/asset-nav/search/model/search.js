"use strict";

var _ = require('underscore');
var $ = require('jquery');
var Backbone = require('backbone');

/**
 * This class contains the model for a Search. A Search is a collection of
 * assets as a result of running a JQL in the server. This entity is not meant
 * to be used directly by any view. Instead, this entity will generate a
 * SearchResults entity that contains the stable search.
 *
 * All operations performed in this entity will result in a new set of results
 * (aka un-stable search).
 */
var SearchCollection = Backbone.Collection.extend({
    url: function () {
        return QuoteFlow.RootUrl + "api/assetTable";
    },

    searchParams: {
        jql: "",
        quoteId: null,
        columnConfig: "explicit",
        columns: [],
        layoutKey: "split-view",
        startIndex: 0
    },

    sync: function () {
        var args = _.toArray(arguments);
        var params = args[2];

        params.type = "POST";
        params.headers = {
            "X-Atlassian-Token": "no-check"
        };

        // Add our search data.
        params.data = {};
        _.each(_.keys(this.searchParams), function (key) {
            if (this.searchParams[key] !== null) {
                params.data[key] = this.searchParams[key];
            }
        }, this);

        return Backbone.Collection.prototype.sync.apply(this, args);
    },

    parse: function (resp) {
        // Clean the data
        var data = resp && resp.issueTable;
        if (!data || !data.issueIds) {
            return [];
        }

        // Extract all the issues from the first search.
        return _.map(data.issueIds, function (id, index) {
            return {id: id, sku: data.issueKeys[index]};
        });
    },

    search: function (jql) {
        this.searchParams.jql = jql;

        var deferred = new jQuery.Deferred();
        this.fetch({
            reset: true,
            success: _.bind(function (collection, response) {
                var results = new Results([], {
                    issues: this.toJSON(),
                    pageSize: response.issueTable.pageSize,
                    totalRecordsInDB: response.issueTable.total,
                    totalRecordsInSearch: this.length,
                    jql: collection.searchParams.jql
                });
                deferred.resolve(results);
            }, this),
            error: function (collection, response) {
                deferred.reject(response);
            }
        });

        return deferred.promise();
    }
});

module.exports = SearchCollection;
