"use strict";

var AssetsApi = {
    init: function (simpleAssetList) {
        QuoteFlow.API = {
            Assets: {}
        };
        QuoteFlow.API.Assets.nextAsset = function () {
            simpleAssetList.selectNext();
        };

        QuoteFlow.API.Assets.previousAsset = function () {
            simpleAssetList.selectPrevious();
        };
    }
};

module.exports = AssetsApi;
