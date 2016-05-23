"use strict";

var _ = require('underscore');

/**
 * This service is responsible for adding/removing metadata about an asset.
 * @type {Object}
 */
var MetadataService = {
    /**
     * Removes all the asset meta data such as asset sku from AJS.Meta
     * @param {AssetModel} model Model used to extract the values to remove
     */
    removeAssetMetadata: function (model) {
        var assetEntity = model.getEntity();
        if (assetEntity.metadata) {
            _.each(assetEntity.metadata, function (value, key) {
                AJS.Meta.set(key, null);
            });
        } else if (AJS.Meta.get("asset-sku")) {
            AJS.Meta.set("asset-sku", null);
        }
    },

    /**
     * Add all the issue meta data such as issue key into AJS.Meta
     *
     * @param {AssetModel} model Model used to extract the values to add
     */
    addAssetMetadata: function (model) {
        var assetEntity = model.getEntity();
        _.each(assetEntity.metadata, function (value, key) {
            AJS.Meta.set(key, value);
        });
    }
};

module.exports = MetadataService;
