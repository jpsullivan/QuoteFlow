"use strict";

var Marionette = require('backbone.marionette');

/**
 * Contains callbacks for the asset module router.
 */
var AssetController = Marionette.Controller.extend({

    create: function () {
        AJS.$(document).ready(function () {
            AJS.$('#catalog_expiration_date').datePicker({ 'overrideBrowserDefault': true });
        });
    },

    importCatalog: function () {
        AsyncFileUploadManager.init(window.asyncActionUrl, 'uploadForm', window.asyncJqueryFallback);
    },

    verify: function () {
        var view = new CatalogImportSetFields({ rawRows: window.rawRows });
    },

    verifySecondary: function () {
        var view = new CatalogImportSetOptionalFields({
            headers: window.headers,
            rawRows: window.rawRows
        });
    },

    showAssetsInteractive: function () {
        debugger;
    }
});

module.exports = AssetController;

