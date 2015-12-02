"use strict";

var Brace = require('backbone-brace');

/**
 * [AssetVarModel description]
 * @extends Brace.Model
 */
var AssetVarModel = Brace.Model.extend({

    namedAttributes: [

        /**
         * The asset var ID.
         * @type {number}
         */
        "id",

        /**
         * The asset var name.
         * @type {string}
         */
        "name",

        /**
         * @type {string}
         */
        "description",

        /**
         * The value type of this asset var. Defaults to "String".
         * @type {string}
         */
        "valueType",

        /**
         * The ID of the organization this asset var belongs to.
         * @type {number}
         */
        "organizationId",

        /**
         * Whether or not this asset var is enabled.
         * @type {boolean}
         */
        "enabled",

        /**
         * When this asset var was created.
         * @type {DateTime}
         */
        "createdUtc",

        /**
         * The ID of the user who created this asset var.
         * @type {number}
         */
        "createdBy"
    ],

    /**
     * Default values for this model
     * @returns {Object} Object with default data
     */
    defaults: function () {
        return {
            valueType: "String",
            enabled: true
        };
    },

    url: function() {
        return QuoteFlow.RootUrl + "api/assetvar";
    },

    /**
     * Whether or not this asset var is enabled.
     * @return {Boolean} True if enabled, otherwise false.
     */
    isEnabled: function() {
        var enabled = this.getEnabled();
        if (enabled === false || enabled === undefined) {
            return false;
        }
        return true;
    },
});

module.exports = AssetVarModel;
