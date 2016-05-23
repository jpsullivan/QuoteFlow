"use strict";

var Marionette = require('backbone.marionette');

var CatalogController = require('./controller');
var CatalogRouter = require('./router');

/**
 *
 */
var CatalogModule = Marionette.Module.extend({

    onStart: function (options) {
        return this.startMediator();
    },

    startMediator: function () {
        this.controller = new CatalogController();

        return new CatalogRouter({ controller: this.controller });
    }
});

module.exports = CatalogModule;
