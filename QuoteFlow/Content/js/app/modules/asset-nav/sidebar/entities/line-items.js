"use strict";

var _ = require('underscore');
var $ = require('jquery');
var Brace = require('backbone-brace');
var LineItemModel = require('./line-item');

/**
 * A collection of LineItemModel objects.
 * @extends {Brace.Collection}
 */
var LineItemsCollection = Brace.Collection.extend({
    model: LineItemModel,

    initialize: function (models, options) {
        options = options || {};
        this.fetchState = options.fetchState;
    },

    _setFetchState: function (state) {
        var isNewSate = state !== this.fetchState;

        this.fetchState = state;
        if (isNewSate) {
            this.trigger("change:fetchState", this.fetchState);
        }
    },

    fetch: function () {
        if (this.length > 0) {
            this._setFetchState("fetched");
            return $.Deferred().resolve();
        }

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
