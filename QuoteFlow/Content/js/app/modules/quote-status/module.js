"use strict";

var Marionette = require('backbone.marionette');

var QuoteStatusController = require('./controller');
var QuoteStatusRouter = require('./router');

/**
 *
 */
var AssetModule = Marionette.Module.extend({

    onStart: function (options) {
        return this.startMediator();
    },

    startMediator: function() {
        this.controller = new QuoteStatusController();

        return new QuoteStatusRouter({ controller: this.controller });
    }
});

module.exports = AssetModule;
