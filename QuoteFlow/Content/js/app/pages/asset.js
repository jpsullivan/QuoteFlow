"use strict";

var $ = require('jquery');
var _ = require('underscore');
var Backbone = require('backbone');
var SubRoute = require('backbone.subroute');
Backbone.$ = $;

var BaseView = require('../view');

// UI Components
var EditAsset = require('../ui/asset/edit');
var ShowAsset = require('../ui/asset/show');

/**
 *
 */
var AssetPage = BaseView.extend({
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
    show: function (assetId, assetName) {
        var view = new ShowAsset();
    },

    /**
     *
     */
    edit: function (assetId, assetName) {
        // get the bootstrapped data
        var data = window.editAssetData;
        if (data) {
            var theAssetId = data.assetId;
            var assetVarNames = data.assetVarNames;

            var view = new EditAsset({
                assetId: theAssetId,
                assetVarNames: assetVarNames
            });
        }

        AJS.$(document).ready(function () {
            AJS.$('select').auiSelect2();
        });
    }
});

AssetPage.Router = Backbone.SubRoute.extend({
    routes: {
        "asset/:assetId/:assetName": "show",
        "asset/:assetId/:assetName/edit": "edit"
    },

    show: function (assetId, assetName) {
        return new AssetPage().show(assetId, assetName);
    },

    edit: function (assetId, assetName) {
        return new AssetPage().edit(assetId, assetName);
    }
});

module.exports = AssetPage;
