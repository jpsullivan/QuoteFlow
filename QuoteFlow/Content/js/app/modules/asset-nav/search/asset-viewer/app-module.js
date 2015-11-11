"use strict";

var _ = require('underscore');
var AppModule = require('../../../../base/app-module');
var AssetViewerController = require('./asset-viewer');

var AssetViewerAppModule = AppModule.extend({
    name: "assetViewer",
    generateMasterRequest: true,

    create: function(options) {
        options = _.defaults({}, options, {
            showReturnToSearchOnError: function() { return false;}
        });

        return new AssetViewerController({
            showReturnToSearchOnError: options.showReturnToSearchOnError
        });
    },

    commands: function() {
        return {
            abortPending: true,
            beforeHide: true,
            beforeShow: true,
            removeAssetMetadata: true,
            updateAssetWithQuery: true,
            close: true,
            setContainer: true,
            dismiss: true
        };
    },

    requests: function() {
        return {
            loadAsset: true,
            canDismissComment: true,
            getAssetId: true,
            refreshAsset: true,
            isCurrentlyLoading: true
        };
    },

    events: function() {
        return [
            "loadComplete",
            "loadError",
            "close",
            "render",
            "replacedFocusedPanel"
        ];
    }
});

module.exports = AssetViewerAppModule;
