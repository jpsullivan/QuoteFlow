"use strict";

var Marionette = require('backbone.marionette');
var LinkRegion = require('../../../../../../mixins/marionette-link-region');

/**
 * View used to render the body of an asset. It uses the AssetModel.
 * This view is just a container for panels, it does not render any data by itself.
 * @extends Marionette.Layout
 */
var AssetBodyLayout = Marionette.LayoutView.extend({
    template: JST["asset-viewer/asset/body"],
    tagName: "div",
    className: "asset-body-content",

    regions: {
        leftPanels: {
            selector: ".issue-main-column",
            regionType: Marionette.LinkRegion
        },
        rightPanels: {
            selector: ".issue-side-column",
            regionType: Marionette.LinkRegion
        },
        infoPanels: {
            selector: ".issue-body",
            regionType: Marionette.LinkRegion
        }
    }
});

module.exports = AssetBodyLayout;
