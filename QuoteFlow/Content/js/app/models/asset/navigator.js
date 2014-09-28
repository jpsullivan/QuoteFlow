"use strict";

var $ = require('jquery');
var _ = require('underscore');
var Backbone = require('backbone');
Backbone.$ = $;

/**
 * 
 */
var AssetNavigatorModel = Backbone.Model.extend({
    defaults: function() {
        return {
            selectedManufacturers: []
        };
    },

    addManufacturer: function(name) {
        var selected = this.get('selectedManufacturers');
        selected.push(name);
        this.set({ 'selectedManufacturers': selected });
    },

    removeManufacturer: function(name) {
        var selected = this.get('selectedManufacturers');
        var index = selected.indexOf(name);
        if (index > -1) {
            selected.splice(index, 1);
        }
        this.set({ 'selectedManufacturers': selected });
    }
});

module.exports = AssetNavigatorModel;