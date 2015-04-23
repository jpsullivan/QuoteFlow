"use strict";

var Marionette = require('backbone.marionette');

/**
 * Contains callbacks for the asset module router.
 */
var AssetController = Marionette.Controller.extend({

    create: function () {
        AJS.$('select').auiSelect2();
    }
});

module.exports = AssetController;
