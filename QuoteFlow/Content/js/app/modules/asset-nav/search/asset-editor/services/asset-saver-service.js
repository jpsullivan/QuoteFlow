"use strict";

var _ = require('underscore');
var Marionette = require('backbone.marionette');
var AssetFieldUtil = require('../../asset-viewer/legacy/asset-field-util');

/**
 * Service responsible for saving an asset.
 * This is just a wrapper for saveInProgressManager, we should refactor
 * it and join these two services.
 * @class
 * @extends Marionette.Controller
 */
var AssetSaverService = Marionette.Controller.extend({
    /**
     * Initialize the service
     *
     * @constructor
     * @param {object} options
     * @param {SaveInProgressManager} object.saveInProgressManager
     * @param {AssetModel} object.model
     */
    initialize: function (options) {
        this.saveInProgressManager = options.saveInProgressManager;
        this.model = options.model;

        // TODO: flaky. Assumes that requests will always come back in the order
        // in which they were started. This is ok as are refactoring
        // save & reload to be in the same request, which will make this much easier.
        this._saveStarted = [];

        this.saveInProgressManager.onSavingStarted(_.bind(function (assetId, savedFieldIds) {
            this._saveStarted.push((new Date()).getTime());
            this.trigger("saveStarted", assetId, savedFieldIds);
        }, this));

        this.saveInProgressManager.onSaveSuccess(_.bind(function (assetId, assetSku, savedFieldIds, response) {
            if (response && response.fields) {
                AssetFieldUtil.transformFieldHtml(response);
            }
            this.trigger("save", {
                assetId: assetId,
                assetSku: assetSku,
                savedFieldIds: savedFieldIds,
                response: response,
                shouldSkipSaveAssetSuccessHandler: this._shouldSkipSaveAssetSuccessHandler,
                duration: (new Date()).getTime() - this._saveStarted.shift()
            });
        }, this));

        this.saveInProgressManager.onSaveError(_.bind(function (assetId, assetSku, attemptedSavedIds, response) {
            if (response && response.fields) {
                AssetFieldUtil.transformFieldHtml(response);
            }

            this.trigger("error", {
                assetId: assetId,
                assetSku: assetSku,
                attemptedSavedIds: attemptedSavedIds,
                response: response,
                duration: (new Date()).getTime() - this._saveStarted.shift()
            });
        }, this));
    },

    /**
     * Skip the next SaveAssetSuccess request.
     * @param {Boolean} skipHandler True to skip the handler
     */
    setSkipSaveAssetSuccessHandler: function (skipHandler) {
        this._shouldSkipSaveAssetSuccessHandler = skipHandler;
    },

    /**
     * Saves an asset.
     * @param assetId
     * @param assetSku
     * @param toSaveIds
     * @param params
     * @param ajaxProperties
     */
    save: function (assetId, assetSku, toSaveIds, params, ajaxProperties) {
        this.saveInProgressManager.saveAsset(assetId, assetSku, toSaveIds, params, ajaxProperties);
    }
});

module.exports = AssetSaverService;
