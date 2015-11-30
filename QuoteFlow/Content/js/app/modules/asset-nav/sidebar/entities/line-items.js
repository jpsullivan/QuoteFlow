"use strict";

var _ = require('underscore');
var Brace = require('backbone-brace');
var LineItemModel = require('./line-item');

/**
 * A collection of LineItemModel objects.
 * @extends Brace.Collection
 */
var LineItemsCollection = Brace.Collection.extend({
    model: LineItemModel,

    initialize: function(models, options) {
        options = options || {};
        this.fetchState = options.fetchState;
    },

    fetch: function () {
        var promise = Brace.Collection.prototype.fetch.apply(this, arguments);

        promise.done(_.bind(function () {
            this.fetchState = "fetched";
            this.trigger("change:fetchState", this.fetchState);
        }, this));

        promise.fail(_.bind(function () {
            this.fetchState = "error";
            this.trigger("change:fetchState", this.fetchState);
        }, this));

        return promise;
    }
});

module.exports = LineItemsCollection;
