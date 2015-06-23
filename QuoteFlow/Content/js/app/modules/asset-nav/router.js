"use strict";

var _ = require('underscore');
var Backbone = require('backbone');
var BackboneQueryParams = require('backbone-query-parameters');
var Marionette = require('backbone.marionette');

var UrlSerializer = require('../../util/url-serializer');

var AssetNavRouter = Marionette.AppRouter.extend({
    appRoutes: {
        "quote/:id/:name/builder/?:query": "builder"
    },

    // initialize: function (options) {
    //     // equivalent to "quote/:id/:name/builder"
    //     this.route("/^quote\/([^\/\?]+)\/([^\/\?]+)\/builder(\?.*)?$/", "builder", options.controller.builder);
    // }
});

module.exports = AssetNavRouter;
