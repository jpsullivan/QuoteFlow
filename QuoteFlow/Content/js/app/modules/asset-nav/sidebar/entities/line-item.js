"use strict";

var $ = require('jquery');
var Brace = require('backbone-brace');
var SmartAjax = require('../../../../components/ajax/smart-ajax');

var LineItemModel = Brace.Model.extend({
    namedAttributes: [
        "id",
        "name",
        "sku",
        "price",
        "quantity",
        "assetId",
        "quoteId",
        "createdUtc",
        "asset",
        "jql"
    ],

    urlRoot: QuoteFlow.RootUrl + "/api/quote/lineitems",

    url: function() {
        return this.urlRoot + encodeURIComponent(this.id);
    },

    parse: function(response) {
        return {
            id: response.id,
            name: response.name,
            sku: response.sku,
            price: response.price,
            quantity: response.quantity,
            jql: response.jql
        };
    },

    saveQuantity: function (quantity) {
        var instance = this;
        var prevState = this.getQuantity();

        // Optimistically set quantity
        this.setQuantity(quantity);

        // abort previous pending request
        if (this.pendingRequest) {
            this.pendingRequest.abort();
        }

        this.pendingRequest = SmartAjax.makeRequest({
            url: QuoteFlow.RootUrl + '/api/quote/lineitem/' + encodeURIComponent(this.getId()) + '/quantity',
            type: quantity > 0 ? 'PUT' : 'DELETE'
        }).done(function(filterData) {
            instance.set(instance.parse(filterData));
        }).fail(function () {
            instance.setQuantity(prevState);
        }).then(function() {
            delete instance.pendingRequest;
            QuoteFlow.trace("quoteflow.sidebar.lineitem.saved");
        });

        return this.pendingRequest;
    }
});

module.exports = LineItemModel;
