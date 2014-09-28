"use strict";

var $ = require('jquery');
var Backbone = require('backbone');
Backbone.$ = $;

// Pages
var AssetPage = require('../app/pages/asset');

var Router = Backbone.Router.extend({
    routes: {
        "asset/*subroute": "asset"
    },

    asset: function() {
        this.renderPage(function() {
            return new AssetPage();
        });
    }
});