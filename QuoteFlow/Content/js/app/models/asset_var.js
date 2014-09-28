"use strict";

var $ = require('jquery');
var _ = require('underscore');
var Backbone = require('backbone');
Backbone.$ = $;

/**
 * 
 */
var AssetVarModel = Backbone.Model.extend({
    url: function() {
        return QuoteFlow.RootUrl + "api/assetvar";
    },

    defaults: function() {
        return {
            Id: null,
            Name: "",
            Description: "",
            ValueType: "",
            OrganizationId: "",
            Enabled: true,
            CreatedUtc: null,
            CreatedBy: 0
        }
    },

    isEnabled: function() {
        var enabled = this.get("Enabled");
        if (enabled === false || enabled === undefined) {
            return false;
        }
        return true;
    },
});

module.exports = AssetVarModel;