"use strict";

var _ = require('underscore');
var Backbone = require('backbone');
var BackboneQueryParams = require('backbone-query-parameters');
var Marionette = require('backbone.marionette');

var UrlSerializer = require('../../util/url-serializer');

var AssetNavRouter = Marionette.AppRouter.extend({
    appRoutes: {
        "quote/:id/:name/builder": "builder"
    }
});

module.exports = AssetNavRouter;
