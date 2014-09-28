"use strict";

var $ = require('jquery');
var _ = require('underscore');
var Backbone = require('backbone');
Backbone.$ = $;

var AssetVarModel = require('../models/asset_var');

/**
 * 
 */
var AssetVarCollection = Backbone.Collection.extend({
    model: AssetVarModel,

    url: function() {
        return QuoteFlow.RootUrl + "api/assetvar";
    }
});

module.exports = AssetVarCollection;