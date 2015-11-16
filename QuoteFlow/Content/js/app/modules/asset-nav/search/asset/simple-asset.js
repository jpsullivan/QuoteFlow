"use strict";

var Brace = require('backbone-brace');

/**
 *
 */
var SimpleAsset = Brace.Model.extend({
    namedAttributes: ["id", "sku"],

    defaults: {
        id: null,
        sku: null
    },
    hasAsset: function () {
        return !!this.getId();
    }
});

module.exports = SimpleAsset;
