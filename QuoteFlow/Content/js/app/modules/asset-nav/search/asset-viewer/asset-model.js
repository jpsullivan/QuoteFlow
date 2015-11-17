"use strict";

var Brace = require('backbone-brace');

/**
 * Model for the AssetView.
 * @extends Brace.Model
 */
var AssetModel = Brace.Model.extend({

    namedEvents: [
        /**
         * @event updated
         * Triggered when the view asset model is updated with data from server. This is the main event
         * used by the related views to known when they should refresh the rendered data.
         */
        "updated"
    ],

    namedAttributes: [
        /**
         * Contains information about the asset, including summary and asset operations
         * @type {Object}
         */
        "entity",

        /**
         * Asset id
         * @type {number}
         */
        "id",

        /**
         * Asset SKU
         * @type {string}
         */
        "sku"
    ],

    /**
     * Default values for this model
     * @returns {Object} Object with default data
     */
    defaults: function () {
        return {
            entity: {}
        };
    },

    /**
     * Updates entity and panels with new data
     *
     * @param {Object} data
     * @param {Object} options
     * @param {string[]} [options.fieldsSaved]      The update may come as the result of a save. This array includes the ids of any fields that may have been saved before hand.
     * @param {string[]} [options.fieldsInProgress] Array of fields that are still in edit mode or still saving.
     * @param {boolean}  [options.initialize]       Parameter indicating if it is the first time the update has been called.
     * @param {Object}   [options.changed]          Changed data since last update
     * @param {boolean}  [options.mergeIntoCurrent] Parameter indicating if the data should be merged into current data
     *
     * //TODO initialize, changed and mergeIntoCurrent are used only by AssetHeaderView. Seems this could be simplified.
     */
    update: function (data, options) {
        var updated = false;

        // If we have new data about the asset, update the entity
        if (data.asset) {
            this.updateFromEntity(data.asset);
            updated = true;
        }

        // If something has been updated, trigger the "updated" event so our views
        // can render the new content
        if (updated) {
            this.trigger("updated", options);
        }
    },

    /**
     * Clears this model, restoring the defaults.
     *
     * //TODO This should be moved to a higher class, it is a very common operation (that should be already
     * provided by backbone)
     */
    resetToDefault: function () {
        this.clear();
        this.set(this.defaults());
    },

    /**
     * Update our fields based on a new entity
     *
     * @param {Object} entity Object containing the asset information
     */
    updateFromEntity: function (entity) {
        this.set({
            id: entity.id,
            sku: entity.sku,
            entity: entity
        });
    },

    /**
     * Check if the provided id matches the current asset
     *
     * @param {string|number} assetId ID to check
     * @returns {boolean}
     */
    isCurrentAsset: function (assetId) {
        return this.get("id") == assetId;
    },

    /**
     * Check if this model contains an asset
     *
     * @returns {boolean}
     */
    hasAsset: function () {
        return this.has("id");
    },

    /**
     * Updates the model with a new assetQuery
     *
     * @param {string} query New query
     */
    updateAssetQuery: function(query) {
        if (!this.hasAsset()) {
            return;
        }
        this.getEntity().viewAssetQuery = query;
    }
});

module.exports = AssetModel;
