"use strict";

var $ = require('jquery');
var Marionette = require('backbone.marionette');

var EventTypes = require('../../../../util/types.js');
var Reasons = require('../../../../util/reasons');
var ReplaceRegion = require('../../../../../../mixins/marionette-replace-region');

/**
 * Main layout for rendering an asset.
 * @extends Marionette.Layout
 */
var AssetLayout = Marionette.LayoutView.extend({
    template: JST["asset-viewer/asset/layout"],

    className: "asset-container",

    regions: {
        header: ".asset-header-container",
        body: ".asset-body-container"
    },

    modelEvents: {
        "updated": "update"
    },

    /**
     * Show the loading indication
     */
    showLoading: function () {
        this.$el.addClass("loading");
    },

    /**
     * Hide the loading indication
     */
    hideLoading: function () {
        this.$el.removeClass("loading");
    },

    /**
     * Focus a comment.
     * @param {string} commentId Comment to focus
     */
    focusComment: function (commentId) {
        this.$("#activitymodule .focused").removeClass("focused");
        this.$("#comment-" + commentId).addClass("focused");
        this.$(".js-stalker").trigger("refresh");
    },

    /**
     * Update this view with the data form the model.
     * @param props
     */
    update: function (props) {
        // Only render this view if this is the initial view.
        if (props.initialize) {
            this.render();
        }

        // // If a comment has been edited, focus it
        // if (props.reason && props.reason.action === JIRA.Issues.Actions.EDIT_COMMENT) {
        //     this.focusComment(props.reason.meta.commentId);
        // }

        this._bringViewIssueElementIntoView();
    },

    /**
     * Remove the view
     *
     * Override Backbone's method, as we don't want to remove the container from the view.
     *
     * @returns {*}
     */
    remove: function () {
        this.stopListening();
        this.$el.empty();
        return this;
    },

    /**
     * Handler for render events, things to do after the template has been rendered
     */
    onRender: function () {
        // $el could have been modified outside this view, we need to restore the className
        this.$el.addClass(this.className);
        this.$el.attr("tabindex", "-1");
        this._bringViewIssueElementIntoView();

        QuoteFlow.trigger(EventTypes.NEW_CONTENT_ADDED, [this.$el, Reasons.pageLoad]);
        if (EventTypes.REFRESH_TOGGLE_BLOCKS) {
            QuoteFlow.trigger(EventTypes.REFRESH_TOGGLE_BLOCKS, [this.model.getId()]);
        }
    },

    onApplyToDom: function () {
        this._bringViewIssueElementIntoView();
    },

    /**
     * Bring some parts of the asset into view (eg: scrolls to focused comment).
     * @private
     */
    _bringViewIssueElementIntoView: function () {
        var viewAssetQuery = this.model.get("entity").viewAssetQuery;
        if (viewAssetQuery) {
            var elementSelector;
            if (viewAssetQuery.focusedCommentId) {
                elementSelector = "#comment-" + viewAssetQuery.focusedCommentId;
            } else if (viewAssetQuery.attachmentSortBy || viewAssetQuery.attachmentOrder) {
                elementSelector = "#attachmentmodule";
            }

            if (elementSelector) {
                $(elementSelector).scrollIntoView({
                    marginBottom: 200,
                    marginTop: 200
                });
            }
        }
    }
});

module.exports = AssetLayout;
