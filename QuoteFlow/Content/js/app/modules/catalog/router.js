"use strict";

var Marionette = require('backbone.marionette');
var CatalogController = require('./controller');

/**
 * 
 */
var CatalogRouter = new Marionette.AppRouter({
    appRoutes: {
        "catalog/new": "create",
        "catalog/:catalogId/:catalogName": "show",
        "catalog/:catalogId/:catalogName/assets": "showAssets",
        ":catalogId/:catalogName/assets/iv": "showAssetsInteractive"
    }
});

module.exports = CatalogRouter;
