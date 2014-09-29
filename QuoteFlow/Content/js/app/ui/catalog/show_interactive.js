"use strict";

var $ = require('jquery');
var _ = require('underscore');
var Backbone = require('backbone');
Backbone.$ = $;

var BaseView = require('../../view');

var AssetNavigator = require('../asset/asset_navigator');
var ShowAssetPage = require('../asset/show');;

/**
 *
 */
var ShowAssetsInteractive = BaseView.extend({
    el: ".asset-container",

    options: {},

    presenter: function () {
        return _.extend(this.defaultPresenter(), {

        });
    },

    initialize: function(options) {
        var assetList = new AssetNavigator();
        var assetDetails = new ShowAssetPage();
    },

    postRenderTemplate: function () { }
});

module.exports = ShowAssetsInteractive;