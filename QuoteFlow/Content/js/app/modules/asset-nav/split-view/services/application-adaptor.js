"use strict";

var existingHandlers = {
    "reqres": {},
    "commands": {}
};

function replaceHandlers (type, handlers) {
    for (var handlerName in handlers) {
        var implementation = handlers[handlerName];

        if (QuoteFlow.application.reqres.hasHandler(handlerName)) {
            existingHandlers[type][handlerName] = QuoteFlow.application.reqres.getHandler(handlerName);
        } else {
            existingHandlers[type][handlerName] = null;
        }

        QuoteFlow.application.reqres.setHandler(handlerName, implementation);
    }
}

function restoreReqRes (type) {
    for (var handlerName in existingHandlers[type]) {
        var implementation = existingHandlers[type][handlerName];

        if (implementation) {
            QuoteFlow.application.reqres.setHandler(handlerName, implementation);
        } else {
            QuoteFlow.application.reqres.removeHandler(handlerName);
        }

        delete existingHandlers[type][handlerName];
    }
}

var appAdaptor = {
    init: function (detailsLayout) {
        replaceHandlers("reqres", {
            "assetEditor:canDismissComment": function () {
                return detailsLayout.canDismissComment();
            },
            "assetEditor:getAssetId": function () {
                return detailsLayout.getActiveAssetId();
            },
            "assetEditor:getAssetSku": function () {
                return detailsLayout.getActiveAssetSku();
            },
            "assetEditor:refreshAsset": function () {
                return detailsLayout.refreshAsset();
            },
            "assetEditor:isCurrentlyLoading": function () {
                return detailsLayout.isLoading();
            },
            "assetEditor:hasSavesInProgress": function () {
                return detailsLayout.hasSavesInProgress();
            },
            "assetEditor:fields": function () {
                return detailsLayout.getEditorFields();
            }
        });

        replaceHandlers("commands", {
            "assetEditor:abortPending": function () {
                return detailsLayout.abortPending();
            },
            "assetEditor:beforeHide": function () {
                return detailsLayout.beforeHide();
            },
            "assetEditor:beforeShow": function () {
                return detailsLayout.beforeShow();
            },
            "assetEditor:removeAssetMetadata": function () {
                return detailsLayout.removeAssetMetadata();
            },
            "assetEditor:updateAssetWithQuery": function () {
                return detailsLayout.updateEditor();
            },
            "assetEditor:editField": function (field) {
                return detailsLayout.editField(field);
            },
            "assetEditor:addToQuote": function (asset) {
                return detailsLayout.addToQuote(asset);
            }
        });
    },

    destroy: function () {
        restoreReqRes("reqres");
        restoreReqRes("commands");
    }
};

module.exports = appAdaptor;
