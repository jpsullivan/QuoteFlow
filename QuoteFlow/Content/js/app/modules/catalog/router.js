﻿"use strict";

var Marionette = require('backbone.marionette');
var CatalogController = require('./controller');

/**
 * 
 */
var CatalogRouter = Marionette.AppRouter.extend({
    appRoutes: {
        "catalog/new": "create",
        "catalog/import": "importCatalog",
        "catalog/verify": "verify",
        "catalog/verifyother": "verifySecondary",
        ":catalogId/:catalogName/assets/iv": "showAssetsInteractive"
    }
});

module.exports = CatalogRouter;