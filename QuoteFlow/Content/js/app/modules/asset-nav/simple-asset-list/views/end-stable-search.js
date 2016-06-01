"use strict";

var Marionette = require('backbone.marionette');

var EndStableSearchView = Marionette.ItemView.extend({
    template: JST["quote-builder/simple-asset-list/end-of-stable-search"]
});

module.exports = EndStableSearchView;
