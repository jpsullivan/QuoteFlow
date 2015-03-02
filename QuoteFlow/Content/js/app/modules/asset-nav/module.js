"use strict";

var _ = require('underscore');

var Marionette = require('backbone.marionette');

var AssetNavController = require('./controller');
var AssetNavCreator = require('./search/asset-nav-creator');
var AssetRouter = require('./router');

/**
 * 
 */
var AssetNavModule = Marionette.Module.extend({

    onStart: function (options) {
        return this.startMediator(options);
    },

    startMediator: function (options) {
        this.controller = new AssetNavController();
        return new AssetRouter({
            controller: this.controller
        });
    }
});

module.exports = AssetNavModule;