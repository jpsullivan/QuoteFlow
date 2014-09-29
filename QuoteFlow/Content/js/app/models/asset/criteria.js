"use strict";

var $ = require('jquery');
var _ = require('underscore');
var Backbone = require('backbone');
var Brace = require('backbone-brace');
Backbone.$ = $;

/**
 * 
 */
var AssetCriteriaModel = Brace.Model.extend({
    namedAttributes: ["id", "name"]
});

module.exports = AssetCriteriaModel;