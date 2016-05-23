"use strict";

var Brace = require('backbone-brace');

var PagerModel = Brace.Model.extend({
    namedAttributes: [
        "nextAsset",
        "previousAsset",
        "position",
        "resultCount",
        "hasSearchLink"
    ],

    update: function (properties) {
        // TODO Refactor this, as isSplitView is not a concern for this module
        properties.hasSearchLink = !properties.isSplitView;
        delete properties.isSplitView;
        this.set(properties);
    }
});

module.exports = PagerModel;
