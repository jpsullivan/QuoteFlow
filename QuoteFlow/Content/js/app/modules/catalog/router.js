"use strict";

var Marionette = require('backbone.marionette');
var CatalogController = require('./controller');

/**
 * 
 */
var CatalogRouter = Marionette.AppRouter.extend({
    //controller: new CatalogController(),
    appRoutes: {
        "catalog/new": "create",
        "catalog/import": "importCatalog",
        "catalog/verify": "verify",
        "catalog/:catalogId/:catalogName": "show",
        "catalog/:catalogId/:catalogName/assets": "showAssets",
        ":catalogId/:catalogName/assets/iv": "showAssetsInteractive"
    }
});

module.exports = CatalogRouter;
