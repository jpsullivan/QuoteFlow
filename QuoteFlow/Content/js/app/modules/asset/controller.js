"use strict";

var Marionette = require('backbone.marionette');

/**
 * Contains callbacks for the catalog module router.
 */
var AssetController = Marionette.Controller.extend({

    create: function () {
        debugger;
        AJS.$('select').auiSelect2();
    }
});

module.exports = AssetController;
