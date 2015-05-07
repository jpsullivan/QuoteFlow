"use strict";

var _ = require('underscore');
var Marionette = require('backbone.marionette');

var Utilities = require('../../../components/utilities');

/**
 * The split view "asset panel" (where the asset is rendered).
 */
var SplitScreenDetailView = Marionette.ItemView.extend({
    events: {
        "focus #addcomment textarea": "scrollToBottom"
    },

    issueDetailsNoSelectionTemplate: JST["quote-builder/split-view/asset-details-no-selection"],

    /**
     * @param {object} options
     * @param {AssetCacheManager} options.assetCacheManager The application's <tt>AssetCacheManager</tt>.
     * @param {AssetViewer} options.assetModule The application's <tt>AssetViewer</tt>.
     * @param {SearchModule} options.search The application's <tt>SearchModule</tt>.
     */
    initialize: function (options) {
        _.bindAll(this, "adjustHeight", "_renderIssue");

        Utilities.initializeResizeHooks();

        this.assetCacheManager = options.assetCacheManager;
        this.search = options.search;
        this.searchResults = options.search.getResults();

        QuoteFlow.application.on("issueEditor:fieldSubmitted", this.focus, this);
        QuoteFlow.application.on("issueEditor:replacedFocusedPanel", this.focus, this);
        this.adjustHeight = _.debounce(this.adjustHeight, options.easeOff);
    },

    adjustHeight: function () {
        _.defer(_.bind(function () {
            var assetContainer = this.getAssetContainer(),
                issueContainerTop;
            if (assetContainer.length) {
                issueContainerTop = assetContainer.length && assetContainer.offset().top;
                assetContainer.css("height", window.innerHeight - issueContainerTop);
            }
        }, this));
    },

    getAssetContainer: function () {
        return this.$el.find(".issue-container");
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
        if (this.isActive) return this;

        var instance = this;
        this.$el.focusin(function () {
            instance.focused = true;
        });
        this.$el.focusout(function () {
            instance.focused = false;
        });

        this.listenTo(this.searchResults, "selectedIssueChange", this.render, this);
        this.listenTo(this.searchResults, "issueUpdated", this.onAssetUpdated, this);
        this.listenTo(this.searchResults, "issueDoesNotExist", this._onAssetDoesNotExist, this);

        //JIRA.bind(JIRA.Events.NEW_CONTENT_ADDED, this.fixMentionsDropdownInMentionableFields);

        QuoteFlow.Interactive.onVerticalResize(this.adjustHeight);
        QuoteFlow.application.on("issueEditor:loadComplete", this.adjustHeight, this);
        this.isActive = true;
        return this;
    },

    onBeforeDestroy: function () {
        QuoteFlow.application.execute("issueEditor:abortPending");
        QuoteFlow.Interactive.offVerticalResize(this.adjustHeight);
        QuoteFlow.application.off("issueEditor:loadComplete", this.adjustHeight);
        //JIRA.unbind(JIRA.Events.NEW_CONTENT_ADDED, this.fixMentionsDropdownInMentionableFields);
        //JIRA.Issues.BaseView.prototype.deactivate.apply(this, arguments);
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
        if (reason === JIRA.CONTENT_ADDED_REASON.panelRefreshed ||
            reason === JIRA.CONTENT_ADDED_REASON.inlineEditStarted
            ) {
            // Search for mentionable elements, add markup to force Mentions drop down
            // to follow the scroll of .split-view .issue-container
            $context.find(".mentionable:not([data-follow-scroll])").attr("follow-scroll", ".split-view .issue-container");

            // Search for mentionable elements, add markup to force Mentions drop down
            // to push the scroll of .issue-body-content if there is not enough room to
            // display the drop down
            $context.find(".mentionable:not([data-push-scroll])").attr("push-scroll", ".issue-body-content");

        }
    },

    onAssetUpdated: function (id, entity, reason) {
        if (reason.action !== JIRA.Issues.Actions.INLINE_EDIT && reason.action !== JIRA.Issues.Actions.ROW_UPDATE) {
            return QuoteFlow.application.request("issueEditor:refreshIssue", reason);
        } else {
            return jQuery.Deferred().resolve();
        }
    },

    render: function (model, options) {
        options = options || {};
        if (this.searchResults.hasAssets() && !this.search.isStandAloneAsset() && options.reason !== "issueLoaded") {
            if (this.searchResults.hasSelectedAsset()) {
                QuoteFlow.application.execute("issueEditor:abortPending");
                JIRA.Issues.Utils.debounce(this, "_renderAsset", this.searchResults.getSelectedAsset());
            } else {
                this._renderNoAsset();
            }
        }
        this.adjustHeight();
        return this;
    },

    _renderIssue: function (selectedAsset) {
        QuoteFlow.application.request("issueEditor:loadIssue", {
            id: selectedAsset.get("id"),
            key: selectedAsset.get("key"),
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
        QuoteFlow.application.execute("issueEditor:setContainer", this.$el);
        this.render();
    },

    _renderNoAsset: function () {
        this.$el.html(this.issueDetailsNoSelectionTemplate());
    },

    hasFocus: function () {
        return this.focused === true;
    }
});

module.exports = SplitScreenDetailView;
