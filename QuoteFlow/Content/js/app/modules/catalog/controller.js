"use strict";

var AsyncFileUploadManager = require('../../../lib/async-file-upload');
var Marionette = require('backbone.marionette');

/**
 * Contains callbacks for the catalog module router.
 */
var CatalogController = Marionette.Controller.extend({

    create: function () {
        AJS.$(document).ready(function () {
            AJS.$('#catalog_expiration_date').datePicker({ 'overrideBrowserDefault': true });
        });
    },

    importCatalog: function() {
        AsyncFileUploadManager.init(
                window.asyncActionUrl,
                'uploadForm',
                window.asyncJqueryFallback);
    },

    show: function() {
        debugger;
    },

    showAssets: function() {
        debugger;
    },

    showAssetsInteractive: function() {
        debugger;
    }
});

module.exports = CatalogController;

