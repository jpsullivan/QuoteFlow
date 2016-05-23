"use strict";

var _ = require('underscore');
var AssetViewerController = require('../asset-viewer/asset-viewer');
var AssetViewerAppModule = require('../asset-viewer/app-module');

var AssetEditorAppModule = AssetViewerAppModule.extend({
    name: "assetEditor",
    generateMasterRequest: true,

    create: function (options) {
        options = _.defaults({}, options, {
            showReturnToSearchOnError: function () { return false;}
        });

        return new AssetViewerController({
            showReturnToSearchOnError: options.showReturnToSearchOnError
        });
    },

    commands: function (module) {
        var viewerCommands = AssetViewerAppModule.prototype.commands.call(this, module);
        return _.extend(viewerCommands, {
            "editField": true
        });
    },

    requests: function (module) {
        var viewerRequests = AssetViewerAppModule.prototype.requests.call(this, module);
        return _.extend(viewerRequests, {
            "fields": function () {
                return module.getFields();
            },
            "hasSavesInProgress": true
        });
    },

    events: function (module) {
        var viewerEvents = AssetViewerAppModule.prototype.events.call(this, module);

        return viewerEvents.concat([
            "saveSuccess",
            "saveError",
            "editField",
            "fieldSubmitted"
        ]);
    }
});

module.exports = AssetEditorAppModule;
