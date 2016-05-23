"use strict";

var $ = require('jquery');
var _ = require('underscore');
var Backbone = require('backbone');
var SubRoute = require('backbone.subroute');
Backbone.$ = $;

var BaseView = require('../view');

// UI Components
var ShowAssetsInteractive = require('../ui/catalog/show_interactive');

/**
 *
 */
var CatalogPage = BaseView.extend({
    templateName: null,
    subviews: {},
    events: {},
    options: {},

    presenter: function () { },

    initialize: function () {},

    postRenderTemplate: function () { },

    /**
     *
     */
    showAssetsInteractive: function (catalogId, catalogName) {
        var view = new ShowAssetsInteractive();
    }
});

CatalogPage.Router = Backbone.SubRoute.extend({
    routes: {
        "catalog/:catalogId/:catalogName": "show",
        "catalog/:catalogId/:catalogName/assets": "showAssets",
        ":catalogId/:catalogName/assets/iv": "showAssetsInteractive"
    },

    show: function (catalogId, catalogName) {
        return new CatalogPage().show(catalogId, catalogName);
    },

    showAssets: function (catalogId, catalogName) {
        return new CatalogPage().showAssets(catalogId, catalogName);
    },

    showAssetsInteractive: function (catalogId, catalogName) {
        return new CatalogPage().showAssetsInteractive(catalogId, catalogName);
    }
});

module.exports = CatalogPage;
