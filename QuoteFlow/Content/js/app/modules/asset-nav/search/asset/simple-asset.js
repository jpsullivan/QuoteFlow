"use strict";

var Brace = require('backbone-brace');

/**
 * 
 */
var SimpleAsset = Brace.Model.extend({
    properties: ["id", "key"],
    defaults: {
        id: null,
        key: null
    },
    hasAsset: function () {
        return !!this.getId();
    }
});

module.exports = SimpleAsset;