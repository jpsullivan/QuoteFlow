"use strict";

var Asset = require('./asset');

var InaccessibleAssetView = Asset.extend({
    template: JST["quote-builder/simple-asset-list/inaccessible-asset"],

    triggers: {
        'click': "select"
    },

    serializeData: function () {
        var model = this.model;
        return {
            "key": model.get("key")
        };
    }
});

module.exports = InaccessibleAssetView;
