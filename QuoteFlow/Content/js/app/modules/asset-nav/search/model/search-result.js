"use strict";

var Backbone = require('backbone');

var SearchResultModel = Backbone.Model.extend({
    initialize: function () {
        // We need to create url function inside the initialize method because when combining
        // Backbone 1.0.0 and PageableCollection, it will pass its own URL when creating models,
        // making all the models have the same URL than the collection (duh). This has been fixed
        // in Backbone 1.1.0
        this.url = function () {
            return QuoteFlow.RootUrl + "asset/builder/getAsset?assetId=" + this.id;
            // return AJS.contextPath() + "/rest/api/2/issue/" + this.id + "?fields=summary,status,issuetype";
        };
    },

    parse: function (data) {
        // Backbone will call this method two times:
        // 1. When the SearchResults is initially created with a specific page.
        // 2. When updating an issue after a successful inline edit.

        var result = {};

        if (data.inaccessible) {
            result.id = parseInt(data.id, 10);
            result.sku = data.sku;
            result.inaccessible = true;
        } else {
            result.id = parseInt(data.id, 10);
            result.sku = data.sku;

            if (data.fields) {
                // This comes from inline edit.
                result.summary = data.fields.name;
                result.cost = data.fields.cost;
                result.type = {
                    description: data.fields.issuetype.description,
                    iconUrl: data.fields.issuetype.iconUrl,
                    name: data.fields.issuetype.name
                };
            } else {
                // This comes from fetching a page
                result.cost = data.cost;
                result.summary = data.name;
                // result.type = data.type;
            }
        }

        return result;
    }
});

module.exports = SearchResultModel;
