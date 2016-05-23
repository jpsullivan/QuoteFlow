"use strict";

var _ = require('underscore');
var AssetViewerAppModule = require('../asset-viewer/app-module');

var NoInlineAppModule = AssetViewerAppModule.extend({
    name: "assetEditor",

    requests: function (module) {
        var viewerRequests = AssetViewerAppModule.prototype.requests.call(this, module);
        return _.extend(viewerRequests, {
            "fields": function () {
                return [];
            },
            "hasSavesInProgress": function () {
                return false;
            }
        });
    }
});

module.exports = NoInlineAppModule;
