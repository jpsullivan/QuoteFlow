"use strict";

var $ = require('jquery');
var _ = require('underscore');
var Backbone = require('backbone');
Backbone.$ = $;

/**
 * 
 */
var AssetCriteriaModel = Backbone.Model.extend({
    namedAttributes: ["id", "name"]
});

module.exports = AssetCriteriaModel;