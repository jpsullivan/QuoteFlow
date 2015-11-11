"use strict";

var _ = require('underscore');
var Marionette = require('backbone.marionette');

var EventTypes = require('../../../../util/types');
var Reasons = require('../../../../util/reasons');

/**
 * View for rendering the header of an asset. It renders the id, name, etc.
 * Also renders the opsbar and pager as regions.
 * @extends Marionette.Layout
 */
var AssetHeaderLayout = Marionette.LayoutView.extend({
    template: JST["asset-viewer/asset/header"],
    className: "asset-header js-stalker",
    id: "stalker",
    tagName: "header",

    modelEvents: {
        "updated": "update"
    },

    regions: {
        opsbar: ".command-bar",
        pager: "#asset-header-pager"
    },

    /**
     * Extract the data from the model in the format needed by the template
     *
     * @returns {Object} Data to be rendered by the template
     */
    serializeData: function () {
        return {
            asset: this.model.getEntity(),
            hasProjectShortcut: false
        };
    },

    /**
     * Update this view with new data
     *
     * @param options
     */
    update: function (options) {
        if (options.initialize) {
            this._updateWindowTitle();
        } else {
            var editingSummary = _.include(options.fieldsInProgress, "summary");
            if (editingSummary) {
                this.renderOpsBar();
            } else {
                this.render();
            }
        }
        this.trigger("updated");
    },

    /**
     * Handler for applyToDom event, things to do after $el has been loaded from the DOM
     */
    onApplyToDom: function () {
        var view = new JIRA.Components.IssueViewer.Views.IssueOpsbar({model: this.model});
        // Since ops bar already is in the dom, we should use the current dom data as the view's element
        view.setElement(this.opsbar.el);
        this.opsbar.attachView(view);
        this.opsbar.currentView.applyToDom();
    },

    /**
     * Handler for render event, things to do after the template has been rendered
     */
    onRender: function () {
        this.renderOpsBar();
        this._updateWindowTitle();
        this.trigger("panelRendered", "header", this.$el);
    },

    /**
     * Render the operations bar
     *
     * //TODO This composition should be done by the AssetController
     */
    renderOpsBar: function () {
        this.opsbar.show(new JIRA.Components.IssueViewer.Views.IssueOpsbar({model: this.model}));
        //TODO This event should be thrown by the AssetController
        QuoteFlow.trigger(EventTypes.NEW_CONTENT_ADDED, [this.$el, Reasons.CONTENT_ADDED_REASON.panelRefreshed]);
    },

    /**
     * Updates the window title to contain information about the issue
     *
     * @private
     */
    _updateWindowTitle: function () {
        var entity = this.model.getEntity();
        var sku = entity.sku;
        var summary = entity.summary;
        var appTitle = "QuoteFlow";

        if (appTitle && summary && sku) {
            document.title = "[" + sku + "] " + summary + " - " + appTitle;
        }
    }
});

module.exports = AssetHeaderLayout;
