"use strict";

var Marionette = require('backbone.marionette');

var AsyncFileUploadManager = require('../../../lib/async-file-upload');
var CatalogImportSetFields = require('./views/import_set_fields');
var CatalogImportSetOptionalFields = require('./views/import_set_optional_fields');

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

    showAssetsInteractive: function() {
        debugger;
    }
});

module.exports = CatalogController;

