"use strict";

var Marionette = require('backbone.marionette');
var CatalogController = require('./controller');

/**
 *
 */
var AssetRouter = Marionette.AppRouter.extend({
    appRoutes: {
        "asset/new": "create"
    }
});

module.exports = AssetRouter;
