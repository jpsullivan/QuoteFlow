"use strict";

var _ = require('underscore');
var Marionette = require('backbone.marionette');

var DetailsLayoutLayout = Marionette.LayoutView.extend({
    template: JST["quote-builder/details-layout/layout"],

    regions: {
        issuesList: ".list-results-panel",
        issueEditor: ".issue-container",
        pager: ".pager-container"
    },

    ui: {
        detailPanel: '.detail-panel'
    },

    onRender: function () {
        this.issuesList._ensureElement();
    },

    maximizeDetailPanelHeight: function () {
        var issueContainerTop = this.ui.detailPanel.offset().top;
        this.ui.detailPanel.css("height", window.innerHeight - issueContainerTop);
    },

    showDraggable: function () {
        var oldSize = 0;

        this.issuesList.$el.sidebar({
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
        this.issuesList.$el.sidebar("updatePosition");
    }
});

module.exports = DetailsLayoutLayout;
