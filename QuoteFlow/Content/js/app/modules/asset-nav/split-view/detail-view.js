"use strict";

var _ = require('underscore');
var Marionette = require('backbone.marionette');

var ContentAddedReason = require('../util/reasons');
var EventTypes = require('../util/types');
var Utilities = require('../../../components/utilities');

/**
 * The split view "asset panel" (where the asset is rendered).
 */
var SplitScreenDetailView = Marionette.ItemView.extend({
    assetDetailsNoSelectionTemplate: JST["quote-builder/split-view/asset-details-no-selection"],

    events: {
        "focus #addcomment textarea": "scrollToBottom"
    },

    /**
     * @param {object} options
     * @param {AssetCacheManager} options.assetCacheManager The application's <tt>AssetCacheManager</tt>.
     * @param {AssetViewer} options.assetModule The application's <tt>AssetViewer</tt>.
     * @param {SearchModule} options.search The application's <tt>SearchModule</tt>.
     */
    initialize: function (options) {
        _.bindAll(this, "adjustHeight", "_renderAsset");

        Utilities.initializeResizeHooks();

        this.assetCacheManager = options.assetCacheManager;
        this.search = options.search;
        this.searchResults = options.search.getResults();

        QuoteFlow.application.on("assetEditor:fieldSubmitted", this.focus, this);
        QuoteFlow.application.on("assetEditor:replacedFocusedPanel", this.focus, this);
        this.adjustHeight = _.debounce(this.adjustHeight, options.easeOff);
    },

    adjustHeight: function () {
        _.defer(_.bind(function () {
            var assetContainer = this.getAssetContainer();
            var assetContainerTop;
            if (assetContainer.length) {
                assetContainerTop = assetContainer.length && assetContainer.offset().top;
                assetContainer.css("height", window.innerHeight - assetContainerTop);
            }
        }, this));
    },

    getAssetContainer: function () {
        return this.$el.find(".asset-container");
    },

    scrollToTop: function () {
        var $assetContainer = this.getAssetContainer();
        if ($assetContainer.size()) {
            $assetContainer.scrollTop(0);
        }
    },

    scrollToBottom: function () {
        var $assetContainer = this.getAssetContainer();
        if ($assetContainer.size()) {
            $assetContainer.scrollTop($assetContainer.prop("scrollHeight"));
        }
    },

    /**
     * Activates this SplitScreenDetailView.
     *
     * @return {SplitScreenDetailView} this
     */
    activate: function () {
        // if this view is already activated, do nothing
        if (this.isActive) {
            return this;
        }

        var instance = this;
        this.$el.focusin(function () {
            instance.focused = true;
        });
        this.$el.focusout(function () {
            instance.focused = false;
        });

        this.addListener(this.searchResults, "selectedAssetChange", this.render, this);
        this.addListener(this.searchResults, "assetUpdated", this.onAssetUpdated, this);
        this.addListener(this.searchResults, "assetDoesNotExist", this._onAssetDoesNotExist, this);

        QuoteFlow.bind(EventTypes.NEW_CONTENT_ADDED, this.fixMentionsDropdownInMentionableFields);

        QuoteFlow.Interactive.onVerticalResize(this.adjustHeight);
        QuoteFlow.application.on("assetEditor:loadComplete", this.adjustHeight, this);
        this.isActive = true;
        return this;
    },

    onBeforeDestroy: function () {
        QuoteFlow.application.execute("assetEditor:abortPending");
        QuoteFlow.Interactive.offVerticalResize(this.adjustHeight);
        QuoteFlow.application.off("assetEditor:loadComplete", this.adjustHeight);
        QuoteFlow.unbind(EventTypes.NEW_CONTENT_ADDED, this.fixMentionsDropdownInMentionableFields);
        // JIRA.Issues.BaseView.prototype.deactivate.apply(this, arguments);
        this.isActive = false;
    },

    /**
     * Adds markup to hide all mentions drop down on mentionable fields when the user scrolls
     *
     * @param event
     * @param $context
     * @param reason
     */
    fixMentionsDropdownInMentionableFields: function (event, $context, reason) {
        if (reason === ContentAddedReason.panelRefreshed || reason === ContentAddedReason.inlineEditStarted) {
            // Search for mentionable elements, add markup to force Mentions drop down
            // to follow the scroll of .split-view .asset-container
            $context.find(".mentionable:not([data-follow-scroll])").attr("follow-scroll", ".split-view .asset-container");

            // Search for mentionable elements, add markup to force Mentions drop down
            // to push the scroll of .asset-body-content if there is not enough room to
            // display the drop down
            $context.find(".mentionable:not([data-push-scroll])").attr("push-scroll", ".asset-body-content");
        }
    },

    onAssetUpdated: function (id, entity, reason) {
        if (reason.action !== JIRA.Issues.Actions.INLINE_EDIT && reason.action !== JIRA.Issues.Actions.ROW_UPDATE) {
            return QuoteFlow.application.request("assetEditor:refreshAsset", reason);
        } else {
            return jQuery.Deferred().resolve();
        }
    },

    render: function (model, options) {
        options = options || {};
        if (this.searchResults.hasAssets() && !this.search.isStandAloneAsset() && options.reason !== "assetLoaded") {
            if (this.searchResults.hasSelectedAsset()) {
                QuoteFlow.application.execute("assetEditor:abortPending");
                Utilities.debounce(this, "_renderAsset", this.searchResults.getSelectedAsset());
            } else {
                this._renderNoAsset();
            }
        }
        this.adjustHeight();
        return this;
    },

    _renderAsset: function (selectedAsset) {
        QuoteFlow.application.request("assetEditor:loadAsset", {
            id: selectedAsset.get("id"),
            detailView: true
        }).always(_.bind(function () {
            QuoteFlow.application.execute("pager:update", _.extend({}, this.searchResults.getPager(), { isSplitView: true }));
            // when the first asset is selected, prefetch the second one
            if (this.searchResults.isFirstAssetSelected() && this.searchResults.getDisplayableTotal() > 1) {
                this.assetCacheManager.scheduleAssetToBePrefetched(this.searchResults.getNextAssetForSelectedAsset());
            }
            this.assetCacheManager.prefetchAssets();
            this.$el.addClass("active");
            this.scrollToTop();
        }, this));
    },

    focus: function () {
        if (!this.hasFocus()) {
            this.getAssetContainer().focus();
        }
    },

    blur: function () {
        this.getAssetContainer().blur();
    },

    _onAssetDoesNotExist: function () {
        this._renderNoAsset();
        QuoteFlow.application.execute("assetEditor:setContainer", this.$el);
        this.render();
    },

    _renderNoAsset: function () {
        this.$el.html(this.assetDetailsNoSelectionTemplate());
    },

    hasFocus: function () {
        return this.focused === true;
    }
});

module.exports = SplitScreenDetailView;
