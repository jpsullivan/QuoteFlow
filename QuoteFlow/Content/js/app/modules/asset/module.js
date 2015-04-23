"use strict";

var Marionette = require('backbone.marionette');

var AssetController = require('./controller');
var AssetRouter = require('./router');

/**
 * 
 */
var AssetModule = Marionette.Module.extend({

    onStart: function (options) {
        return this.startMediator();
    },

    startMediator: function() {
        this.controller = new AssetController();

        return new AssetRouter({ controller: this.controller });
    }
});

module.exports = AssetModule;