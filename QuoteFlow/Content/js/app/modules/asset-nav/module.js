"use strict";

var _ = require('underscore');

var Marionette = require('backbone.marionette');

var AssetNavController = require('./controller');
var AssetNavCreator = require('./search/asset-nav-creator');
var AssetRouter = require('./router');
var AssetCustomRouter = require('./router-custom');

/**
 *
 */
var AssetNavModule = Marionette.Module.extend({

    onStart: function (options) {
        // // Workaround for Chrome bug firing a null popstate event on page load.
        // // Backbone should fix this!
        // // @see http://code.google.com/p/chromium/issues/detail?id=63040
        // // @see also JRADEV-14804
        // if (jQuery.browser.webkit) {
        //     QuoteFlow.application.ignorePopState = true;
        //     window.addEventListener('load', function() {
        //         setTimeout(function() {
        //           QuoteFlow.application.ignorePopState = false;
        //         }, 0);
        //     });
        // }

        QuoteFlow.application.ignorePopState = false;

        return this.startMediator(options);
    },

    startMediator: function (options) {
        this.controller = new AssetNavController();
        this.controller.builder();

        // return new AssetCustomRouter({
        //     controller: this.controller
        // });
    }
});

module.exports = AssetNavModule;
