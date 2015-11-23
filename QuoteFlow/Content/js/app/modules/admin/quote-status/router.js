"use strict";

var Marionette = require('backbone.marionette');

/**
 *
 */
var QuoteStatusRouter = Marionette.AppRouter.extend({
    appRoutes: {
        "admin/status": "adminIndex"
    }
});

module.exports = QuoteStatusRouter;
