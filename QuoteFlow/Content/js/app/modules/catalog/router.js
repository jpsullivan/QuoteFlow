"use strict";

var Marionette = require('backbone.marionette');

/**
 *
 */
var CatalogRouter = Marionette.AppRouter.extend({
    appRoutes: {
        "catalog/new": "create",
        "catalog/import": "importCatalog",
        "catalog/verify": "verify",
        "catalog/verifyother": "verifySecondary",
        "catalog/:catalogId/:catalogName/assets": "assets",
        ":catalogId/:catalogName/assets/iv": "showAssetsInteractive"
    }
});

module.exports = CatalogRouter;
