"use strict";

var AssetsApi = require('../asset-nav/search/assets-api');
var PagerComponent = require('./component');

var PagerAppModule = function (PagerModule, app) {
    var pager = new PagerComponent({
        nextItem: function () {
            // TODO Refactor to app commands
            AssetsApi.nextAsset();
        },
        previousItem: function () {
            // TODO Refactor to app commands
            AssetsApi.prevAsset();
        },
        goBack: function () {
            app.execute("returnToSearch");
        }
    });

    app.addInitializer(function () {
        pager.initialize();
        app.commands.setHandlers({
            "pager:update": pager.update,
            "pager:render": pager.show,
            "pager:close":  pager.close
        });
    });
};

module.exports = PagerAppModule;
