"use strict";

var _ = require('underscore');
var Marionette = require('backbone.marionette');
var jqUi = require('jquery-ui');
var jqUiSidebar = require('jquery-ui-sidebar');

var DetailsLayoutLayout = Marionette.LayoutView.extend({
    template: JST["quote-builder/details-layout/layout"],

    regions: {
        assetsList: ".list-results-panel",
        assetEditor: ".asset-container",
        tools: ".tools-container"
    },

    ui: {
        detailPanel: '.detail-panel'
    },

    onRender: function () {
        this.assetsList._ensureElement();
    },

    maximizeDetailPanelHeight: function () {
        var assetContainerTop = this.ui.detailPanel.offset().top;
        this.ui.detailPanel.css("height", window.innerHeight - assetContainerTop);
    },

    showDraggable: function () {
        var oldSize = 0;

        this.assetsList.$el.sidebar({
            id: "layoutview-draggable",
            minWidth: function () {
                return 250;
            },
            maxWidth: _.bind(function () {
                return this.$el.width() - 500;
            }, this),
            resize: _.bind(function (newSize) {
                // This should be handled by the sidebar plugin :(
                if (newSize !== oldSize) {
                    oldSize = newSize;
                    this.trigger("resize");
                }
            }, this)
        });
    },

    updateDraggable: function () {
        this.assetsList.$el.sidebar("updatePosition");
    }
});

module.exports = DetailsLayoutLayout;
