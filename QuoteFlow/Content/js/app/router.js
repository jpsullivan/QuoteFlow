"use strict";

var $ = require('jquery');
var Backbone = require('backbone');
Backbone.$ = $;

// Pages
var AssetPage = require('./pages/asset');
var CatalogPage = require('./pages/catalog');

var Router = Backbone.Router.extend({
    routes: {
        "asset/*subroute": "asset",
        "catalog/*subroute": "catalog"
    },

    asset: function() {
        this.renderPage(function() {
            return new AssetPage();
        });
    },

    catalog: function(subroute) {
//        this.renderPage(function() {
//            return new CatalogPage();
//        });

        return new CatalogPage.Router("catalog", { createTrailingSlashRoutes: false });
    },

    renderPage: function (pageConstructor) {
        var existingView = {};
        if (QuoteFlow.Page) {
            existingView = QuoteFlow.Page;
            QuoteFlow.Page.unbind && QuoteFlow.Page.unbind(); // old page might mutate global events $(document).keypress, so unbind before creating
        }

        var page = pageConstructor();

        // remove the old view after the new one renders to prevent any paint flashes
        existingView.remove && existingView.remove();
    }
});

module.exports = Router;